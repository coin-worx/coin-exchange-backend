using System;
using System.Collections.Generic;
using System.Configuration;
using System.Timers;
using BitcoinLib.Responses;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Bitcoin;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Service for interacting with the Bitcoin Client
    /// </summary>
    public class CoinClientService : ICoinClientService
    {
        private ICoinService _bitcoinService;
        private IFundsValidationService _fundsValidationService;
        private IDepositRepository _depositRepository;
        private IDepositAddressRepository _depositAddressRepository;
        private IDepositIdGeneratorService _depositIdGeneratorService;
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private List<string> _currencies; 
        private Timer _timer = null;        
        private Dictionary<ICoinService, string> _serviceToBlockHashDictionary = new Dictionary<ICoinService, string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CoinClientService(IFundsValidationService fundsValidationService, IDepositRepository depositRepository, 
            IDepositAddressRepository depositAddressRepository, IDepositIdGeneratorService depositIdGeneratorService,
            IFundsPersistenceRepository fundsPersistenceRepository)
        {
            _fundsValidationService = fundsValidationService;
            _depositRepository = depositRepository;
            _depositAddressRepository = depositAddressRepository;
            _depositIdGeneratorService = depositIdGeneratorService;
            _fundsPersistenceRepository = fundsPersistenceRepository;

            PopulateCurrencies();
            PopulateServices();
            StartTimer();
        }

        /// <summary>
        /// Populates the currencies
        /// </summary>
        public void PopulateCurrencies()
        {
            _currencies = new List<string>();
            _currencies.Add(CurrencyConstants.Btc);
        }

        /// <summary>
        /// Populates all the coin services
        /// </summary>
        public void PopulateServices()
        {
            bool useBitcoinTestNet = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("UseTestNet"));
            _bitcoinService = new BitcoinService(useTestnet:useBitcoinTestNet);
        }

        /// <summary>
        /// Starts the Timer
        /// </summary>
        private void StartTimer()
        {
            _timer = new Timer(Convert.ToDouble(ConfigurationManager.AppSettings.Get("PollingInterval")));
            _timer.Elapsed += PollIntervalElapsed;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Returns the appropriate Coin Service corresponding to the given currency
        /// </summary>
        /// <returns></returns>
        private ICoinService GetSpecificCoinService(string currency)
        {
            switch (currency)
            {
                case CurrencyConstants.Btc:
                    return _bitcoinService;
            }
            return null;
        }

        /// <summary>
        /// Checks for new Deposits
        /// </summary>
        /// <returns></returns>
        public void PollIntervalElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (var currency in _currencies)
            {
                CheckSingleCurrencyDeposit(GetSpecificCoinService(currency), currency);
            }
        }

        /// <summary>
        /// Checks the deposit for a single currency
        /// </summary>
        public void CheckSingleCurrencyDeposit(ICoinService coinService, string currency)
        {
            //List<TransactionSinceBlock> oldTransactions = null;
            string blockHash = null;
            
            _serviceToBlockHashDictionary.TryGetValue(coinService, out blockHash);
            if (string.IsNullOrEmpty(blockHash))
            {
                _serviceToBlockHashDictionary.Add(coinService, coinService.GetBestBlockHash());                
            }
            else
            {
                // Get the list of blocks from the point we last time checked for blocks, by providing the last blockHash
                // recorded last time 
                ListSinceBlockResponse blocksList = coinService.ListSinceBlock(blockHash, 1);

                // Check for new transactions
                CheckNewTransactions(currency, blocksList.Transactions);

                // Poll for confirmations of all the deposits that have else than 7 confirmations yet. If enough 
                // confirmations are available, then forwards the deposit to FundsValidationService
                PollConfirmations(coinService);
                
                // Get the latest of the client's transactions list
                blockHash = coinService.GetBestBlockHash();

                // Update the blockHash in the dictionary
                _serviceToBlockHashDictionary[coinService] = blockHash;
            }
        }

        /// <summary>
        /// Check for new Transactions on any of the addresses that this Exchange provided
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        private void CheckNewTransactions(string currency, List<TransactionSinceBlock> newTransactions)
        {
            // Get all the deposit addresses to get the AccountId of the user who created this address. These
            // addresses are created whenever a new address is requested from the bitcoin network
            List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
            for (int i = 0; i < newTransactions.Count; i++)
            {
                if (newTransactions[i].Category == "receive")
                {
                    foreach (var depositAddress in allDepositAddresses)
                    {
                        // If any of the new transactions' addresses matches any deposit addresses
                        if (depositAddress.BitcoinAddress.Value == newTransactions[i].Address)
                        {
                            // Make sure this address hasn't been used earlier
                            if (depositAddress.Status != AddressStatus.Used &&
                                depositAddress.Status != AddressStatus.Expired)
                            {
                                // Create a new deposit for this transaction
                                ValidateDeposit(currency, newTransactions[i].Address, newTransactions[i].Amount,
                                                depositAddress.AccountId.Value, newTransactions[i].TxId);

                                // Change the status of the deposit address to Used and save
                                depositAddress.StatusUsed();
                                _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                                // If any object is waiting for the deposit event, raise it.
                                if (DepositArrived != null)
                                {
                                    DepositArrived();
                                }
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Poll Confirmations
        /// </summary>
        public void PollConfirmations(ICoinService coinService)
        {
            // get all deposits
            List<Deposit> allDeposits = _depositRepository.GetAllDeposits();            
            foreach (var deposit in allDeposits)
            {
                // If enough confirmations are not available for the current deposit yet
                if (deposit.Confirmations < 7)
                {
                    // Get the information about this transation
                    GetTransactionResponse getTransactionResponse = coinService.GetTransaction(deposit.TransactionId.Value);
                    // Set the confirmations
                    deposit.SetConfirmations(getTransactionResponse.Confirmations);
                    // Save in database
                    _fundsPersistenceRepository.SaveOrUpdate(deposit);

                    // If enough confirmations are available, forward to the FundsValidationService to proceed with the 
                    // ledger transation of this deposit
                    if (deposit.Confirmations >= 7)
                    {
                        _fundsValidationService.DepositConfirmed(deposit);
                    }                    
                }
            }
        }

        /// <summary>
        /// Creates a new Deposit instance if not already present, or updates the deposit confirmations otherwise and 
        /// sends to FundsValidationService for further validation
        /// </summary>
        public void ValidateDeposit(string currency, string address, decimal amount, int accountId, string transactionId)
        {
            Deposit deposit = new Deposit(new Currency(currency, true), _depositIdGeneratorService.GenerateId(),
                    DateTime.Now, DepositType.Default, amount, 0, TransactionStatus.Pending, new AccountId(accountId),
                    new TransactionId(transactionId), new BitcoinAddress(address));
            _fundsPersistenceRepository.SaveOrUpdate(deposit);
        }

        public event Action DepositArrived;

        /// <summary>
        /// Gets a new address from the client for either a Deposit or Withdrawal
        /// </summary>
        /// <returns></returns>
        public string CreateNewAddress(string currency)
        {
            string newAddress = _bitcoinService.GetNewAddress();
            return newAddress;
        }

        /// <summary>
        /// Commits the withdraw and forwards to the bitcoin network.
        /// </summary>
        /// <param name="bitcoinAddress"> </param>
        /// <param name="amount"> </param>
        /// <returns></returns>
        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            string transactionId = _bitcoinService.SendToAddress(bitcoinAddress, amount);
            return transactionId;
        }

        /// <summary>
        /// Checks the balance for the wallet
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public decimal CheckBalance(string currency)
        {
            return _bitcoinService.GetBalance();
        }

        #region Properties

        /// <summary>
        /// Interval for polling
        /// </summary>
        public double PollingInterval
        {
            get { return _timer.Interval; }
        }

        #endregion Properties
    }
}
