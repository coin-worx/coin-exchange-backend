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
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    public class RegistrationControllerTests
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
        public void RegisterAccountTest_IfRegisterControllerIsCalled_SignUpShouldBeDoneSuccessfullyAndActivationKeyShouldBeReceived()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult=registrationController.Register(new SignUpParam("user@user.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);
        }

        [Test]
        [Category("Integration")]
        public void RegisterAccountSuccessfulTest_ChecksIfTheUserCreatesAnAccountSuccessfuly_VerifiesRetunredvalueAndQueriesDatabase()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string username = "user";
            string email = "user@abcdefg.com";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username, "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
            Assert.AreEqual(username, userByUserName.Username);
            Assert.AreEqual(email, userByUserName.Email);
        }

        [Test]
        [Category("Integration")]
        public void RegisterAccountFailTest_ChecksIfTheUserCannotCreateAccountAgainWIthSameUsername_VerifiesReturnedvalueAndQueriesDatabase()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string username = "user";
            string email = "user@abcdefg.com";
            string password = "123";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username, password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
            Assert.AreEqual(username, userByUserName.Username);
            Assert.AreEqual(email, userByUserName.Email);

            string email2 = "newemail@abcdefg.com";
            string password2 = "newPassword";
            httpActionResult = registrationController.Register(new SignUpParam(email2, username, password2, "Pakistan", TimeZone.CurrentTimeZone, ""));
            BadRequestErrorMessageResult badRequest = (BadRequestErrorMessageResult)httpActionResult;

            Assert.IsNotNull(badRequest);

            // Verify that the old credentials are still the same
            userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
            Assert.AreEqual(username, userByUserName.Username);
            Assert.AreEqual(email, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));

            Assert.AreNotEqual(email2, userByUserName.Email);
            Assert.IsFalse(passwordEncryptionService.VerifyPassword(password2, userByUserName.Password));
        }

        [Test]
        [Category("Integration")]
        public void RegisterAccountFailTest_ChecksIfTheUserCannotCreateAccountAgainWIthSameEmail_VerifiesReturnedValueAndQueriesDatabase()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string username = "user";
            string email = "user@abcdefg.com";
            string password = "123";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username, password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
            Assert.AreEqual(username, userByUserName.Username);
            Assert.AreEqual(email, userByUserName.Email);

            string username2 = "newUser";
            string password2 = "newPassword";
            httpActionResult = registrationController.Register(new SignUpParam(email, username2, password2, "Pakistan", TimeZone.CurrentTimeZone, ""));
            BadRequestErrorMessageResult badRequest = (BadRequestErrorMessageResult)httpActionResult;

            Assert.IsNotNull(badRequest);

            // Verify that the old credentials are still the same
            userByUserName = userRepository.GetUserByEmail(email);
            Assert.IsNotNull(userByUserName);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
            Assert.AreEqual(username, userByUserName.Username);
            Assert.AreEqual(email, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));

            Assert.AreNotEqual(username2, userByUserName.Username);
            Assert.IsFalse(passwordEncryptionService.VerifyPassword(password2, userByUserName.Password));
        }
    }
}
