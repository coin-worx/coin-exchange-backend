using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        /// Dictionary to add CoinService against the pending Transaction's TxId and No. of Confirmations
        /// List: Item1 = TxId, Item2 = No. Of Confirmations
        /// </summary>
        private Dictionary<ICoinService, List<Tuple<string, int>>> _coinServiceToPendingDepositsDict = 
            new Dictionary<ICoinService, List<Tuple<string, int>>>();

        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

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
                CheckNewTransactions(coinService, currency, blocksList.Transactions);

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
        /// Raises event in case new transactions arrive. 
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="coinService"> </param>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        private void CheckNewTransactions(ICoinService coinService, string currency, List<TransactionSinceBlock> newTransactions)
        {
            List<Tuple<string, string, decimal, string>> transactionAddressList = null;
            if (newTransactions != null && newTransactions.Any())
            {
                transactionAddressList = new List<Tuple<string, string, decimal, string>>();
                foreach (TransactionSinceBlock transactionSinceBlock in newTransactions)
                {
                    if (transactionSinceBlock.Category == "receive")
                    {
                        transactionAddressList.Add(
                            new Tuple<string, string, decimal, string>(transactionSinceBlock.Address,
                                                                       transactionSinceBlock.TxId,
                                                                       transactionSinceBlock.Amount,
                                                                       transactionSinceBlock.Category));

                        List<Tuple<string, int>> pendingDeposits = null;
                        if (_coinServiceToPendingDepositsDict.TryGetValue(coinService, out pendingDeposits))
                        {
                            bool txIdExists = false;
                            foreach (Tuple<string, int> pendingDeposit in pendingDeposits)
                            {
                                // Make sure that one transaciton ID does not get saved twice
                                if (pendingDeposit.Item1 == transactionSinceBlock.TxId)
                                {
                                    txIdExists = true;
                                }
                            }
                            if (txIdExists)
                            {
                                continue;
                            }
                            // If the ICoinService exists already, add to the list of pending confirmations(Deposits)
                            pendingDeposits.Add(new Tuple<string, int>(transactionSinceBlock.TxId, 0));
                            _coinServiceToPendingDepositsDict[coinService] = pendingDeposits;
                            if (DepositArrived != null)
                            {
                                DepositArrived(currency, transactionAddressList);
                            }
                        }
                        else
                        {
                            // If the ICoinService does not exist, add it and a new list of Pending Confirmations(Deposist)
                            pendingDeposits = new List<Tuple<string, int>>();
                            pendingDeposits.Add(new Tuple<string, int>(transactionSinceBlock.TxId, 0));
                            _coinServiceToPendingDepositsDict.Add(coinService, pendingDeposits);
                            if (DepositArrived != null)
                            {
                                DepositArrived(currency, transactionAddressList);
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
            List<Tuple<string, int>> depositsConfirmed = null;
            List<Tuple<string, int>> pendingDeposits = null;
            if (_coinServiceToPendingDepositsDict.TryGetValue(coinService, out pendingDeposits))
            {
                depositsConfirmed = new List<Tuple<string, int>>();
                for (int i = 0; i < pendingDeposits.Count; i++)
                {
                    // Get the number of confirmations of each pending confirmation (deposit)
                    GetTransactionResponse getTransactionResponse = coinService.GetTransaction(pendingDeposits[i].Item1);
                    pendingDeposits[i] = new Tuple<string, int>(pendingDeposits[i].Item1, getTransactionResponse.Confirmations);

                    // If the no of confirmations is >= 7, send this information to the DepositApplicationService
                    if (pendingDeposits[i].Item2 >= 7)
                    {
                        if (DepositConfirmed != null)
                        {
                            // Raise the event and sned the TransacitonID and the no. of confirmation respectively
                            DepositConfirmed(pendingDeposits[i].Item1, getTransactionResponse.Confirmations);
                        }
                        // Add the confirmed trnasactions into the list of confirmed deposits
                        depositsConfirmed.Add(pendingDeposits[i]);
                    }
                }

                // Remove the confirmed deposits from the list of pending deposits. Do it here as we cannot do that when iterating
                // the pendingDeposits list above
                if (depositsConfirmed.Any())
                {
                    foreach (Tuple<string, int> deposit in depositsConfirmed)
                    {
                        pendingDeposits.Remove(deposit);
                    }
                }
            }
        }

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
