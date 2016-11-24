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
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    class SecurityKeysDatabasePersistenceTests
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

        [Test]
        [Category("Integration")]
        public void SecurityKeysMfaVerification_ChecksIfMfaSubscriptionsAreAddedAsExpected_VerifiesByQueryingTheDatabase()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];

            string apiKey = "123456";
            SecurityKeysPair originalPair = new SecurityKeysPair("1", apiKey, "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, false, null);
            persistenceRepository.SaveUpdate(originalPair);

            SecurityKeysPair retrievedPair = securityKeysRepository.GetByApiKey(apiKey);
            Assert.IsNotNull(retrievedPair);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            Assert.IsNotNull(allSubscriptions);
            Assert.AreEqual(5, allSubscriptions.Count);
            Assert.AreEqual("CancelOrder", allSubscriptions[0].MfaSubscriptionName);
            Assert.AreEqual("Deposit", allSubscriptions[1].MfaSubscriptionName);
            Assert.AreEqual("Login", allSubscriptions[2].MfaSubscriptionName);
            Assert.AreEqual("PlaceOrder", allSubscriptions[3].MfaSubscriptionName);
            Assert.AreEqual("Withdraw", allSubscriptions[4].MfaSubscriptionName);

            IList<Tuple<string, string, bool>> subscriptionsStringList = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                subscriptionsStringList.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId,
                    subscription.MfaSubscriptionName, true));
            }
            retrievedPair.AssignMfaSubscriptions(subscriptionsStringList);

            persistenceRepository.SaveUpdate(retrievedPair);

            retrievedPair = securityKeysRepository.GetByApiKey(apiKey);
            Assert.IsNotNull(retrievedPair);
        }
    }
}
