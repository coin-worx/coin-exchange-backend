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
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Application.WithdrawServices.Commands;
using CoinExchange.Funds.Application.WithdrawServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class WithdrawApplicationServiceIntegrationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void Teardown()
        {
            _databaseUtility.Create();
        }

        // ToDo: Test for over the limit withdrawal


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailedTest_TestsIfWithdrawFailsWhenTirLevelIsNotHighEnough_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            decimal amount = 1.02m;

            withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name,
                                                                                  currency.IsCryptoCurrency,
                                                                                  bitcoinAddress.Value, amount));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailedTest_TestsIfWithdrawFailsWhenMaximumWIthdrawLimitIsOverTheThresholdLimit_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1, LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit + 1;

            Balance balance = new Balance(currency, accountId, amount + 1, amount + 1);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name,
                                                                                  currency.IsCryptoCurrency,
                                                                                  bitcoinAddress.Value, amount));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailedTest_TestsIfWithdrawFailsBecauseOfNotEnoughBalance_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1, LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit + 0.001M;

            Balance balance = new Balance(currency, accountId, amount - 1, amount - 1);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name,
                                                                                  currency.IsCryptoCurrency,
                                                                                  bitcoinAddress.Value, amount));
        }

        [Test]
        public void WithdrawSuccessfulTest_TestsIfWithdrawIsSuccessfulWhenTierLevelIsHighEnough_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1, LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit;

            Balance balance = new Balance(currency, accountId, amount + 1, amount + 1);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            CommitWithdrawResponse commitWithdrawResponse = withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name, currency.IsCryptoCurrency, bitcoinAddress.Value, amount));
            Assert.IsNotNull(commitWithdrawResponse);
            Assert.IsTrue(commitWithdrawResponse.CommitSuccessful);

            Withdraw withdraw = withdrawRepository.GetWithdrawByWithdrawId(commitWithdrawResponse.WithdrawId);
            Assert.IsNotNull(withdraw);
            Assert.AreEqual(accountId.Value, withdraw.AccountId.Value);
            Assert.AreEqual(currency.Name, withdraw.Currency.Name);
            Assert.AreEqual(currency.IsCryptoCurrency, withdraw.Currency.IsCryptoCurrency);
            Assert.AreEqual(amount - withdraw.Fee, withdraw.Amount);
            Assert.AreEqual(TransactionStatus.Pending, withdraw.Status);

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual((amount + 1) - (amount), balance.AvailableBalance);
            Assert.AreEqual(amount + 1, balance.CurrentBalance);
            Assert.AreEqual(amount, balance.PendingBalance);
        }

        [Test]
        public void AssignWithdrawLimitsTest_TestsIfWithdrawLimitsGetAsignedProperlyWhenTier1IsVerifiedAndBalanceIsLessThanDailyLimit_VerifiesThroughReturnedValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];            
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];           
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit;

            Balance balance = new Balance(currency, accountId, amount - 0.001M, amount);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            WithdrawLimitRepresentation withdrawLimitRepresentation = withdrawApplicationService.GetWithdrawLimitThresholds(accountId.Value, currency.Name);
            Assert.IsNotNull(withdrawLimitRepresentation);
            Assert.AreEqual(currency.Name, withdrawLimitRepresentation.Currency);
            Assert.AreEqual(accountId.Value, withdrawLimitRepresentation.AccountId);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, withdrawLimitRepresentation.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitRepresentation.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.MonthlyLimitUsed);
            Assert.AreEqual(amount - 0.001M, withdrawLimitRepresentation.CurrentBalance);
            Assert.AreEqual(withdrawFees.Fee, withdrawLimitRepresentation.Fee);
            Assert.AreEqual(withdrawFees.MinimumAmount, withdrawLimitRepresentation.MinimumAmount);
            Assert.AreEqual(0.001M, withdrawLimitRepresentation.Withheld);
            Assert.AreEqual(amount - 0.001M, withdrawLimitRepresentation.MaximumWithdraw);
        }

        [Test]
        public void AssignWithdrawLimitsTest_TestsIfWithdrawLimitsGetAsignedProperlyWhenTier1IsVerifiedAndBalanceIsMoreThanDailyLimit_VerifiesThroughReturnedValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit + 0.001M;

            Balance balance = new Balance(currency, accountId, amount, amount);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            WithdrawLimitRepresentation withdrawLimitRepresentation = withdrawApplicationService.GetWithdrawLimitThresholds(accountId.Value, currency.Name);
            Assert.IsNotNull(withdrawLimitRepresentation);
            Assert.AreEqual(currency.Name, withdrawLimitRepresentation.Currency);
            Assert.AreEqual(accountId.Value, withdrawLimitRepresentation.AccountId);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, withdrawLimitRepresentation.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitRepresentation.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.MonthlyLimitUsed);
            Assert.AreEqual(amount, withdrawLimitRepresentation.CurrentBalance);
            Assert.AreEqual(withdrawFees.Fee, withdrawLimitRepresentation.Fee);
            Assert.AreEqual(withdrawFees.MinimumAmount, withdrawLimitRepresentation.MinimumAmount);
            Assert.AreEqual(0, withdrawLimitRepresentation.Withheld);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.MaximumWithdraw);
        }

        [Test]
        public void AssignWithdrawLimitsTest_TestsIfWithdrawLimitsGetAsignedProperlyWhenNoBalanceisPresent_VerifiesThroughReturnedValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            
            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            WithdrawLimitRepresentation withdrawLimitRepresentation = withdrawApplicationService.GetWithdrawLimitThresholds(accountId.Value, currency.Name);
            Assert.IsNotNull(withdrawLimitRepresentation);
            Assert.AreEqual(currency.Name, withdrawLimitRepresentation.Currency);
            Assert.AreEqual(accountId.Value, withdrawLimitRepresentation.AccountId);
            Assert.AreEqual(withdrawLimit.DailyLimit, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, withdrawLimitRepresentation.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitRepresentation.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.MonthlyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.CurrentBalance);
            Assert.AreEqual(withdrawFees.Fee, withdrawLimitRepresentation.Fee);
            Assert.AreEqual(withdrawFees.MinimumAmount, withdrawLimitRepresentation.MinimumAmount);
            Assert.AreEqual(0, withdrawLimitRepresentation.Withheld);
            Assert.AreEqual(0, withdrawLimitRepresentation.MaximumWithdraw);
        }

        [Test]
        public void AssignWithdrawLimitsTest_TestsIfWithdrawLimitsGetAsignedProperlyWhenTier1IsNotVerified_VerifiesThroughReturnedValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];            
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            
            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            WithdrawLimitRepresentation withdrawLimitRepresentation = withdrawApplicationService.GetWithdrawLimitThresholds(accountId.Value, currency.Name);
            Assert.IsNotNull(withdrawLimitRepresentation);
            Assert.AreEqual(currency.Name, withdrawLimitRepresentation.Currency);
            Assert.AreEqual(accountId.Value, withdrawLimitRepresentation.AccountId);
            Assert.AreEqual(0, withdrawLimitRepresentation.DailyLimit);
            Assert.AreEqual(0, withdrawLimitRepresentation.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitRepresentation.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.MonthlyLimitUsed);
            Assert.AreEqual(0, withdrawLimitRepresentation.CurrentBalance);
            Assert.AreEqual(withdrawFees.Fee, withdrawLimitRepresentation.Fee);
            Assert.AreEqual(withdrawFees.MinimumAmount, withdrawLimitRepresentation.MinimumAmount);
            Assert.AreEqual(0, withdrawLimitRepresentation.Withheld);
            Assert.AreEqual(0, withdrawLimitRepresentation.MaximumWithdraw);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignWithdrawLimitsTest_TestsIfWithdrawLimitsGetAsignedProperlyWhenInvalidTierLevelIsSpecified_VerifiesThroughReturnedValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];            
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel("Tier 6");
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            withdrawApplicationService.GetWithdrawLimitThresholds(accountId.Value, currency.Name);
        }

        [Test]
        public void WithdrawExecutedEventTest_ChecksThatTheEventIsProperlyRaisedAndHandledWhenWithdrawIsSubmittedToTheNetwork_VerifiesThroughRaisedEvent()
        {
            // Withdraw is submitted and upon submission to network an event is raised confirming withdrawal execution which
            // is handled and balance is updated. This whole process of events firing and balance validation is checked by this
            // test case
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            IClientInteractionService clientInteractionService = (IClientInteractionService)ContextRegistry.GetContext()["ClientInteractionService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Btc, true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            WithdrawLimit withdrawLimit = withdrawLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1, LimitsCurrency.Default.ToString());
            Assert.IsNotNull(withdrawLimit);
            decimal amount = withdrawLimit.DailyLimit;

            Balance balance = new Balance(currency, accountId, amount + 1, amount + 1);
            fundsPersistenceRepository.SaveOrUpdate(balance);
            bool withdrawEventRaised = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
                                                             {
                                                                 withdrawEventRaised = true;
                                                                 receivedWithdraw = incomingWithdraw;
                                                                 manualResetEvent.Set();
                                                             };
            
            CommitWithdrawResponse commitWithdrawResponse = withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name, currency.IsCryptoCurrency, bitcoinAddress.Value, amount));
            Assert.IsNotNull(commitWithdrawResponse);
            Assert.IsTrue(commitWithdrawResponse.CommitSuccessful);
            manualResetEvent.WaitOne();

            // Apply assertions after event has been handled
            Assert.IsTrue(withdrawEventRaised);
            Assert.IsNotNull(receivedWithdraw);
            Withdraw withdraw = withdrawRepository.GetWithdrawByWithdrawId(commitWithdrawResponse.WithdrawId);
            Assert.IsNotNull(withdraw);
            Assert.AreEqual(accountId.Value, withdraw.AccountId.Value);
            Assert.AreEqual(currency.Name, withdraw.Currency.Name);
            Assert.AreEqual(currency.IsCryptoCurrency, withdraw.Currency.IsCryptoCurrency);
            Assert.AreEqual(amount - withdraw.Fee, withdraw.Amount);
            Assert.AreEqual(TransactionStatus.Confirmed, withdraw.Status);

            Assert.AreEqual(receivedWithdraw.AccountId.Value, withdraw.AccountId.Value);
            Assert.AreEqual(receivedWithdraw.TransactionId.Value, withdraw.TransactionId.Value);
            Assert.AreEqual(receivedWithdraw.BitcoinAddress.Value, withdraw.BitcoinAddress.Value);
            Assert.AreEqual(receivedWithdraw.Currency.Name, withdraw.Currency.Name);
            Assert.AreEqual(TransactionStatus.Confirmed, withdraw.Status);
            Assert.AreEqual(receivedWithdraw.Amount, withdraw.Amount);
            Assert.AreEqual(receivedWithdraw.WithdrawId, withdraw.WithdrawId);

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Assert.IsNotNull(withdrawFees);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual((amount + 1) - (amount), balance.AvailableBalance);
            Assert.AreEqual((amount + 1) - (amount), balance.CurrentBalance);
            Assert.AreEqual(0, balance.PendingBalance);
        }
    }
}
