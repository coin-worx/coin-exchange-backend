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


﻿using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests that actaully persist data in the database and cleanup on tear down
    /// </summary>
    [TestFixture]
    class WithdrawLimitDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawLimitRepository _withdrawLimitRepository;

        [SetUp]
        public void Setup()
        {
            _withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            _persistanceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
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
        public void SaveWithdrawLimitAndRetreiveByTierLevelTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawLimit withdrawLimit = new WithdrawLimit("tierlevel1", 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawLimit);

            WithdrawLimit retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedWithdrawLimit);

            Assert.AreEqual(withdrawLimit.DailyLimit, retrievedWithdrawLimit.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, retrievedWithdrawLimit.MonthlyLimit);
        }

        [Test]
        public void SaveWithdrawLimitAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawLimit withdrawLimit = new WithdrawLimit("tierlevel1", 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawLimit);

            WithdrawLimit retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedWithdrawLimit);
            int id = retrievedWithdrawLimit.Id;
            _persistanceRepository.SaveOrUpdate(retrievedWithdrawLimit);

            retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitById(id);
            Assert.AreEqual(withdrawLimit.DailyLimit, retrievedWithdrawLimit.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, retrievedWithdrawLimit.MonthlyLimit);
        }
    }
}
