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
        private Timer _pollConfirmationsTimer = null;
        private string _blockHash = null;

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
            // Initializing Timer for new transactions
            _newTransactrionsTimer = new Timer(Convert.ToDouble(ConfigurationManager.AppSettings.Get("LtcNewTransactionsTimer")));
            _newTransactrionsTimer.Elapsed += NewTransactionsElapsed;
            _newTransactrionsTimer.Enabled = true;

            // Initializing Timer for polling Confirmations of pending transactions
            _pollConfirmationsTimer = new Timer(Convert.ToDouble(ConfigurationManager.AppSettings.Get("LtcPollingIntervalTimer")));
            _pollConfirmationsTimer.Elapsed += PollIntervalElapsed;
            _pollConfirmationsTimer.Enabled = true;
        }

        /// <summary>
        /// When the interval for new 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Transaction]
        private void NewTransactionsElapsed(object sender, ElapsedEventArgs e)
        {
            CheckNewTransactions();
        }

        /// <summary>
        /// When the interval for the Poll confirmations Timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Transaction]
        private void PollIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            PollConfirmations();
        }

        /// <summary>
        /// Looks for new transactions
        /// </summary>
        public void CheckNewTransactions()
        {
            if (string.IsNullOrEmpty(_blockHash))
            {
                _blockHash = _litecoinService.GetBestBlockHash();
            }

            // Get the list of blocks from the point we last time checked for blocks, by providing the last blockHash
            // recorded last time 
            List<TransactionSinceBlock> newTransactions = _litecoinService.ListSinceBlock(_blockHash, 1).Transactions;

            List<Tuple<string, string, decimal, string>> transactionAddressList = null;
            if (newTransactions != null && newTransactions.Any())
            {
                transactionAddressList = new List<Tuple<string, string, decimal, string>>();
                foreach (TransactionSinceBlock transactionSinceBlock in newTransactions)
                {
                    if (transactionSinceBlock.Category == "receive")
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
                            DepositArrived(CurrencyConstants.Ltc, transactionAddressList);
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
            List<Tuple<string, int>> depositsConfirmed = new List<Tuple<string, int>>();
            for (int i = 0; i < _pendingTransactions.Count; i++)
            {
                // Get the number of confirmations of each pending confirmation (deposit)
                GetTransactionResponse getTransactionResponse = _litecoinService.GetTransaction(_pendingTransactions[i].Item1);

                // If the current confirmation count in the block chain is greater than the one we have stored, update 
                // confirmations
                if (getTransactionResponse.Confirmations > _pendingTransactions[i].Item2)
                {
                    _pendingTransactions[i] = new Tuple<string, int>(_pendingTransactions[i].Item1, 
                                                                     getTransactionResponse.Confirmations);
                    if (DepositConfirmed != null)
                    {
                        // Raise the event and sned the TransacitonID and the no. of confirmation respectively
                        DepositConfirmed(_pendingTransactions[i].Item1, getTransactionResponse.Confirmations);
                    }
                    // If the no of confirmations is >= 7, add to the list of confirmed deposits to be deleted outside this 
                    // loop
                    if (_pendingTransactions[i].Item2 >= 7)
                    {
                        // Add the confirmed trnasactions into the list of confirmed deposits
                        depositsConfirmed.Add(_pendingTransactions[i]);
                    }
                }
            }

            // Remove the confirmed deposits from the list of _pendingTransactions. Do it here as we cannot do that when iterating
            // the _pensingTransactions list above
            if (depositsConfirmed.Any())
            {
                foreach (Tuple<string, int> deposit in depositsConfirmed)
                {
                    _pendingTransactions.Remove(deposit);
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

        public double PollingInterval { get; private set; }
    }
}
