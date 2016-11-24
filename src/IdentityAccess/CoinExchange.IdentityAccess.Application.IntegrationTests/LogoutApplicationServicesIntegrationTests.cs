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
using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class LogoutApplicationServicesIntegrationTests
    {
        private IApplicationContext _applicationContext;
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            _applicationContext = ContextRegistry.GetContext();
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
        public void LogoutServiceInitializationAndInjectiontest_ChecksIfTheServiceGetsInitializedUsingSpring_FailsIfNot()
        {
            ILogoutApplicationService logoutApplicationService = (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            Assert.IsNotNull(logoutApplicationService);
        }

        [Test]
        [Category("Integration")]
        public void LogoutSuccessTest_TestsIfAUserGetsLogoutAsExpected_FailsIfDoesNot()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            // Register
            string username = "Bob";
            string password = "alice";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", username, password, "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNotNull(activationKey);

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            // Activate account
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));
            Assert.IsTrue(accountActivated);
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            // Login
            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand(username, password));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            // Logout
            ILogoutApplicationService logoutApplicationService = 
                (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            Assert.IsNotNull(logoutApplicationService);
            bool logout = logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));
            Assert.IsTrue(logout);

            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey);
            Assert.IsNull(securityKeysPair);
        }
    }
}
