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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.MfaServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    internal class MfaAuthorizationServiceIntegrationTests
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
        public void TearDown()
        {
            ContextRegistry.Clear();
            _databaseUtility.Create();
        }

        [Test]
        [Category("Integration")]
        public void ServiceInitilizationTest_ChecksIfTheServiceInstanceIsInitializedThroughSpringAsExpected_VerifiesThroughVariableValue()
        {
            IMfaAuthorizationService mfaAuthorizationService = (IMfaAuthorizationService) ContextRegistry.GetContext()["MfaAuthorizationService"];
            Assert.IsNotNull(mfaAuthorizationService);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationSuccessfulTest_ChecksThatServiceReturnsTrueIfNoMfaSubscriptionIsSubscribed_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationFailTest_ChecksThatServiceReturnsFalseIfMfaCodeIsNotProvidedAfterSubscription_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";

            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            IList<Tuple<string, string, bool>> mfaSubscriptions = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                mfaSubscriptions.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId, 
                    subscription.MfaSubscriptionName, true));
            }
            user.AssignMfaSubscriptions(mfaSubscriptions);
            persistenceRepository.SaveUpdate(user);

            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, null);
            Assert.IsFalse(authorizeAccess.Item1);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationSuccessfulTest_ChecksThatServiceReturnsTrueIfMfaCodeIsProvidedAfterSubscription_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";

            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            IList<Tuple<string, string, bool>> mfaSubscriptions = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                mfaSubscriptions.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId,
                    subscription.MfaSubscriptionName, true));
            }
            user.AssignMfaSubscriptions(mfaSubscriptions);
            persistenceRepository.SaveUpdate(user);

            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, null);
            Assert.IsFalse(authorizeAccess.Item1);

            // The Stub Implementation always generates and returns the same MFA Code, so we can use that to our convenience
            string mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, mfaCode);
            Assert.IsTrue(authorizeAccess.Item1);

            // Withdraw Mfa Code Verfification
            mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Withdrawal, mfaCode);
            Assert.IsTrue(authorizeAccess.Item1);

            // Login Mfa Code Verfification
            mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Login, mfaCode);
            Assert.IsTrue(authorizeAccess.Item1);

            // Place Order Mfa Code Verfification
            mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.PlaceOrder, mfaCode);
            Assert.IsTrue(authorizeAccess.Item1);

            // Cancel order Mfa Code Verfification
            mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.CancelOrder, mfaCode);
            Assert.IsTrue(authorizeAccess.Item1);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationFailTest_ChecksThatServiceReturnsTrueIfMfaCodesDontMatch_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";

            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            IList<Tuple<string, string, bool>> mfaSubscriptions = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                mfaSubscriptions.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId,
                    subscription.MfaSubscriptionName, true));
            }
            user.AssignMfaSubscriptions(mfaSubscriptions);
            persistenceRepository.SaveUpdate(user);

            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, null);
            Assert.IsFalse(authorizeAccess.Item1);

            // The Stub Implementation always generates and returns the same MFA Code. We manuipulate it so that the code is 
            // incorrect
            string mfaCode = mfaCodeGenerationService.GenerateCode();
            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, mfaCode + "1");
            Assert.IsFalse(authorizeAccess.Item1);
        }
    }
}