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
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Infrastructure.Services.CoinClientServices;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastructure.Services.IntegrationTests
{
    [TestFixture]
    class LitecoinClientServiceIntegrationTests
    {
        // Care must be taken to decide this amount, as this will subtract the currency from the testnet version
        // of bitcoin
        private const decimal Amount = 0.0001m;

        // As these tests need bitcoins to run from the test account, or may be even real bitcoins, this flag is for 
        // safety and must be set to true to run tests related to deposits and withdrawals
        private bool _shouldRunTests = false;

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
        public void CreateNewAddressTest_TestsWhetherTheServiceCreatesNewAddressSuccessfullyOrNot_VerifiesByTheReturnedResult()
        {
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["LitecoinClientService"];

                string newAddress = coinClientService.CreateNewAddress();
                Assert.IsNotNull(newAddress);
                Assert.IsFalse(string.IsNullOrEmpty(newAddress));
            }
        }

        [Test]
        public void CommitWithdrawTest_TestsIfTheWithdrawalIsMadeAsExpected_VerifiesTheReturnedResponseAndCheskTheBlockChainToConfirm()
        {
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService)ContextRegistry.GetContext()["LitecoinClientService"];

                decimal fee = 0.0001m;
                AccountId accountId = new AccountId(1);
                string newAddress = coinClientService.CreateNewAddress();
                Withdraw withdraw = new Withdraw(new Currency("LTC", true), Guid.NewGuid().ToString(), DateTime.Now,
                                                 WithdrawType.Bitcoin, Amount, fee,
                                                 TransactionStatus.Pending, accountId,
                                                 new BitcoinAddress(newAddress));
                string transactionId = coinClientService.CommitWithdraw(withdraw.BitcoinAddress.Value, withdraw.Amount);
                Assert.IsNotNull(transactionId);
            }
        }

        [Test]
        public void CheckBalanceTest_TestIfBalanceIsCheckedAndReturnedProperly_VerifiesThroughReturnedValue()
        {
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["LitecoinClientService"];

                decimal checkBalance = coinClientService.CheckBalance("LTC");

                Assert.AreNotEqual(0, checkBalance);
            }
        }

        [Test]
        public void DepositCheckNewTransactionsTest_TestIfDepositHandlingIsDoneAsExpected_VerifiesThroughReturnedValue()
        {
            // Submits withdraw to own address, after the first NewTransactionInterval has been elapsed.
            // Checks if new deposit has been created by the DepositAPplicationService and DepositAddress marked used
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService)ContextRegistry.GetContext()["LitecoinClientService"];
                IFundsPersistenceRepository fundsPersistenceRepository =
                    (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
                IDepositAddressRepository depositAddressRepository =
                    (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
                IDepositRepository depositRepository =
                    (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];

                Currency currency = new Currency("LTC", true);
                AccountId accountId = new AccountId(1);
                string newAddress = coinClientService.CreateNewAddress();
                BitcoinAddress bitcoinAddress = new BitcoinAddress(newAddress);
                DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress,
                                                                   AddressStatus.New, DateTime.Now, accountId);
                fundsPersistenceRepository.SaveOrUpdate(depositAddress);

                // Check that there is no deposit with htis address present
                List<Deposit> deposits = depositRepository.GetDepositsByBitcoinAddress(bitcoinAddress);
                Assert.AreEqual(0, deposits.Count);

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                // Wait for the first interval to elapse, and then withdraw, because only then we will be able to figure if a 
                // new transaction has been received
                manualResetEvent.WaitOne(Convert.ToInt32(coinClientService.PollingInterval + 3000));

                manualResetEvent.Reset();
                bool eventFired = false;
                coinClientService.DepositArrived += delegate(string curr, List<Tuple<string, string, decimal, string>> pendingList)
                {
                    eventFired = true;
                    manualResetEvent.Set();
                };

                Withdraw withdraw = new Withdraw(currency, Guid.NewGuid().ToString(), DateTime.Now, WithdrawType.Bitcoin,
                                                 Amount, 0.001m, TransactionStatus.Pending, accountId,
                                                 new BitcoinAddress(newAddress));
                string commitWithdraw = coinClientService.CommitWithdraw(withdraw.BitcoinAddress.Value, withdraw.Amount);
                Assert.IsNotNull(commitWithdraw);
                Assert.IsFalse(string.IsNullOrEmpty(commitWithdraw));
                manualResetEvent.WaitOne();

                Assert.IsTrue(eventFired);
                depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
                Assert.IsNotNull(depositAddress);
                Assert.AreEqual(AddressStatus.Used, depositAddress.Status);

                // See If DepositApplicationService created the deposit instance
                deposits = depositRepository.GetDepositsByBitcoinAddress(bitcoinAddress);
                Deposit deposit = deposits.Single();
                Assert.IsNotNull(deposit);
                Assert.AreEqual(Amount, deposit.Amount);
                Assert.AreEqual(currency.Name, deposit.Currency.Name);
                Assert.IsFalse(string.IsNullOrEmpty(deposit.TransactionId.Value));
                Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
                Assert.AreEqual(DepositType.Default, deposit.Type);
                Assert.AreEqual(0, deposit.Confirmations);
                Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
                Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            }
        }

        [Test]
        public void NewTransactionClientStandaloneTest_TestsIfTheRaisedEventForNewTransactionSendsTheCorrectData_VerifiesThroughReturnedValue()
        {
            if (_shouldRunTests)
            {
                // Checks the data when a DepositArrived event is raised by the CoinClientService. Sees that the parameters are as 
                // expected
                ICoinClientService coinClientService = new LitecoinClientService();

                string newAddress = coinClientService.CreateNewAddress();
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                // Wait for the first interval to elapse, and then withdraw, because only then we will be able to figure if a 
                // new transaction has been received
                manualResetEvent.WaitOne(Convert.ToInt32(coinClientService.PollingInterval + 3000));

                manualResetEvent.Reset();
                bool eventFired = false;
                string receivedCurrency = null;
                List<Tuple<string, string, decimal, string>> receivedTransactionList = null;
                coinClientService.DepositArrived +=
                    delegate(string curr, List<Tuple<string, string, decimal, string>> pendingList)
                        {
                            eventFired = true;
                            receivedCurrency = curr;
                            receivedTransactionList = pendingList;
                            manualResetEvent.Set();
                        };

                string commitWithdraw = coinClientService.CommitWithdraw(newAddress, Amount);
                Assert.IsNotNull(commitWithdraw);
                Assert.IsFalse(string.IsNullOrEmpty(commitWithdraw));
                manualResetEvent.WaitOne();

                Assert.IsTrue(eventFired);
                Assert.AreEqual(CurrencyConstants.Ltc, receivedCurrency);
                Assert.AreEqual(1, receivedTransactionList.Count);
                Assert.AreEqual(newAddress, receivedTransactionList[0].Item1);
                Assert.AreEqual(Amount, receivedTransactionList[0].Item3);
                Assert.AreEqual(BitcoinConstants.ReceiveCategory, receivedTransactionList[0].Item4);
            }
        }
    }
}
