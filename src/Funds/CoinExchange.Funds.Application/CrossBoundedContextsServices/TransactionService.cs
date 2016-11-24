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
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service for making transaction resulting in Ledgers
    /// </summary>
    public class TransactionService : ITransactionService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private ILedgerIdGeneraterService _ledgerIdGeneraterService;
        private ILimitsConfigurationService _limitsConfigurationService;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="fundsPersistenceRepository"> </param>
        /// <param name="ledgerIdGeneratorService"></param>
        /// <param name="limitsConfigurationService"> </param>
        public TransactionService(IFundsPersistenceRepository fundsPersistenceRepository, 
            ILedgerIdGeneraterService ledgerIdGeneratorService, ILimitsConfigurationService limitsConfigurationService)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _ledgerIdGeneraterService = ledgerIdGeneratorService;
            _limitsConfigurationService = limitsConfigurationService;
        }

        /// <summary>
        /// Creates a ledger entry for one currency of one of the two order sids of a trade
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="amountInUsd"> </param>
        /// <param name="fee"></param>
        /// <param name="balance"></param>
        /// <param name="executionDate"></param>
        /// <param name="orderId"></param>
        /// <param name="tradeId"></param>
        /// <param name="accountId"></param>
        /// <param name="isBaseCurrencyInTrade"> </param>
        /// <returns></returns>
        public bool CreateLedgerEntry(Currency currency, decimal amount, decimal amountInUsd, decimal fee, decimal balance,
            DateTime executionDate, string orderId, string tradeId, AccountId accountId, bool isBaseCurrencyInTrade)
        {
            try
            {
                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), executionDate, LedgerType.Trade, 
                    currency, amount, amountInUsd, fee, balance, tradeId, orderId, isBaseCurrencyInTrade, accountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                Log.Debug(string.Format("Ledger Trade Transaction saved: Currency = {0} | Account ID = {1} | " +
                                        "Amount = {2} | Fee = {3} | Balance = {4} | TradeID = {5} | OrderID = {6}", 
                                        currency.Name, accountId.Value, amount, fee, balance, tradeId, orderId));
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return false;
            }
        }

        /// <summary>
        /// Creates a transaction in result of a Deposit
        /// </summary>
        /// <param name="deposit"> </param>
        /// <param name="balance"> </param>
        /// <returns></returns>
        public bool CreateDepositTransaction(Deposit deposit, decimal balance)
        {
            if (deposit != null)
            {
                // double currenctBalance = _ledgerRepository.GetBalanceForCurrency(deposit.Currency.Name, 
                //    new AccountId(deposit.AccountId.Value));

                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), DateTime.Now,
                                           LedgerType.Deposit, deposit.Currency, deposit.Amount, 
                                           _limitsConfigurationService.ConvertCurrencyToFiat(deposit.Currency.Name, deposit.Amount),
                                           balance, null, null, null, deposit.DepositId,
                                           deposit.AccountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                Log.Debug(string.Format("Ledger Deposit Transaction saved: Currency = {0} | Account ID = {1} | " +
                                        "Amount = {2} | Fee = {3} | Balance = {4} | DepositID = {5} | Address = {6} | " +
                                        "TransactionID = {7} | DateTime = {8}",
                                        deposit.Currency.Name, deposit.AccountId.Value, deposit.Amount, deposit.Fee, balance, 
                                        deposit.DepositId, deposit.BitcoinAddress, deposit.TransactionId, deposit.Date));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a transaction in result of a Withdraw
        /// </summary>
        /// <param name="withdraw"> </param>
        /// <param name="balance"> </param>
        /// <returns></returns>
        public bool CreateWithdrawTransaction(Withdraw withdraw, decimal balance)
        {
            if (withdraw != null)
            {
                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), DateTime.Now,
                                           LedgerType.Withdrawal,
                                           withdraw.Currency, withdraw.Amount, withdraw.Fee,
                                           balance, null, null, withdraw.WithdrawId,
                                           null, withdraw.AccountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                Log.Debug(string.Format("Ledger Withdraw Transaction saved: Currency = {0} | Account ID = {1} | " +
                                        "Amount = {2} | Fee = {3} | Balance = {4} | WithdrawID = {5} | Address = {6} | " +
                                        "TransactionID = {7} | DateTime = {8}",
                                        withdraw.Currency.Name, withdraw.AccountId.Value, withdraw.Amount, withdraw.Fee, balance,
                                        withdraw.WithdrawId, withdraw.BitcoinAddress, withdraw.TransactionId, withdraw.DateTime));
                return true;
            }
            return false;
        }
    }
}
