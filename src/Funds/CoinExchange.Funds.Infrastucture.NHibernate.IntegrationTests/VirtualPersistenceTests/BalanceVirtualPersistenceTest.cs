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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    [TestFixture]
    class BalanceVirtualPersistenceTest : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IBalanceRepository _balanceRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IBalanceRepository DepositLimitRepository
        {
            set { _balanceRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveDepositAddressesAndRetreiveByAccountIdTest_SavesObjectsToDatabase_ChecksIfTheyAreAsExpected()
        {
            Balance balance = new Balance(new Currency("LTC", true), new AccountId(1), 4000, 5000);

            _persistanceRepository.SaveOrUpdate(balance);

            Balance retrievedDepositAddressList = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retrievedDepositAddressList);

            Assert.AreEqual(balance.AvailableBalance, retrievedDepositAddressList.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, retrievedDepositAddressList.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, retrievedDepositAddressList.PendingBalance);
        }

        [Test]
        public void SavePendingTransactionsTest_SavesObjectsToPendingTransactionsList_ChecksIfTheyAreAsExpected()
        {
            Balance balance = new Balance(new Currency("LTC", true), new AccountId(1), 5000, 5000);
            bool addPendingTransaction = balance.AddPendingTransaction("withdrawid123", PendingTransactionType.Withdraw, -500);
            Assert.IsTrue(addPendingTransaction);

            _persistanceRepository.SaveOrUpdate(balance);

            Balance retrievedDepositAddressList = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retrievedDepositAddressList);

            Assert.AreEqual(4500, retrievedDepositAddressList.AvailableBalance);
            Assert.AreEqual(5000, retrievedDepositAddressList.CurrentBalance);
            Assert.AreEqual(500, retrievedDepositAddressList.PendingBalance);

            balance.ConfirmPendingTransaction("withdrawid123", PendingTransactionType.Withdraw, -500);

            _persistanceRepository.SaveOrUpdate(balance);
            Balance retreivedBalance = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retreivedBalance);
            Assert.AreEqual(4500, retreivedBalance.AvailableBalance);
            Assert.AreEqual(4500, retreivedBalance.CurrentBalance);
            Assert.AreEqual(0, retreivedBalance.PendingBalance);
        }
    }
}
