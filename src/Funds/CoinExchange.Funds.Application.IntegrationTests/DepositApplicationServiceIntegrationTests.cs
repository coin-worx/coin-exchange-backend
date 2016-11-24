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
using CoinExchange.Funds.Application.DepositServices;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Application.WithdrawServices.Commands;
using CoinExchange.Funds.Application.WithdrawServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class DepositApplicationServiceIntegrationTests
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void DepositAddressCreationFailedTest_TestsIfDepositAddressIsActuallyNotCreatedIfTier1IsNotVerified_VerfiesThroughExceptionRaised()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];            
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            depositApplicationService.GenarateNewAddress(new GenerateNewAddressCommand(accountId.Value, currency.Name));
        }

        [Test]
        public void DepositAddressCreationSuccessTest_TestsIfDepositAddressIsActuallyCreatedIfTier1IsVerified_VerfiesThroughReturnValueAndDatabaseQuery()
        {
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            DepositAddressRepresentation addressResponse = depositApplicationService.GenarateNewAddress(new GenerateNewAddressCommand(accountId.Value, currency.Name));
            Assert.IsNotNull(addressResponse);
            Assert.AreEqual(AddressStatus.New.ToString(), addressResponse.Status);

            List<DepositAddress> depositAddresses = depositAddressRepository.GetDepositAddressByAccountIdAndCurrency(accountId, currency.Name);
            Assert.AreEqual(1, depositAddresses.Count);
            Assert.AreEqual(accountId.Value, depositAddresses.Single().AccountId.Value);
            Assert.AreEqual(currency.Name, depositAddresses.Single().Currency.Name);
            Assert.AreEqual(AddressStatus.New, depositAddresses.Single().Status);
        }

        [Test]
        public void DepositArrivedStandaloneFailedTest_TestsIfTheOperationIsAbortedWhenTheTierLevelIsNotHighEnough_VerifiesThroughQueryingDatabaseIsntances()
        {
            // Tests the event handler DepositArrived by directly sending data
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1,
                LimitsCurrency.Default.ToString());
            decimal amount = depositLimit.DailyLimit - 0.001M;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(amount, deposit.Amount);
            Assert.AreEqual(currency.Name, deposit.Currency.Name);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(TransactionStatus.Frozen, deposit.Status);

            // Confirm that the balance for the user has been frozen for this account
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.IsTrue(balance.IsFrozen);
        }

        [Test]
        public void DepositArrivedStandaloneFailedTest_TestsIfTheOperationIsAbortedWhenAmountIshigherThanThresholdLimit_VerifiesThroughQueryingDatabaseIsntances()
        {
            // Tests the event handler DepositArrived by directly sending data
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");

            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1,
                LimitsCurrency.Default.ToString());
            decimal amount = depositLimit.DailyLimit + 0.001M;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(amount, deposit.Amount);
            Assert.AreEqual(currency.Name, deposit.Currency.Name);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(TransactionStatus.Frozen, deposit.Status);

            // Confirm that the balance for the user has been frozen for this account
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.IsTrue(balance.IsFrozen);
        }

        [Test]
        public void DepositArrivedStandaloneTest_TestsIfTheOperationProceedsAsExpectedWHenANewDepositArrives_VerifiesThroughQueryingDatabaseIsntances()
        {
            // Tests the event handler DepositArrived by directly sending data
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            tierLevelRetrieval.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency(TierConstants.TierLevel1,
                LimitsCurrency.Default.ToString());
            decimal amount = depositLimit.DailyLimit - 0.001M;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, 
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string,string,decimal,string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(2000);
            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Amount, amount);
            Assert.AreEqual(deposit.Currency.Name, currency.Name);
            Assert.IsTrue(deposit.Currency.IsCryptoCurrency);
            Assert.AreEqual(deposit.TransactionId.Value, transactionId.Value);
            Assert.AreEqual(deposit.BitcoinAddress.Value, bitcoinAddress.Value);
            Assert.AreEqual(deposit.Status, TransactionStatus.Pending);

            depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(depositAddress.Status, AddressStatus.Used);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);
        }

        [Test]
        public void DepositConfirmedTest_TestsIfTheOperationProceedsAsExpectedWhenADepositIsConfirmed_VerifiesThroughQueryingDatabaseIsntances()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            if (tierLevelRetrievalService != null)
            {
                tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            }
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            decimal amount = 1.02m;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Amount, amount);
            Assert.AreEqual(deposit.Currency.Name, currency.Name);
            Assert.IsTrue(deposit.Currency.IsCryptoCurrency);
            Assert.AreEqual(deposit.TransactionId.Value, transactionId.Value);
            Assert.AreEqual(deposit.BitcoinAddress.Value, bitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);

            depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.Used, depositAddress.Status);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(0, ledgers.Count);

            depositApplicationService.OnDepositConfirmed(transactionId.Value, 7);

            deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Status, TransactionStatus.Confirmed);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(amount, balance.AvailableBalance);
            Assert.AreEqual(amount, balance.CurrentBalance);
            Assert.AreEqual(0, balance.PendingBalance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(1, ledgers.Count);
            var ledger = ledgers.SingleOrDefault();
            Assert.IsNotNull(ledger);
            Assert.AreEqual(LedgerType.Deposit, ledger.LedgerType);
            Assert.AreEqual(accountId.Value, ledger.AccountId.Value);
            Assert.AreEqual(amount, ledger.Amount);
            Assert.AreEqual(deposit.DepositId, ledger.DepositId);
            Assert.AreEqual(0, ledger.Fee);
        }

        [Test]
        public void DepositArrivedFailTest_VerifiesThatNewDepositWithAMountGreaterThanLimitsIsSuspendedAndAccountBalanceFrozen_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress =  new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit + 0.001M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(TransactionStatus.Frozen, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.IsTrue(balance.IsFrozen);
        }

        [Test]
        public void DepositArrivedStandaloneTest_ChecksThatNewDepositInstanceIsCreatedAsExpectedWhenANewTrnasactionComesIn_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            tierLevelRetrieval.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit - 0.01M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
        }

        [Test]
        public void DepositArrivedAndConfirmedTest_ChecksThatANewDepositInstanceIsCreatedAndConfirmedAsExpected_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();

            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit - 0.01M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            // Deposit Transaction first arrival
            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            // Deposit Confirmation
            depositApplicationService.OnDepositConfirmed(transactionId.Value, 7);
            deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Confirmed, deposit.Status);

            // Balance instance now will have been created
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, balance.AvailableBalance);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, balance.CurrentBalance);
            Assert.AreEqual(0, balance.PendingBalance);
            Assert.IsFalse(balance.IsFrozen);
        }

        [Test]
        public void AssignDepositLimits1Test_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsVerified_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];

            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        public void AssignDepositLimits2Test_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsVerifiedAndBalanceIsAlreadyPresent_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            decimal balanceAmount = 100;
            Balance balance = new Balance(currency, accountId, balanceAmount, balanceAmount);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(balanceAmount, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsNotVerifiedAndBalanceIsAlreadyPresent_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            tierLevelRetrieval.SetTierLevel("Tier 0");
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 0", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(depositLimit);
            Assert.AreEqual(0, depositLimit.DailyLimit);
            Assert.AreEqual(0, depositLimit.MonthlyLimit);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(0, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreNotAssignedWhenNoDepositLimitIsFound_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            // Specify invalid tier level
            tierLevelRetrieval.SetTierLevel("Tier 6");
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 0", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(depositLimit);
            Assert.AreEqual(0, depositLimit.DailyLimit);
            Assert.AreEqual(0, depositLimit.MonthlyLimit);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            // Exception occurs here because there is no such level as Tier 6
            depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
        }

        [Test]
        public void DepositArrivedAndConfirmedEventTest_TestsThatDepositArrivedEventIsHandledAsExpected_VerifiesThroughRaisedEvent()
        {
            // Withdraw is submitted and events are actually raised from the CoinClientService
            // Over the limit deposit so account and deposit are frozen
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");
            decimal amount = depositLimit.DailyLimit - 0.001M;

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            Balance balance = new Balance(currency, accountId, 10, 10);
            fundsPersistenceRepository.SaveOrUpdate(balance);
            CommitWithdrawResponse commitWithdrawResponse = withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(
                accountId.Value, currency.Name, currency.IsCryptoCurrency, bitcoinAddress.Value, amount));
            Assert.IsNotNull(commitWithdrawResponse);
            Assert.IsTrue(commitWithdrawResponse.CommitSuccessful);
            // Get the interval after which the withdraw will be submitted. Once submitted to the StubClientService, the events
            // of DepositArrived will be raised and handled in Deposit APplicationService, which will create a new Deposit
            // instance and save in database
            int withdrawInterval = Convert.ToInt16(ConfigurationManager.AppSettings.Get("WithdrawSubmitInterval"));
            manualResetEvent.WaitOne(withdrawInterval + 2000);

            Withdraw withdraw = withdrawRepository.GetWithdrawByWithdrawId(commitWithdrawResponse.WithdrawId);
            Assert.IsNotNull(withdraw);
            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(amount - withdraw.Fee, deposit.Amount);
            Assert.AreEqual(withdraw.TransactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Confirmed, deposit.Status);
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);

            // Balance instance now will have been created
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(0, balance.PendingBalance);
            Assert.IsFalse(balance.IsFrozen);
        }
    }
}
