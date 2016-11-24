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
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.BalanceService;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    public class BalanceQueryServiceTests
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
        [Category("Integration")]
        public void TestBalanceQueryService_WhenAccountIdIsSupplied_ItShouldReturnListOfBalanceDetails()
        {
            IFundsPersistenceRepository repository =(IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];
            Balance balance=new Balance(new Currency("BTC"),new AccountId(1),1000,1000 );
            Balance balance2 = new Balance(new Currency("LTC"), new AccountId(1), 2000, 2000);
            repository.SaveOrUpdate(balance);
            repository.SaveOrUpdate(balance2);
            IBalanceQueryService _balanceQueryService =(IBalanceQueryService) ContextRegistry.GetContext()["BalanceQueryService"];
            List<BalanceDetails> balanceDetailses=_balanceQueryService.GetBalances(new AccountId(1));
            Assert.NotNull(balanceDetailses);
            Assert.AreEqual(2,balanceDetailses.Count);
            Assert.AreEqual(balanceDetailses[0].Currency,"BTC");
            Assert.AreEqual(balanceDetailses[0].Balance,1000);
            Assert.AreEqual(balanceDetailses[1].Currency, "LTC");
            Assert.AreEqual(balanceDetailses[1].Balance, 2000);
        }
    }
}
