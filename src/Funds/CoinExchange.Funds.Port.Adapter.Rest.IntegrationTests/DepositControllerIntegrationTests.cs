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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate;
using CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit;
using CoinExchange.Funds.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    class DepositControllerIntegrationTests
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
        public void DepositControllerInitializationTest_ChecksIfTheControllerInitializesAsExpected_VerifiesThroughInstance()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            Assert.IsNotNull(depositController);
        }

        [Test]
        [Category("Integration")]
        public void DepositAddressTest_NewAddressMustBeGenerated_VerifiesThroughDatabaseQueryAndReturnValue()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            Assert.IsNotNull(depositController);

            string apiKey = "apiKey";
            depositController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            depositController.Request.Headers.Add("Auth", apiKey);
            IHttpActionResult httpActionResult = depositController.CreateDepositAddress(new GenerateAddressParams(1, "LTC"));
            OkNegotiatedContentResult<DepositAddressRepresentation> response = 
                (OkNegotiatedContentResult<DepositAddressRepresentation>) httpActionResult;
            Assert.IsNotNull(response);
            Assert.IsTrue(!string.IsNullOrEmpty(response.Content.Address));
            Assert.AreEqual(AddressStatus.New.ToString(), response.Content.Status);

            DepositAddress depositAddress = depositAddressRepository.GetDepositAddressByAddress(new BitcoinAddress(response.Content.Address));
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.New, depositAddress.Status);
        }

        [Test]
        [Category("Integration")]
        public void DepositTest_NewAddressMustBeGenerated_VerifiesThroughDatabaseQueryAndReturnValue()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            Assert.IsNotNull(depositController);

            string apiKey = "apiKey";
            depositController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            depositController.Request.Headers.Add("Auth", apiKey);
            IHttpActionResult httpActionResult = depositController.CreateDepositAddress(new GenerateAddressParams(1, "LTC"));
            OkNegotiatedContentResult<DepositAddressRepresentation> response =
                (OkNegotiatedContentResult<DepositAddressRepresentation>)httpActionResult;
            Assert.IsNotNull(response);
            Assert.IsTrue(!string.IsNullOrEmpty(response.Content.Address));
            Assert.AreEqual(AddressStatus.New.ToString(), response.Content.Status);

            DepositAddress depositAddress = depositAddressRepository.GetDepositAddressByAddress(new BitcoinAddress(response.Content.Address));
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.New, depositAddress.Status);

            httpActionResult = depositController.GetDepositAddresses(new GetDepositAddressesParams("LTC"));
            OkNegotiatedContentResult<IList<DepositAddressRepresentation>> depositAddresses =
                (OkNegotiatedContentResult<IList<DepositAddressRepresentation>>) httpActionResult;
            Assert.IsNotNull(depositAddresses.Content);
            Assert.Greater(depositAddresses.Content.Count, 0);
            Assert.AreEqual(response.Content.Address, depositAddresses.Content[0].Address);
            Assert.AreEqual(response.Content.Status, depositAddresses.Content[0].Status);
        }
    }
}
