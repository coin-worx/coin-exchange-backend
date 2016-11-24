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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Application.DepositServices
{
    /// <summary>
    /// Deposit Application Service
    /// </summary>
    public class DepositApplicationService : IDepositApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IFundsValidationService _fundsValidationService;
        private IClientInteractionService _clientInteractionService;
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IDepositAddressRepository _depositAddressRepository;
        // NOTE: The balanceRepository is here for initial testing of Funds, it must be removed once the Exchange is
        // in link with the bank accounts to deposit USD
        private IBalanceRepository _balanceRepository;
        private IDepositRepository _depositRepository;
        private IDepositLimitRepository _depositLimitRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        private DepositApplicationService(IFundsValidationService fundsValidationService, IClientInteractionService clientInteractionService,
            IFundsPersistenceRepository fundsPersistenceRepository, IDepositAddressRepository depositAddressRepository,
            IBalanceRepository balanceRepository, IDepositRepository depositRepository, IDepositLimitRepository depositLimitRepository)
        {
            _fundsValidationService = fundsValidationService;
            _clientInteractionService = clientInteractionService;
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _depositAddressRepository = depositAddressRepository;
            _balanceRepository = balanceRepository;
            _depositRepository = depositRepository;
            _depositLimitRepository = depositLimitRepository;

            _clientInteractionService.DepositArrived += OnDepositArrival;
            _clientInteractionService.DepositConfirmed += OnDepositConfirmed;
        }

        /// <summary>
        /// Event handler for event raised in the result of a new confirmation for a deposit
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        [Transaction]
        public void OnDepositConfirmed(string transactionId, int confirmations)
        {
            Log.Debug(string.Format("Deposit Confirmation Event received: Transaction ID = {0}, Confirmations = {1}",
                transactionId, confirmations));
            // Get all deposits
            Deposit deposit = _depositRepository.GetDepositByTransactionId(new TransactionId(transactionId));

            if (deposit != null)
            {
                // Set the confirmations
                deposit.SetConfirmations(confirmations);
                Log.Debug(string.Format("Confirmations set: Deposit ID = {0}, Confirmations = {1}",
                                             deposit.DepositId, deposit.Confirmations));

                try
                {
                    // If enough confirmations are not available for the current deposit yet
                    if (deposit.Confirmations < 7)
                    {
                        Log.Debug(string.Format("Deposit Confirmations updated: Transaction ID = {0}, Confirmations = {1}",
                                                transactionId, confirmations));
                        // Save in database
                        _fundsPersistenceRepository.SaveOrUpdate(deposit);
                    }
                        // If enough confirmations are available, forward to the FundsValidationService to proceed with the 
                        // ledger transation of this deposit
                    else
                    {
                        Log.Debug(string.Format("7 Confirmations received: Transaction ID = {0}",
                                                transactionId));
                        _fundsValidationService.DepositConfirmed(deposit);
                        Log.Debug(string.Format("Deposit Verified: Transaction ID = {0}, Confirmations = {1}," +
                                                " Currency = {2}, Date Received = {3}, Address = {4}, Account ID = {5}",
                                                transactionId, confirmations, deposit.Currency.Name, deposit.Date,
                                                deposit.BitcoinAddress, deposit.AccountId.Value));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error while adding Confirmations to deposit: " + ex.Message);
                }
            }
            else
            {
                Log.Error(string.Format("Could not finds deposit in database: Transaction ID = {0}", transactionId));
            }
        }

        /// <summary>
        /// Handles event raised in result when new transacitons are available. 
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        [Transaction]
        public void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
        {
            Log.Debug(string.Format("New Deposit Event received: Currency = {0}",
                currency));
            // Get all the deposit addresses to get the AccountId of the user who created this address. These
            // addresses are created whenever a new address is requested from the bitcoin network
            List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
            for (int i = 0; i < newTransactions.Count; i++)
            {
                // If there is already a deposit by the same transacitonId, raise an exception
                Deposit deposit = _depositRepository.GetDepositByTransactionId(new TransactionId(newTransactions[i].Item2));
                if (deposit != null)
                {
                    continue;
                }
                if (newTransactions[i].Item4 == BitcoinConstants.ReceiveCategory)
                {
                    foreach (var depositAddress in allDepositAddresses)
                    {
                        // Confirm if the user has the permissions to perform the current transaction, and deposit is within 
                        // the threshold limits
                        if (_fundsValidationService.IsDepositLegit(depositAddress.AccountId, depositAddress.Currency, 
                            newTransactions[i].Item3))
                        {
                            // If any of the new transactions' addresses matches any deposit addresses
                            if (depositAddress.BitcoinAddress.Value == newTransactions[i].Item1)
                            {
                                Log.Debug(string.Format("New Deposit Instance created: Currency = {0}, Transaction ID = {1}, " +
                                                        "Account ID = {2}, Amount = {3}, Address = {4}",
                                                          currency, newTransactions[i].Item2, depositAddress.AccountId.Value,
                                                          newTransactions[i].Item3, newTransactions[i].Item1));
                                // Create a new deposit for this transaction
                                ValidateDeposit(currency, newTransactions[i].Item1, newTransactions[i].Item3,
                                                depositAddress.AccountId.Value, newTransactions[i].Item2,
                                                TransactionStatus.Pending);

                                // Change the status of the deposit address to Used and save
                                depositAddress.StatusUsed();
                                _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                            }
                        }
                        // If deposit is not within tier levels and threshold limits, freeze the balance. No update to the
                        // balance will ye be made
                        else
                        {
                            Log.Error(string.Format("FATAL ERROR: Tier Level not enough for submitting deposits: Account ID: {0}",
                                depositAddress.AccountId.Value));
                            // Create a deposit that is marked freezed
                            ValidateDeposit(currency, newTransactions[i].Item1, newTransactions[i].Item3,
                                                depositAddress.AccountId.Value, newTransactions[i].Item2,
                                                TransactionStatus.Frozen);
                            
                            // Mark the balance frozen
                            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(currency, true),
                                depositAddress.AccountId);
                            // Update balance if already exists
                            if (balance != null)
                            {
                                balance.FreezeAccount();
                                _fundsPersistenceRepository.SaveOrUpdate(balance);
                            }
                            // Else creaate a new one and freeze
                            else
                            {
                                balance = new Balance(new Currency(currency, true), depositAddress.AccountId);
                                balance.FreezeAccount();
                                _fundsPersistenceRepository.SaveOrUpdate(balance);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new Deposit instance if not already present, or updates the deposit confirmations otherwise and 
        /// sends to FundsValidationService for further validation
        /// </summary>
        public void ValidateDeposit(string currency, string address, decimal amount, int accountId, string transactionId, 
            TransactionStatus transactionStatus)
        {
            Deposit deposit = new Deposit(new Currency(currency, true), Guid.NewGuid().ToString(),
                    DateTime.Now, DepositType.Default, amount, 0, transactionStatus, new AccountId(accountId),
                    new TransactionId(transactionId), new BitcoinAddress(address));
            _fundsPersistenceRepository.SaveOrUpdate(deposit);
        }


        /// <summary>
        /// Get deposits for the given currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"> </param>
        /// <returns></returns>
        public List<DepositRepresentation> GetRecentDeposits(string currency, int accountId)
        {
            List<DepositRepresentation> depositRepresentations = new List<DepositRepresentation>();
            List<Deposit> deposits = _depositRepository.GetDepositByCurrencyAndAccountId(currency, new AccountId(accountId));
            if (deposits != null && deposits.Any())
            {
                foreach (var deposit in deposits)
                {
                    depositRepresentations.Add(new DepositRepresentation(deposit.Currency.Name, "", deposit.DepositId, 
                        deposit.Date, deposit.Amount, deposit.Status.ToString(), (deposit.BitcoinAddress == null) ? null : 
                        deposit.BitcoinAddress.Value, (deposit.TransactionId == null) ? null : deposit.TransactionId.Value));
                }
            }
            else
            {
                Log.Debug(string.Format("No recent deposits found: Currency = {0}, AccountID = {1}",currency, accountId));
            }
            return depositRepresentations;
        }

        /// <summary>
        /// Get a new address from the Bitcoin Client
        /// </summary>
        /// <param name="generateNewAddressCommand"></param>
        /// <returns></returns>
        public DepositAddressRepresentation GenarateNewAddress(GenerateNewAddressCommand generateNewAddressCommand)
        {
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(generateNewAddressCommand.Currency),
                new AccountId(generateNewAddressCommand.AccountId));
            if (balance != null)
            {
                if (balance.IsFrozen)
                {
                    throw new InvalidOperationException(string.Format("Account balance Frozen for Account ID = {0}, Currency = {1}",
                        generateNewAddressCommand.AccountId, generateNewAddressCommand.Currency));
                }
            }
            //Check if the required Tier Level for this operation is verified i.e., Tier 1
            if (_fundsValidationService.IsTierVerified(generateNewAddressCommand.AccountId, true).Item1)
            {
                List<DepositAddress> depositAddresses = _depositAddressRepository.
                    GetDepositAddressByAccountIdAndCurrency(
                        new AccountId(generateNewAddressCommand.AccountId), generateNewAddressCommand.Currency);

                if (depositAddresses != null && depositAddresses.Any())
                {
                    // Cannot allow more than 5 New Unused addresses at a time, so will raise exception if count exceeds or reaches 5
                    int counter = 0;
                    foreach (DepositAddress depositAddress1 in depositAddresses)
                    {
                        if (depositAddress1.Status == AddressStatus.New)
                        {
                            counter++;
                        }
                    }
                    if (counter >= 5)
                    {
                        Log.Debug(string.Format("Cannot create more than 5 new addresses at a time: Currency = {0}, AccountID = {1}",
                            generateNewAddressCommand.Currency, generateNewAddressCommand.AccountId));
                        throw new InvalidOperationException("Too many New addresses");
                    }
                }
                // Generate new address using the client
                string address = _clientInteractionService.GenerateNewAddress(generateNewAddressCommand.Currency);
                if (!string.IsNullOrEmpty(address))
                {
                    // Save the address in database
                    DepositAddress depositAddress = new DepositAddress(new Currency(generateNewAddressCommand.Currency),
                                                                       new BitcoinAddress(address), AddressStatus.New,
                                                                       DateTime.Now,
                                                                       new AccountId(generateNewAddressCommand.AccountId));
                    _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                    Log.Debug(string.Format("New address generated and saved: Currency = {0} | AccountID = {1}", 
                        generateNewAddressCommand.Currency, generateNewAddressCommand.AccountId));
                    return new DepositAddressRepresentation(address, AddressStatus.New.ToString());
                }
                throw new NullReferenceException(string.Format("Null address returned from client for currency: {0}", generateNewAddressCommand.Currency));
            }
            throw new InvalidOperationException(string.Format("Tier 1 is not verified: Account ID = {0}", 
                generateNewAddressCommand.AccountId));
        }

        /// <summary>
        /// Get the Threshold Limits for the given account and currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public DepositLimitThresholdsRepresentation GetThresholdLimits(int accountId, string currency)
        {
            AccountDepositLimits depositLimitThresholds = _fundsValidationService.GetDepositLimitThresholds(new AccountId(accountId), new Currency(currency));
            return new DepositLimitThresholdsRepresentation(depositLimitThresholds.Currency.Name,
                                                  depositLimitThresholds.AccountId.Value,
                                                  depositLimitThresholds.DailyLimit,
                                                  depositLimitThresholds.DailyLimitUsed,
                                                  depositLimitThresholds.MonthlyLimit,
                                                  depositLimitThresholds.MonthlyLimitUsed,
                                                  depositLimitThresholds.CurrentBalance,
                                                  depositLimitThresholds.MaximumDeposit);
        }

        /// <summary>
        /// Get the Bitcoin addresses saved against this user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        public IList<DepositAddressRepresentation> GetAddressesForAccount(int accountId, string currency)
        {
            List<DepositAddressRepresentation> depositAddressRepresentations = new List<DepositAddressRepresentation>();
            List<DepositAddress> depositAddresses = _depositAddressRepository.GetDepositAddressByAccountIdAndCurrency(
                new AccountId(accountId), currency);
            foreach (var depositAddress in depositAddresses)
            {
                // If the address has been used, there is a 7 day limit for its expiration from first use
                if (depositAddress.Status == AddressStatus.Used)
                {
                    if (depositAddress.DateUsed.AddDays(7) < DateTime.Now)
                    {
                        // If 7 dayshave passed after first use, mark this address expired
                        depositAddress.StatusExpired();
                        continue;
                    }
                }
                // Only pick addresses that are either New or Used, but not expired
                if (depositAddress.Status != AddressStatus.Expired)
                {
                    depositAddressRepresentations.Add(new DepositAddressRepresentation(depositAddress.BitcoinAddress.Value,
                                                         depositAddress.Status.ToString()));
                }
            }
            return depositAddressRepresentations;
        }

        /// <summary>
        /// Make the deposit
        /// </summary>
        /// <param name="makeDepositCommand"> </param>
        /// <returns></returns>
        public bool MakeDeposit(MakeDepositCommand makeDepositCommand)
        {
            if (_fundsValidationService.IsTierVerified(makeDepositCommand.AccountId, makeDepositCommand.IsCryptoCurrency).Item1)
            {
                Balance balance =
                    _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(makeDepositCommand.Currency),
                                                                        new AccountId(makeDepositCommand.AccountId));
                if (balance == null)
                {
                    balance = new Balance(new Currency(makeDepositCommand.Currency),
                                          new AccountId(makeDepositCommand.AccountId), makeDepositCommand.Amount,
                                          makeDepositCommand.Amount);
                }
                else
                {
                    balance.AddAvailableBalance(makeDepositCommand.Amount);
                    balance.AddCurrentBalance(makeDepositCommand.Amount);
                }
                _fundsPersistenceRepository.SaveOrUpdate(balance);

                if (makeDepositCommand.IsCryptoCurrency)
                {
                    Deposit deposit = new Deposit(new Currency(makeDepositCommand.Currency, true),
                                                  Guid.NewGuid().ToString(),
                                                  DateTime.Now, DepositType.Default, makeDepositCommand.Amount, 0,
                                                  TransactionStatus.Confirmed,
                                                  new AccountId(makeDepositCommand.AccountId), null, null);
                    _fundsPersistenceRepository.SaveOrUpdate(deposit);
                }
                return true;
            }
            throw new InvalidOperationException("Require Tier Level is not verified.");
        }

        /// <summary>
        /// Get the Daily and Monthly Tier limits for Deposit
        /// </summary>
        /// <returns></returns>
        public DepositTierLimitRepresentation GetDepositTiersLimits()
        {
            decimal tier0DailyLimit = 0;
            decimal tier0MonthlyLimit = 0;

            decimal tier1DailyLimit = 0;
            decimal tier1MonthlyLimit = 0;

            decimal tier2DailyLimit = 0;
            decimal tier2MonthlyLimit = 0;

            decimal tier3DailyLimit = 0;
            decimal tier3MonthlyLimit = 0;

            decimal tier4DailyLimit = 0;
            decimal tier4MonthlyLimit = 0;

            IList<DepositLimit> allDepositLimits = _depositLimitRepository.GetAllDepositLimits();
            if (allDepositLimits != null && allDepositLimits.Any())
            {
                foreach (DepositLimit depositLimit in allDepositLimits)
                {
                    if (depositLimit.TierLevel.Equals("Tier 0"))
                    {
                        tier0DailyLimit = depositLimit.DailyLimit;
                        tier0MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 1"))
                    {
                        tier1DailyLimit = depositLimit.DailyLimit;
                        tier1MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 2"))
                    {
                        tier2DailyLimit = depositLimit.DailyLimit;
                        tier2MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 3"))
                    {
                        tier3DailyLimit = depositLimit.DailyLimit;
                        tier3MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 4"))
                    {
                        tier4DailyLimit = depositLimit.DailyLimit;
                        tier4MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                }
                return new DepositTierLimitRepresentation(
                    tier0DailyLimit, tier0MonthlyLimit, tier1DailyLimit, tier1MonthlyLimit, tier2DailyLimit, tier2MonthlyLimit,
                    tier3DailyLimit, tier3MonthlyLimit, tier4DailyLimit, tier4MonthlyLimit);

            }
            return null;
        }
    }
}
