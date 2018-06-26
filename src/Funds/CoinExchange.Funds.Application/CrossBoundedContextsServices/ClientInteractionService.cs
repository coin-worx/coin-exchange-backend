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
using System.Management.Instrumentation;
using System.Timers;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service to store and submit withdrawals and keeping track of the time interval after which withdrawals are passed to the 
    /// CoinClientService
    /// </summary>
    public class ClientInteractionService : IClientInteractionService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IWithdrawRepository _withdrawRepository;        
        private ICoinClientService _bitcoinClientService;
        private ICoinClientService _litecoinClientService;

        private Dictionary<Timer, Withdraw> _withdrawTimersDictionary = new Dictionary<Timer, Withdraw>();

        private double _withdrawSubmissioInterval = 0;
            

        public event Action<Withdraw> WithdrawExecuted;
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ClientInteractionService(IFundsPersistenceRepository fundsPersistenceRepository, IWithdrawRepository withdrawRepository, 
            ICoinClientService bitcoinClientService, ICoinClientService litecoinClientService)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _withdrawRepository = withdrawRepository;
            _bitcoinClientService = bitcoinClientService;
            _litecoinClientService = litecoinClientService;

            _withdrawSubmissioInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("WithdrawSubmitInterval"));
            Log.Debug(string.Format("Withdraw submission interval = {0}", _withdrawSubmissioInterval));

            HookEventHandlers();
        }

        /// <summary>
        /// Hook the event handlers with events of the implementations of CoinClientService
        /// </summary>
        private void HookEventHandlers()
        {
            _bitcoinClientService.DepositArrived -= this.OnDepositArrival;
            _bitcoinClientService.DepositConfirmed -= this.OnDepositConfirmed;
            _bitcoinClientService.DepositArrived += this.OnDepositArrival;
            _bitcoinClientService.DepositConfirmed += this.OnDepositConfirmed;

            _litecoinClientService.DepositArrived -= this.OnDepositArrival;
            _litecoinClientService.DepositConfirmed -= this.OnDepositConfirmed;
            _litecoinClientService.DepositArrived += this.OnDepositArrival;
            _litecoinClientService.DepositConfirmed += this.OnDepositConfirmed;
        }

        /// <summary>
        /// Selects the CoinClient Service to which to forward the event
        /// </summary>
        private ICoinClientService SelectCoinService(string currency)
        {
            switch (currency)
            {
                case CurrencyConstants.Btc:
                    return _bitcoinClientService;
                case CurrencyConstants.Ltc:
                    return _litecoinClientService;
            }
            return null;
        }

        /// <summary>
        /// Saveas a withdraw to database and starts teh timer after which the withrawal is submitted to CoinClientService
        /// </summary>
        /// <returns></returns>
        public bool CommitWithdraw(Withdraw withdraw)
        {
            if (withdraw != null)
            {
                Log.Debug(string.Format("New Withdraw  request received, starting withdraw timer: Account ID = {0}, Currency = {1}", withdraw.AccountId.Value, withdraw.Currency.Name));
                Timer timer = new Timer(_withdrawSubmissioInterval);
                timer.Elapsed += OnTimerElapsed;
                //timer.Enabled = true;
                timer.Start();
                _withdrawTimersDictionary.Add(timer, withdraw);
                return true;
            }
            throw new NullReferenceException(string.Format("Withdraw received to ClientInteractionService is null"));
        }

        /// <summary>
        /// Cancels the withdraw with the given WithdrawId
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        public bool CancelWithdraw(string withdrawId)
        {
            Log.Debug(string.Format("Cancel Withdraw request received: Withdraw ID = {0}", withdrawId));
            Timer timerKey = null;
            foreach (KeyValuePair<Timer, Withdraw> keyValuePair in _withdrawTimersDictionary)
            {
                if (keyValuePair.Value.WithdrawId == withdrawId)
                {
                    timerKey = keyValuePair.Key;
                }
            }
            if (timerKey != null)
            {
                Log.Debug(string.Format("Withdraw cancelled: Withdraw ID = {0}", withdrawId));
                timerKey.Stop();
                timerKey.Dispose();
                _withdrawTimersDictionary.Remove(timerKey);
                return true;
            }
            throw new InstanceNotFoundException(string.Format("No withdraw found in memory for Withdraw ID: {0}",
                    withdrawId));
        }

        /// <summary>
        /// Sends request to generate a new address to the specific CoinClientService for the given currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public string GenerateNewAddress(string currency)
        {
            ICoinClientService coinService = SelectCoinService(currency);
            if (coinService != null)
            {
                // Select the corresponding coin client service and return a response after requesting
                return coinService.CreateNewAddress();
            }
            throw new InstanceNotFoundException(string.Format("No Client Service found for Currency: {0}", currency));
        }

        /// <summary>
        /// Handler when the timer has been elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Log.Debug(string.Format("Withdraw Timer Elapsed."));
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                Log.Debug(string.Format("Withdraw Timer Stopped"));
                Withdraw withdraw = null;
                _withdrawTimersDictionary.TryGetValue(timer, out withdraw);
                if (withdraw != null)
                {
                    Log.Debug(string.Format("Submitting Withdraw to network: Account ID = {0}, Currency = {1}", withdraw.AccountId.Value, withdraw.Currency.Name));
                    // Selecct the coin client service to which to send the withdraw
                    ICoinClientService coinClientService = SelectCoinService(withdraw.Currency.Name);
                    string transactionId = coinClientService.CommitWithdraw(
                                                             withdraw.BitcoinAddress.Value, withdraw.Amount);
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        Log.Error(string.Format("Withdraw could not be submitted to network: Account ID = {0}, Currency = {1}", withdraw.AccountId.Value, withdraw.Currency.Name));
                    }
                    else
                    {
                        Log.Debug(string.Format("Withdraw submitted succcessfully to network: Account ID = {0}, Currency = {1}", withdraw.AccountId.Value, withdraw.Currency.Name));
                        // Set transaction Id recevied from the network
                        withdraw.SetTransactionId(transactionId);
                        /*
                        // Set status as confirmed
                        withdraw.StatusConfirmed();
                        // Save the withdraw
                        _fundsPersistenceRepository.SaveOrUpdate(withdraw);*/
                        _withdrawTimersDictionary.Remove(timer);
                        if (WithdrawExecuted != null)
                        {
                            WithdrawExecuted(withdraw);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the event raised by CoinClientServices when a deposit is confirmed
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        private void OnDepositConfirmed(string transactionId, int confirmations)
        {
            if (DepositConfirmed != null)
            {
                DepositConfirmed(transactionId, confirmations);
            }
        }

        /// <summary>
        /// Handles event raised in result when new transacitons are available. 
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        private void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
        {
            if (DepositArrived != null)
            {
                DepositArrived(currency, newTransactions);
            }
        }

        /// <summary>
        /// Interval after which Withdraw is submitted
        /// </summary>
        public double WithdrawSubmissionInterval
        {
            get { return _withdrawSubmissioInterval; }
        }
    }
}
