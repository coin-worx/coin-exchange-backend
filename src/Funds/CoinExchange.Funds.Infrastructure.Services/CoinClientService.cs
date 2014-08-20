using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BitcoinLib.Responses;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Bitcoin;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
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
        private List<string> _currencies; 
        //private List<string> _addressList; 
        private Timer _timer = new Timer();
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
            IDepositAddressRepository depositAddressRepository, IDepositIdGeneratorService depositIdGeneratorService)
        {
            _fundsValidationService = fundsValidationService;
            _depositRepository = depositRepository;
            _depositAddressRepository = depositAddressRepository;
            _depositIdGeneratorService = depositIdGeneratorService;

            PopulateCurrencies();
            PopulateServices();
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
            _bitcoinService = new BitcoinService();
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
            _timer.Elapsed += CheckNewDeposits;
            _timer.Interval = new TimeSpan(0, 1, 0).Minutes;
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
        public void CheckNewDeposits(object sender, ElapsedEventArgs elapsedEventArgs)
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
            if (_serviceToBlockHashDictionary.TryGetValue(coinService, out blockHash))
            {
                List<TransactionSinceBlock> newTransactions = coinService.ListSinceBlock(blockHash).Transactions;
                for (int i = 0; i < newTransactions.Count - 1; i++)
                {
                    List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
                    foreach (var allDepositAddress in allDepositAddresses)
                    {
                        if (allDepositAddress.BitcoinAddress.Value == newTransactions[i].Address)
                        {
                            ValidateDeposit(currency, newTransactions[i].Address, newTransactions[i].Amount,
                                            allDepositAddress.AccountId.Value, newTransactions[i].TxId);
                            if (DepositArrived != null)
                            {
                                DepositArrived();
                            }
                        }
                    }
                }
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
        /// Creates a new Deposit instance if not already present, or updates the deposit confirmations otherwise and 
        /// sends to FundsValidationService for further validation
        /// </summary>
        public void ValidateDeposit(string currency, string address, decimal amount, int accountId, string transactionId)
        {
            
            // ToDo: Confirm are we going to restrict one address to one transaction? Or not? Devise a plan accordingly
            List<Deposit> depositsByBitcoinAddress = _depositRepository.GetDepositsByBitcoinAddress(new BitcoinAddress(address));
            if (depositsByBitcoinAddress == null || !depositsByBitcoinAddress.Any())
            {
                Deposit deposit = new Deposit(new Currency(currency, true), _depositIdGeneratorService.GenerateId(),
                    DateTime.Now, DepositType.Default, amount, 0, TransactionStatus.Pending, new AccountId(accountId), 
                    new TransactionId(transactionId), new BitcoinAddress(address));
                _fundsValidationService.DepositConfirmed(deposit);
            }
        }

        public event Action DepositArrived;

        /// <summary>
        /// Gets a new address from the client for either a Deposit or Withdrawal
        /// </summary>
        /// <returns></returns>
        public string CreateNewAddress(string currency)
        {
            string newAddress = _bitcoinService.GetNewAddress();
            List<string> addressList = null;
            if (_serviceToAddressList.TryGetValue(GetSpecificCoinService(currency), out addressList))
            {
                addressList.Add(newAddress);
                return newAddress;
            }
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
