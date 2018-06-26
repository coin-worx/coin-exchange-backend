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
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    /// <summary>
    /// Tests that do not actually save the objects in the database, but use the configuration for NHibernate to virtually 
    /// save and retreive objects on the fly
    /// </summary>
    [TestFixture]
    class WithdrawAddressVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawAddressRepository _withdrawAddressRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IWithdrawAddressRepository WithdrawAddressRepository
        {
            set { _withdrawAddressRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveWithdrawAddressesAndRetreiveByAccountIdTest_SavesMultipleObjectsToDatabase_ChecksIfRetreivedOutputIsAsExpected()
        {
            WithdrawAddress withdrawAddress = new WithdrawAddress(new Currency("XBT", true), new BitcoinAddress("iambitcoin123"), "Description is for dummies",
                new AccountId(1), DateTime.Now);

            _persistanceRepository.SaveOrUpdate(withdrawAddress);

            WithdrawAddress deposit2 = new WithdrawAddress(new Currency("XBT", true), new BitcoinAddress("321nioctibmai"), "Description is for champs",
                new AccountId(1), DateTime.Now);
            Thread.Sleep(500);

            _persistanceRepository.SaveOrUpdate(deposit2);

            List<WithdrawAddress> retrievedWithdrawAddressList = _withdrawAddressRepository.GetWithdrawAddressByAccountId(new AccountId(1));
            Assert.IsNotNull(retrievedWithdrawAddressList);
            Assert.AreEqual(2, retrievedWithdrawAddressList.Count);

            Assert.AreEqual(withdrawAddress.BitcoinAddress.Value, retrievedWithdrawAddressList[0].BitcoinAddress.Value);
            Assert.AreEqual(withdrawAddress.Description, retrievedWithdrawAddressList[0].Description);
            Assert.AreEqual(withdrawAddress.AccountId.Value, retrievedWithdrawAddressList[0].AccountId.Value);

            Assert.AreEqual(deposit2.BitcoinAddress.Value, retrievedWithdrawAddressList[1].BitcoinAddress.Value);
            Assert.AreEqual(deposit2.Description, retrievedWithdrawAddressList[1].Description);
            Assert.AreEqual(deposit2.AccountId.Value, retrievedWithdrawAddressList[1].AccountId.Value);
        }
    }
}
