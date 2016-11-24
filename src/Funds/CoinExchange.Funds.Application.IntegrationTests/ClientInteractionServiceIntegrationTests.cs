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
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate;
using CoinExchange.Funds.Infrastructure.Services.CoinClientServices;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class ClientInteractionServiceIntegrationTests
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

        [Test]
        public void InitializationTest_TestsThatServiceIsInitializedProperly_VerifiesByInstanceValue()
        {
            IClientInteractionService clientInteractionService = (IClientInteractionService)ContextRegistry.GetContext()["ClientInteractionService"];
            Assert.IsNotNull(clientInteractionService);
        }

        /// <summary>
        /// Gets new address from the respective CoinClientService Implementation
        /// </summary>
        [Test]
        public void BtcNewDespoitAddressSuccessTest_TestsThatTheCLientServiceRequestsForAndReceviesTheNewDepositAddressSuccessfully()
        {
            IClientInteractionService clientInteractionService = (IClientInteractionService)ContextRegistry.GetContext()["ClientInteractionService"];

            string generateNewAddress = clientInteractionService.GenerateNewAddress(CurrencyConstants.Btc);
            Assert.IsFalse(string.IsNullOrEmpty(generateNewAddress));
        }

        /// <summary>
        /// Gets new address from the respective CoinClientService Implementation
        /// </summary>
        [Test]
        public void LtcNewDespoitAddressSuccessTest_TestsThatTheCLientServiceRequestsForAndReceviesTheNewDepositAddressSuccessfully()
        {
            IClientInteractionService clientInteractionService = (IClientInteractionService)ContextRegistry.GetContext()["ClientInteractionService"];

            string generateNewAddress = clientInteractionService.GenerateNewAddress(CurrencyConstants.Ltc);
            Assert.IsFalse(string.IsNullOrEmpty(generateNewAddress));
        }

        [Test]
        public void BtcWithdrawTest_TestsThatWithdrawIsSuccessfullySubmittedByTheService_VerifiesByTheRaisedEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            bool eventFired = true;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
                                                             {
                                                                 receivedWithdraw = incomingWithdraw;
                                                                 eventFired = true;
                                                                 manualResetEvent.Set();
                                                             };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Btc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Bitcoin, amount, 0.0001M, 
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);
        }

        [Test]
        public void LtcWithdrawTest_TestsThatWithdrawIsSuccessfullySubmittedByTheService_VerifiesByTheRaisedEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            bool eventFired = true;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
            {
                receivedWithdraw = incomingWithdraw;
                eventFired = true;
                manualResetEvent.Set();
            };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Ltc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Litecoin, amount, 0.0001M,
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);
        }

        [Test]
        public void BtcDepositArrivedTest_TestsThatDepositArrivedEventIsRaisedFromTheClient_VerifiesByHandlingEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            // Event handling for Deposit Arrived Event
            bool depositEventFired = false;
            ManualResetEvent depositManualResetEvent = new ManualResetEvent(false);
            string receivedCurrency = null;            
            clientInteractionService.DepositArrived += delegate(string incomingCurrency, List<Tuple<string, string, decimal, string>> arg2)
                                                           {
                                                               depositEventFired = true;
                                                               receivedCurrency = incomingCurrency;
                                                               depositManualResetEvent.Set();
                                                           };            
            
            // Event handling for Withdraw Executed Event
            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
                                                        {
                                                            receivedWithdraw = incomingWithdraw;
                                                            eventFired = true;
                                                            manualResetEvent.Set();
                                                        };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Btc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Bitcoin, amount, 0.0001M,
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();
            depositManualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);

            Assert.IsTrue(depositEventFired);
            Assert.AreEqual(CurrencyConstants.Btc, receivedCurrency);
        }

        [Test]
        public void LtcDepositArrivedTest_TestsThatDepositArrivedEventIsRaisedFromTheClient_VerifiesByHandlingEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            // Event handling for Deposit Arrived Event
            bool depositEventFired = false;
            ManualResetEvent depositManualResetEvent = new ManualResetEvent(false);
            string receivedCurrency = null;
            clientInteractionService.DepositArrived += delegate(string incomingCurrency, List<Tuple<string, string, decimal, string>> arg2)
            {
                depositEventFired = true;
                receivedCurrency = incomingCurrency;
                depositManualResetEvent.Set();
            };

            // Event handling for Withdraw Executed Event
            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
            {
                receivedWithdraw = incomingWithdraw;
                eventFired = true;
                manualResetEvent.Set();
            };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Ltc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Litecoin, amount, 0.0001M,
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();
            depositManualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);

            Assert.IsTrue(depositEventFired);
            Assert.AreEqual(CurrencyConstants.Btc, receivedCurrency);
        }

        [Test]
        public void BtcDepositConfirmedTest_TestsThatDepositArrivedAndDepositConfirmedEventsAreRaisedFromTheClient_VerifiesByHandlingEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            // Event handling for Deposit Arrived Event
            bool depositArrivedEventFired = false;
            ManualResetEvent depositManualResetEvent = new ManualResetEvent(false);
            string receivedCurrency = null;
            clientInteractionService.DepositArrived += delegate(string incomingCurrency, List<Tuple<string, string, decimal, string>> arg2)
                                                            {
                                                                depositArrivedEventFired = true;
                                                                receivedCurrency = incomingCurrency;
                                                            };

            bool depositConfirmedEventFired = false;
            string receivedTransactionId = null;
            clientInteractionService.DepositConfirmed += delegate(string arg1, int arg2)
                                                             {
                                                                 receivedTransactionId = arg1;
                                                                 depositConfirmedEventFired = true;
                                                                 depositManualResetEvent.Set();
                                                             };

            // Event handling for Withdraw Executed Event
            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
                                                            {
                                                                receivedWithdraw = incomingWithdraw;
                                                                eventFired = true;
                                                                manualResetEvent.Set();
                                                            };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Btc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Bitcoin, amount, 0.0001M,
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();
            depositManualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);

            Assert.IsTrue(depositArrivedEventFired);
            Assert.AreEqual(CurrencyConstants.Btc, receivedCurrency);

            Assert.IsTrue(depositConfirmedEventFired);
            Assert.AreEqual(receivedWithdraw.TransactionId.Value, receivedTransactionId);            
        }

        [Test]
        public void LtcDepositConfirmedTest_TestsThatDepositArrivedAndDepositConfirmedEventsAreRaisedFromTheClient_VerifiesByHandlingEvent()
        {
            // We will directly create Stub Implemenetations'instances as stubs are mentionedin configuration file in this project,
            // because we do not want the raised event to reach WithdrawApplicationService, so we would not inject services using 
            // Spring DI.

            FundsPersistenceRepository fundsPersistenceRepository = new FundsPersistenceRepository();
            IWithdrawRepository withdrawRepository = new WithdrawRepository();
            ICoinClientService bitcoinClientService = new StubBitcoinClientService();
            ICoinClientService litecoinClientService = new StubLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(fundsPersistenceRepository,
                withdrawRepository, bitcoinClientService, litecoinClientService);

            // Event handling for Deposit Arrived Event
            bool depositArrivedEventFired = false;
            ManualResetEvent depositManualResetEvent = new ManualResetEvent(false);
            string receivedCurrency = null;
            clientInteractionService.DepositArrived += delegate(string incomingCurrency, List<Tuple<string, string, decimal, string>> arg2)
            {
                depositArrivedEventFired = true;
                receivedCurrency = incomingCurrency;
            };

            bool depositConfirmedEventFired = false;
            string receivedTransactionId = null;
            clientInteractionService.DepositConfirmed += delegate(string arg1, int arg2)
            {
                receivedTransactionId = arg1;
                depositConfirmedEventFired = true;
                depositManualResetEvent.Set();
            };

            // Event handling for Withdraw Executed Event
            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw receivedWithdraw = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw incomingWithdraw)
            {
                receivedWithdraw = incomingWithdraw;
                eventFired = true;
                manualResetEvent.Set();
            };
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency(CurrencyConstants.Ltc, true);
            decimal amount = 0.02M;
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now, WithdrawType.Litecoin, amount, 0.0001M,
                TransactionStatus.Pending, accountId, new BitcoinAddress("bitcoinaddress1"));
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();
            depositManualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(withdraw.Currency.Name, receivedWithdraw.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, receivedWithdraw.AccountId.Value);
            Assert.AreEqual(withdraw.BitcoinAddress.Value, receivedWithdraw.BitcoinAddress.Value);
            Assert.AreEqual(withdraw.Fee, receivedWithdraw.Fee);
            Assert.AreEqual(TransactionStatus.Pending, receivedWithdraw.Status);
            Assert.IsNotNull(receivedWithdraw.TransactionId);
            Assert.AreEqual(withdraw.Type, receivedWithdraw.Type);
            Assert.AreEqual(withdraw.Amount, receivedWithdraw.Amount);
            Assert.AreEqual(withdraw.WithdrawId, receivedWithdraw.WithdrawId);

            Assert.IsTrue(depositArrivedEventFired);
            Assert.AreEqual(CurrencyConstants.Btc, receivedCurrency);

            Assert.IsTrue(depositConfirmedEventFired);
            Assert.AreEqual(receivedWithdraw.TransactionId.Value, receivedTransactionId);
        }
    }
}
