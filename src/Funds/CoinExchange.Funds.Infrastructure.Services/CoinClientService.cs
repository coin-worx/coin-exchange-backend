using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
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
        //private List<string> _addressList; 
        private Timer _timer = null;
        //private int _transactionsCount = 0;
        //private List<TransactionSinceBlock> _transactionSinceBlock = null;
        private Dictionary<ICoinService, List<TransactionSinceBlock>> _serviceToTransactionsDictionary = new Dictionary<ICoinService, List<TransactionSinceBlock>>();
        private Dictionary<ICoinService, List<string>> _serviceToAddressList = new Dictionary<ICoinService, List<string>>(); 
        private Dictionary<ICoinService, string> _serviceToBlockHashDictionary = new Dictionary<ICoinService, string>();
        private int _depositPollingMinutes = 10;
        
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
        /// Populates coin services against the list of transaction they contain
        /// </summary>
        public void PopulateServicesAgainstTransacations()
        {
            foreach (var currency in _currencies)
            {
                
                ICoinService coinService = GetSpecificCoinService(currency);
                List<TransactionSinceBlock> transactions = coinService.ListSinceBlock().Transactions;

                _serviceToTransactionsDictionary.Add(coinService, transactions);
                _serviceToTransactionsDictionary.Add(coinService, new List<TransactionSinceBlock>());
                _serviceToBlockHashDictionary.Add(coinService, transactions.Last().BlockHash);
            }
        }

        /// <summary>
        /// Starts the Timer
        /// </summary>
        public void StartTimer()
        {
            _timer = new Timer(20000);
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
                List<TransactionSinceBlock> transactions = coinService.ListSinceBlock().Transactions;
                _serviceToBlockHashDictionary.Add(coinService, coinService.GetBestBlockHash());                
            }
            else
            {
                // Get the list of blocks from the point we last time checked for blocks, by providing the last blockHash
                // recorded last time 
                ListSinceBlockResponse blocksList = coinService.ListSinceBlock(blockHash, 1);

                _timer.Stop();
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

            /*if (_serviceToTransactionsDictionary.TryGetValue(coinService, out oldTransactions))
            {
                List<TransactionSinceBlock> newTransactions = coinService.ListSinceBlock().Transactions;
                
                if (newTransactions.Count > oldTransactions.Count)
                {
                    // Start from the last transaction of the older transaction list, end at the latest of the new 
                    // transaction list
                    for (int i = oldTransactions.Count; i <= newTransactions.Count - 1; i++)
                    {
                        List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
                        foreach (var allDepositAddress in allDepositAddresses)
                        {
                            if (allDepositAddress.BitcoinAddress.Value == newTransactions[i].Address)
                            {
                                ValidateDeposit(currency, newTransactions[i].Address, newTransactions[i].Amount,
                                    allDepositAddress.AccountId.Value, newTransactions[i].TxId);
                            }
                        }
                    }
                }
            }*/
        }

        /// <summary>
        /// Check for new Transactions on any of the addresses that this Exchange provided
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        private void CheckNewTransactions(string currency, List<TransactionSinceBlock> newTransactions)
        {
            // Create a list that will mark multiple transactions with the same address, as one transaction comes with two
            // entries in the transaction list
            List<string> processedAddresses = new List<string>();
            for (int i = 0; i < newTransactions.Count - 1; i++)
            {
                // If this new address has just been processed, go to the next iteration
                if (processedAddresses.Contains(newTransactions[i].Address))
                {
                    continue;
                }
                // Get all the deposit addresses to get the AccountId of the user who created this address. These
                // addresses are created whenever a new address is requested from the bitcoin network
                List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
                foreach (var depositAddress in allDepositAddresses)
                {
                    // Make sure this address hasn't been used earlier
                    if (depositAddress.Status != AddressStatus.Used && depositAddress.Status != AddressStatus.Expired)
                    {
                        // If any of the new transactions' addresses matches any deposit addresses
                        if (depositAddress.BitcoinAddress.Value == newTransactions[i].Address)
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
                    else
                    {
                        throw new InvalidOperationException(string.Format("Given address has already been used." +
                                           " Address: {0}", depositAddress.BitcoinAddress.Value));
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

            // ToDo: Confirm are we going to restrict one address to one transaction? Or not? Devise a plan accordingly
            /*List<Deposit> depositsByBitcoinAddress = _depositRepository.GetDepositsByBitcoinAddress(new BitcoinAddress(address));
            if (depositsByBitcoinAddress == null || !depositsByBitcoinAddress.Any())
            {
                
            }*/
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
            /*List<string> addressList = null;
            if (_serviceToAddressList.TryGetValue(GetSpecificCoinService(currency), out addressList))
            {
                addressList.Add(newAddress);
                return newAddress;
            }*/
            return null;
        }

        /// <summary>
        /// Commits the withdraw and forwards to the bitcoin network.
        /// </summary>
        /// <param name="withdraw"></param>
        /// <returns></returns>
        public bool CommitWithdraw(Withdraw withdraw)
        {
            string transactionId = _bitcoinService.SendToAddress(withdraw.BitcoinAddress.Value, withdraw.Amount);
            if (!string.IsNullOrEmpty(transactionId))
            {
                withdraw.SetTransactionId(transactionId);
                return _fundsValidationService.WithdrawalExecuted(withdraw);
            }
            return true;
        }

        /// <summary>
        /// Callback method, invoked when a deposit is made to an address that was provided by the Exchange
        /// </summary>
        /// <param name="address"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool DepositMade(string address, string currency, decimal amount)
        {
            //ToDo: Solutions to getting notified of a deposit made to an address we provided:
            // 1. Use walletNotify, specify a script, or directly provide a URL to the webservice(using curl or wget),
            // and call this method by providing the Transaction ID though port and application services.
            // Or, specify a shell script in the walletnotify, and then provide the URL and port and send it using curl

            // Example, ./bitcoind  -blocknotify="echo '%s' | nc 127.0.0.1 4001" 
            // -walletnotify="echo '%s' | nc 127.0.0.1 4002" 
            // -alertnotify="echo '%s' | nc 127.0.0.1 4003" 
            // -daemon
            // netcat (nc) will write %s to a tcp port, %s is transaction id or block id. with those you can go ahead 
            // and query the api for details. This guy has put some code here about it: 
            // https://github.com/johannbarbie/BitcoindClient4J

            // 2. Or, fund the last block, call 'ListSinceBlock' method in the BitcoinService after every 10 minutes,
            // and see if new blocks are added after that block and get the TxId and get info about it using
            // BitcoinService.GetTransaction(TxId) method

            // ToDo: Confirm if the Transaction ID also comes in when a Deposit is made. 
            // Answer: yes it does
            // 
            // After getting the deposit, 
            // 1. Increment the Confirmations
            // 2. Set the Transaction ID
            // 3. Call the FundsValidationService, which will check if enough confirmations are
            // available for this deposit(7 confirmations), and proceed accordingly

            //CheckSingleCurrencyDeposit(GetSpecificCoinService(currency), currency);
            return false;
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
    }
}
