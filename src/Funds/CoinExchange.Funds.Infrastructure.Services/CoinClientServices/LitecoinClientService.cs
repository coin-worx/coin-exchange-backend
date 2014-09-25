using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using BitcoinLib.Responses;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Litecoin;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Services;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Litecoin Client Service
    /// </summary>
    public class LitecoinClientService : ICoinClientService
    {
        private ICoinService _litecoinService = null;
        private Timer _newTransactrionsTimer = null;
        private string _blockHash = null;
        private double _newTransactionsInterval = 0;
        private double _pollInterval = 0;
        private DateTime _pollIntervalDateTime = DateTime.MinValue;
        private string _currency = CurrencyConstants.Ltc;

        /// <summary>
        /// List of transactions with confirmations less than 7
        /// Item1 = Transaction ID, Item2 = No. of Confirmations
        /// </summary>
        private List<Tuple<string, int>> _pendingTransactions = new List<Tuple<string, int>>();
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LitecoinClientService()
        {
            bool useBitcoinTestNet = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("LtcUseTestNet"));
            _litecoinService = new LitecoinService(useTestnet: useBitcoinTestNet);

            StartTimer();
        }

        /// <summary>
        /// Starts the Timer
        /// </summary>
        private void StartTimer()
        {
            _newTransactionsInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("LtcNewTransactionsTimer"));
            _pollInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("LtcPollingIntervalTimer"));     

            // Initializing Timer for new transactions
            _newTransactrionsTimer = new Timer(_newTransactionsInterval);
            _newTransactrionsTimer.Elapsed += NewTransactionsElapsed;
            _newTransactrionsTimer.Enabled = true;
        }

        /// <summary>
        /// When the interval for new 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Transaction]
        private void NewTransactionsElapsed(object sender, ElapsedEventArgs e)
        {
            List<TransactionSinceBlock> newTransactions = GetTransactionsSinceBlock();

            // Check if there are new transactions that we dont have a track of
            CheckNewTransactions(newTransactions);

            // We will not check confirmations with the timer interval of checking new transactions, so we will make sure that we
            // poll for confirmations after the specified PollConfrmations Interval. Because if we have separate timers, then
            // the integrity of the _newTransactions list is doubtful, and having two separate timers and/or list can result in
            // overheads and also disregard of data integrity

            // If _pollIntervalDateTime has not yet been assigned
            if (_pollIntervalDateTime == DateTime.MinValue)
            {
                _pollIntervalDateTime = DateTime.Now.AddMilliseconds(_pollInterval);
            }
            else
            {
                // If Poll Time has been assigned, check if it has been elapsed, only after that will we check for confirmations
                if (_pollIntervalDateTime < DateTime.Now)
                {
                    // Add the time again after which confirmations will be checked
                    _pollIntervalDateTime = DateTime.Now.AddMilliseconds(_pollInterval);
                    PollConfirmations();
                }
            }
        }

        /// <summary>
        /// Get the Transactions after the specified block
        /// </summary>
        /// <returns></returns>
        private List<TransactionSinceBlock> GetTransactionsSinceBlock()
        {
            if (string.IsNullOrEmpty(_blockHash))
            {
                _blockHash = _litecoinService.GetBestBlockHash();
                return null;
            }
            else
            {
                // Get the list of blocks from the point we last time checked for blocks, by providing the last blockHash
                // recorded last time 
                List<TransactionSinceBlock> transactionSinceBlocks = _litecoinService.ListSinceBlock(_blockHash, 1).Transactions;
                _blockHash = _litecoinService.GetBestBlockHash();
                return transactionSinceBlocks;
            }
        }

        /// <summary>
        /// Look for new transactions coming from the network
        /// </summary>
        public void CheckNewTransactions(List<TransactionSinceBlock> newTransactions)
        {
            List<Tuple<string, string, decimal, string>> transactionAddressList = null;
            if (newTransactions != null && newTransactions.Any())
            {
                transactionAddressList = new List<Tuple<string, string, decimal, string>>();
                foreach (TransactionSinceBlock transactionSinceBlock in newTransactions)
                {
                    if (transactionSinceBlock.Category == BitcoinConstants.ReceiveCategory)
                    {
                        bool txIdExists = false;
                        foreach (Tuple<string, int> pendingDeposit in _pendingTransactions)
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
                        // Add the new transaction to the list of pending transactions
                        _pendingTransactions.Add(new Tuple<string, int>(transactionSinceBlock.TxId, 0));
                        // Send the address, TransactionId, amount and category of the new transaction to the event handlers
                        transactionAddressList.Add(new Tuple<string, string, decimal, string>(transactionSinceBlock.Address,
                                                                                              transactionSinceBlock.TxId,
                                                                                              transactionSinceBlock.Amount,
                                                                                              transactionSinceBlock.Category));
                        // Raise the event to let event handlers know
                        if (DepositArrived != null)
                        {
                            DepositArrived(_currency, transactionAddressList);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Polls confirmations for pending transactions
        /// </summary>
        public void PollConfirmations()
        {
            // List to temporarily save the transactions whose confirmations have reached the count of 7, these confirmations
            // are going to be deleted from the _pendingTransactions List
            List<Tuple<string, int>> confirmedDeposits = new List<Tuple<string, int>>();
            for (int i = 0; i < _pendingTransactions.Count; i++)
            {
                // Get the number of confirmations of each pending confirmation (deposit)
                GetTransactionResponse getTransactionResponse = _litecoinService.GetTransaction(_pendingTransactions[i].Item1);

                // Add new confirmations and raise events
                AddNewConfirmation(getTransactionResponse, i, confirmedDeposits);
            }

            // Remove the confirmed deposits from the list of _pendingTransactions. Do it here as we cannot do that when iterating
            // the _pensingTransactions list above
            if (confirmedDeposits.Any())
            {
                foreach (Tuple<string, int> deposit in confirmedDeposits)
                {
                    _pendingTransactions.Remove(deposit);
                }
            }
        }

        private void AddNewConfirmation(GetTransactionResponse getTransactionResponse, int transactionIndex, 
            List<Tuple<string,int>> depositsConfirmed)
        {
            // If the current confirmation count in the block chain is greater than the one we have stored, update 
            // confirmations
            if (getTransactionResponse.Confirmations > _pendingTransactions[transactionIndex].Item2)
            {
                _pendingTransactions[transactionIndex] = new Tuple<string, int>(_pendingTransactions[transactionIndex].Item1,
                                                                 getTransactionResponse.Confirmations);
                if (DepositConfirmed != null)
                {
                    // Raise the event and sned the TransacitonID and the no. of confirmation respectively
                    DepositConfirmed(_pendingTransactions[transactionIndex].Item1, getTransactionResponse.Confirmations);
                }
                // If the no of confirmations is >= 7, add to the list of confirmed deposits to be deleted outside this 
                // loop
                if (_pendingTransactions[transactionIndex].Item2 >= 7)
                {
                    // Add the confirmed trnasactions into the list of confirmed deposits
                    depositsConfirmed.Add(_pendingTransactions[transactionIndex]);
                }
            }
        }

        /// <summary>
        /// Create New Address using the daemon
        /// </summary>
        /// <returns></returns>
        public string CreateNewAddress()
        {
            return _litecoinService.GetNewAddress();
        }

        /// <summary>
        /// Commit the withdraw to the Litecoin Network
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            return _litecoinService.SendToAddress(bitcoinAddress, amount);
        }

        /// <summary>
        /// Check the balance for LTC
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public decimal CheckBalance(string currency)
        {
            return _litecoinService.GetBalance();
        }

        /// <summary>
        /// Polling INterval
        /// </summary>
        public double PollingInterval { get { return _pollInterval; } }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get { return _currency; } }

        /// <summary>
        /// New Transaction checking itnerval
        /// </summary>
        public double NewTransactionsInterval { get { return _newTransactionsInterval; } }
    }
}
