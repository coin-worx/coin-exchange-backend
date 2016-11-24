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
using System.Security.Authentication;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    public class RegistrationApplicationServiceIntegrationTests
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
        public void InjectionTest_TestsWhetherSpringInitiatesAsExpectedAndInitializesRegistrationService_FailsIfNotInitialized()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            Assert.IsNotNull(registrationService);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewAccountTest_ChecksWhetherTheUserIsCreatedAndSavedToPersistence_VerifiesThroughUsingDatabaseresult()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("waqasshah047@gmail.com", receivedUser.Email);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);
            Assert.IsFalse(receivedUser.IsActivationKeyUsed.Value);
            Assert.IsFalse(receivedUser.IsUserBlocked.Value);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegistrationTwiceFailTest_ChecksThatAccountWithSameUsernameIsNotSavedAgain_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("waqasshah047@gmail.com", receivedUser.Email);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);

            registrationService.CreateAccount(new SignupUserCommand(
                "waqas.syed1@abcdefg.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            User userByEmail = userRepository.GetUserByEmail("waqas.syed1@abcdefg.com");
            Assert.IsNull(userByEmail);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegistrationTwiceFailTest_ChecksThatAccountWithSameUsernameAndEmailIsNotSavedAgain_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("waqasshah047@gmail.com", receivedUser.Email);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);

            registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            User userByEmail = userRepository.GetUserByEmail("waqasshah047@gmail.com");
            Assert.IsNotNull(userByEmail);
            Assert.AreEqual("Bob", userByEmail.Username);

            User userByUsername = userRepository.GetUserByUserName("Bob");
            Assert.IsNull(userByUsername);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegistrationTwiceFailTest_ChecksThatAccountWithSameEmailIsNotSavedAgain_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("waqasshah047@gmail.com", receivedUser.Email);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);

            registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Boby", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            User userByUsername = userRepository.GetUserByUserName("Boby");
            Assert.IsNull(userByUsername);

            User userByEmail = userRepository.GetUserByEmail("waqasshah047@gmail.com");
            Assert.IsNotNull(userByEmail);
            Assert.AreEqual("Bob", userByEmail.Username);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void RegistrationFailTest_ChecksThatRegistrationFailsWithBlankEmail_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNull(activationKey);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void RegistrationFailTest_ChecksThatRegistrationFailsWithBlankUsername_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNull(activationKey);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void RegistrationFailTest_ChecksThatRegistrationFailsWithBlankPassword_VerifiesExpectingException()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "", "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNull(activationKey);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void CreateNewAccountFailTest_CredentialsNotGivenAndDatabaseSaveShouldNotHappen_VerifiesThroughUsingDatabaseResult()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            
            registrationService.CreateAccount(new SignupUserCommand("", "", "", "Wonderland", TimeZone.CurrentTimeZone, ""));
        }

        [Test]
        [Category("Integration")]
        public void CreateAccount_IfAccountIsCreatedSuccessfully_AllTierLevelsShouldBeAssignedAndAreNonVerified()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            ITierRepository tierRepository =
                (ITierRepository)_applicationContext["TierRepository"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqasshah047@gmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            IList<Tier> tiers = tierRepository.GetAllTierLevels();
            for (int i = 0; i < tiers.Count; i++)
            {
                Assert.AreEqual(receivedUser.GetTierLevelStatus(tiers[i]),Status.NonVerified.ToString());
            }
        }
    }
}
