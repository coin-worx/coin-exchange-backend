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
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    class LogoutControllerTests
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
        public void LogoutSuccessfulTest_MakesSureUserLogsOutAndTheSecurityKeysAreDeletedFromDatabase_VerifiesAndAssertsTheReturnedValueAndQueriesDatabase()
        {
            // Register User
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Verify that Security Keys are in the database
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(keys.Content.SecretKey, securityKeysPair.SecretKey);
            Assert.IsTrue(securityKeysPair.SystemGenerated);

            LogoutController logoutController = (LogoutController)_applicationContext["LogoutController"];
            logoutController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            logoutController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult logoutResult = logoutController.Logout();
            OkNegotiatedContentResult<bool> logoutOkResponse = (OkNegotiatedContentResult<bool>) logoutResult;
            Assert.IsNotNull(logoutOkResponse);
            Assert.IsTrue(logoutOkResponse.Content);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNull(securityKeysPair);
        }

        [Test]
        [Category("Integration")]
        public void LogoutFailTest_ChecksThatExceptionIsThrownWhenInvalidActivationKeyIsGiven_VerifiesAndAssertsTheReturnedValueAndQueriesDatabase()
        {
            // Register User
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Verify that Security Keys are in the database
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(keys.Content.SecretKey, securityKeysPair.SecretKey);
            Assert.IsTrue(securityKeysPair.SystemGenerated);

            LogoutController logoutController = (LogoutController)_applicationContext["LogoutController"];
            logoutController.Request = new HttpRequestMessage(HttpMethod.Get, "");
            logoutController.Request.Headers.Add("Auth", "123");
            IHttpActionResult logoutResult = logoutController.Logout();
            BadRequestErrorMessageResult logoutOkResponse = (BadRequestErrorMessageResult)logoutResult;
            Assert.IsNotNull(logoutOkResponse);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(keys.Content.SecretKey, securityKeysPair.SecretKey);
            Assert.IsTrue(securityKeysPair.SystemGenerated);
        }

        [Test]
        [Category("Integration")]
        public void LogoutSuccessfulThenFailTest_ChecksThatUserLogsInThenLogsOutAndThenTriesTologoutAgainUsingTheSameApiKeyThenExceptionShouldBeThrown_VerifiesAndAssertsTheReturnedValueAndQueriesDatabase()
        {
            // Register User
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Verify that Security Keys are in the database
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(keys.Content.SecretKey, securityKeysPair.SecretKey);
            Assert.IsTrue(securityKeysPair.SystemGenerated);

            // Logout
            LogoutController logoutController = (LogoutController)_applicationContext["LogoutController"];
            logoutController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            logoutController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult logoutResult = logoutController.Logout();
            OkNegotiatedContentResult<bool> logoutOkResponse = (OkNegotiatedContentResult<bool>)logoutResult;
            Assert.IsNotNull(logoutOkResponse);
            Assert.IsTrue(logoutOkResponse.Content);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNull(securityKeysPair);
            logoutController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            logoutController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            // Invalid Logout as the user has logged out already
            logoutResult = logoutController.Logout();
            BadRequestErrorMessageResult logoutBadResponse = (BadRequestErrorMessageResult)logoutResult;
            Assert.IsNotNull(logoutBadResponse);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNull(securityKeysPair);
        }
    }
}
