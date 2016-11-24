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
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Application.DepositServices;
using CoinExchange.Funds.Application.LedgerServices;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class FundsApplicationServicesInitializationTests
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
        public void DepositApplicationServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            Assert.IsNotNull(depositApplicationService);
        }

        [Test]
        public void WithdrawApplicationServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            Assert.IsNotNull(withdrawApplicationService);
        }

        [Test]
        public void FundsValidationServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            Assert.IsNotNull(fundsValidationService);
        }

        [Test]
        public void TransactionServiceServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            Assert.IsNotNull(transactionService);
        }

        [Test]
        public void ClientInteractionServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IClientInteractionService clientInteractionService = (IClientInteractionService)ContextRegistry.GetContext()["ClientInteractionService"];
            Assert.IsNotNull(clientInteractionService);
        }

        [Test]
        public void LedgerQueryServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            ILedgerQueryService ledgerQueryService = (ILedgerQueryService)ContextRegistry.GetContext()["LedgerQueryService"];
            Assert.IsNotNull(ledgerQueryService);
        }
    }
}
