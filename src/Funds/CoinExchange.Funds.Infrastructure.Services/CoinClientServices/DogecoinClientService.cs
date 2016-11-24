/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BitcoinLib.Responses;
using BitcoinLib.Services.Coins.Cryptocoin;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Services;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Client service for handling Dogecoin Transactions
    /// </summary>
    public class DogecoinClientService : ICoinClientService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _useTestNet = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("DogeUseTestNet"));
        private readonly string _daemonUrl = ConfigurationManager.AppSettings.Get("Dogecoin_DaemonUrl");
        private readonly string _username = ConfigurationManager.AppSettings.Get("Dogecoin_RpcUsername");
        private readonly string _password = ConfigurationManager.AppSettings.Get("Dogecoin_RpcPassword");
        private readonly string _walletPassword = ConfigurationManager.AppSettings.Get("Dogecoin_WalletPassword");

        private ICryptocoinService _dogecoinClientService;
        private Timer _newTransactrionsTimer = null;
        private string _blockHash = null;
        private double _newTransactionsInterval = 0;
        private double _pollInterval = 0;
        private DateTime _pollIntervalDateTime = DateTime.MinValue;
        private string _currency = CurrencyConstants.Doge;

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
        public DogecoinClientService()
        {
            if (_useTestNet)
            {
                _daemonUrl = ConfigurationManager.AppSettings.Get("Dogecoin_DaemonUrl_Testnet");
            }
            else
            {
                _daemonUrl = ConfigurationManager.AppSettings.Get("Dogecoin_DaemonUrl");
            }
            _dogecoinClientService = new CryptocoinService(_daemonUrl, _username, _password, _walletPassword);

            StartTimer();
            Log.Debug(string.Format("Dogecoin Timer Started"));
        }

        /// <summary>
        /// Starts the Timer
        /// </summary>
        private void StartTimer()
        {
            _newTransactionsInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("DogeNewTransactionsTimer"));
            _pollInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("DogePollingIntervalTimer"));

            Log.Debug(string.Format("Dogecoin New Transaction Timer Interval = {0}", _newTransactionsInterval));
            Log.Debug(string.Format("Dogecoin Poll Confirmations Timer Interval = {0}", _pollInterval));

            // Initializing Timer for new transactions
            _newTransactrionsTimer = new Timer(_newTransactionsInterval);
            _newTransactrionsTimer.Elapsed += NewTransactionsElapsed;
            _newTransactrionsTimer.Start();
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
                _blockHash = _dogecoinClientService.GetBestBlockHash();
                return null;
            }
            else
            {
                // Get the list of blocks from the point we last time checked for blocks, by providing the last blockHash
                // recorded last time 
                List<TransactionSinceBlock> transactionSinceBlocks = _dogecoinClientService.ListSinceBlock(_blockHash, 1).Transactions;
                _blockHash = _dogecoinClientService.GetBestBlockHash();
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
                        Log.Debug(string.Format("New Dogecoin transaction received from network: " +
                                                "Address = {0}, Amount = {1}, Transaction ID = {2}",
                            transactionSinceBlock.Address, transactionSinceBlock.Amount, transactionSinceBlock.TxId));
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
                GetTransactionResponse getTransactionResponse = _dogecoinClientService.GetTransaction(_pendingTransactions[i].Item1);

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
                    Log.Debug(string.Format("Verified Deposit removed from list of pending deposits: Transaction ID = {0}",
                        deposit.Item1));
                }
            }
        }

        /// <summary>
        /// Adds a new confirmation if it is greater than the last number of confirmations
        /// </summary>
        /// <param name="getTransactionResponse"></param>
        /// <param name="transactionIndex"></param>
        /// <param name="depositsConfirmed"></param>
        private void AddNewConfirmation(GetTransactionResponse getTransactionResponse, int transactionIndex,
            List<Tuple<string, int>> depositsConfirmed)
        {
            // If the current confirmation count in the block chain is greater than the one we have stored, update 
            // confirmations
            if (getTransactionResponse.Confirmations > _pendingTransactions[transactionIndex].Item2)
            {
                Log.Debug(string.Format("Dogecoin Confirmation received from network: Transaction ID = {0}, Confirmations = {1}",
                    _pendingTransactions[transactionIndex].Item1, getTransactionResponse.Confirmations));
                _pendingTransactions[transactionIndex] = new Tuple<string, int>(_pendingTransactions[transactionIndex].Item1,
                                                                 getTransactionResponse.Confirmations);
                try
                {
                    if (DepositConfirmed != null)
                    {
                        // Raise the event and sned the TransacitonID and the no. of confirmation respectively
                        DepositConfirmed(_pendingTransactions[transactionIndex].Item1, getTransactionResponse.Confirmations);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
                finally
                {
                    // If the no of confirmations is >= 7, add to the list of confirmed deposits to be deleted outside this 
                    // loop
                    if (getTransactionResponse.Confirmations > 6)
                    {
                        // Add the confirmed trnasactions into the list of confirmed deposits
                        depositsConfirmed.Add(_pendingTransactions[transactionIndex]);
                        Log.Debug(string.Format("Verified deposit added to confirmed deposits list: Transaction ID = {0}",
                            _pendingTransactions[transactionIndex].Item1));
                    }
                }
            }
        }

        /// <summary>
        /// Create New Address using the daemon
        /// </summary>
        /// <returns></returns>
        public string CreateNewAddress()
        {
            return _dogecoinClientService.GetNewAddress();
        }

        /// <summary>
        /// Commit the withdraw to the Dogecoin Network
        /// </summary>
        /// <param name="dogecoinAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string CommitWithdraw(string dogecoinAddress, decimal amount)
        {
            return _dogecoinClientService.SendToAddress(dogecoinAddress, amount);
        }

        /// <summary>
        /// Check the balance for LTC
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public decimal CheckBalance(string currency)
        {
            return _dogecoinClientService.GetBalance();
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
