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
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    public class LoginApplicationServiceUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        [Category("Unit")]
        public void LoginSuccessfulTest_ChecksIfTheSecurityKeysAreProperlyReturnedWhileLoggingIn_VerifiesTheReturnedKeysToConfirm()
        {
            IUserRepository userRepository = new MockUserRepository();
            IIdentityAccessPersistenceRepository persistRepository = new MockPersistenceRepository(false);
            ISecurityKeysApplicationService securityKeysApplicationService = new SecurityKeysApplicationService(new SecurityKeysGenerationService(),
                persistRepository,null,null);
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IMfaAuthorizationService mockMfaAuthorizationService = new MockMfaAuthorizationService();
            ILoginApplicationService loginApplicationService = new LoginApplicationService(userRepository, passwordEncryptionService, 
                securityKeysApplicationService, new MockPersistenceRepository(false), mockMfaAuthorizationService);

            string enteredPassword = "whereistheJoker";
            User user = new User("batman@gotham.com", "brucewayne", passwordEncryptionService.EncryptPassword(enteredPassword),
                "Ninja County", TimeZone.CurrentTimeZone, "", "");
            user.AutoLogout = new TimeSpan(0, 0, 0, 60);
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            // Add this user to the MockUserRepository
            (userRepository as MockUserRepository).AddUser(user);
            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(
                new LoginCommand("brucewayne", enteredPassword));

            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.AreEqual(userValidationEssentials.SessionLogoutTime, user.AutoLogout);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginfailDueToIncorrectUsernameTest_MakesSureLoginFailsInCaseOfWrongUsername_VerifiesTheReturnedNullResultToConfirm()
        {
            IUserRepository userRepository = new MockUserRepository();
            IIdentityAccessPersistenceRepository persistRepository = new MockPersistenceRepository(false);
            ISecurityKeysApplicationService securityKeysApplicationService = new SecurityKeysApplicationService(new SecurityKeysGenerationService(),
                persistRepository,null,null);
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IMfaAuthorizationService mockMfaAuthorizationService = new MockMfaAuthorizationService();
            ILoginApplicationService loginApplicationService = new LoginApplicationService(userRepository, passwordEncryptionService,
                securityKeysApplicationService, new MockPersistenceRepository(false), mockMfaAuthorizationService);

            string enteredPassword = "whereistheJoker";
            User user = new User("batman@gotham.com", "brucewayne", passwordEncryptionService.EncryptPassword(enteredPassword),
                "Ninja County", TimeZone.CurrentTimeZone, "", "");
            user.AutoLogout = new TimeSpan(0, 0, 0, 60);
            // Add this user to the MockUserRepository
            (userRepository as MockUserRepository).AddUser(user);

            loginApplicationService.Login(new LoginCommand("alfred", "whereisthejoker"));            
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailDueToBlankUsernameTest_MakesSureLoginFailsInCaseOfBlankUsername_VerifiesTheReturnedNullResultToConfirm()
        {
            IUserRepository userRepository = new MockUserRepository();
            IIdentityAccessPersistenceRepository persistRepository = new MockPersistenceRepository(false);
            ISecurityKeysApplicationService securityKeysApplicationService =
                new SecurityKeysApplicationService(new SecurityKeysGenerationService(),
                                                   persistRepository, null,null);
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IMfaAuthorizationService mockMfaAuthorizationService = new MockMfaAuthorizationService();
            ILoginApplicationService loginApplicationService = new LoginApplicationService(userRepository, passwordEncryptionService,
                securityKeysApplicationService, new MockPersistenceRepository(false), mockMfaAuthorizationService);

            string enteredPassword = "whereistheJoker";
            User user = new User("batman@gotham.com", "brucewayne",
                                 passwordEncryptionService.EncryptPassword(enteredPassword),
                                 "Ninja County", TimeZone.CurrentTimeZone, "", "");
            user.AutoLogout = new TimeSpan(0, 0, 0, 60);
            // Add this user to the MockUserRepository
            (userRepository as MockUserRepository).AddUser(user);

            loginApplicationService.Login(new LoginCommand("", "whereisthejoker"));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginfailDueToIncorrectPasswordTest_MakesSureLoginFailsInCaseOfWrongPassword_VerifiesTheReturnedNullResultToConfirm()
        {
            IUserRepository userRepository = new MockUserRepository();
            IIdentityAccessPersistenceRepository persistRepository = new MockPersistenceRepository(false);
            ISecurityKeysApplicationService securityKeysApplicationService = new SecurityKeysApplicationService(new SecurityKeysGenerationService(),
                persistRepository,null,null);
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IMfaAuthorizationService mockMfaAuthorizationService = new MockMfaAuthorizationService();
            ILoginApplicationService loginApplicationService = new LoginApplicationService(userRepository, passwordEncryptionService,
                securityKeysApplicationService, new MockPersistenceRepository(false), mockMfaAuthorizationService);
            string enteredPassword = "whereistheJoker";
            User user = new User("batman@gotham.com", "brucewayne", passwordEncryptionService.EncryptPassword(enteredPassword),
                "Ninja County", TimeZone.CurrentTimeZone, "", "");
            user.AutoLogout = new TimeSpan(0, 0, 0, 60);
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            // Add this user to the MockUserRepository
            (userRepository as MockUserRepository).AddUser(user);

            loginApplicationService.Login(new LoginCommand("brucewayne", "whereisthejoke"));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailDueToBlankPasswordTest_MakesSureLoginFailsInCaseOfBlankPassword_VerifiesTheReturnedNullResultToConfirm()
        {
            IUserRepository userRepository = new MockUserRepository();
            IIdentityAccessPersistenceRepository persistRepository = new MockPersistenceRepository(false);
            ISecurityKeysApplicationService securityKeysApplicationService = new SecurityKeysApplicationService(new SecurityKeysGenerationService(),
                persistRepository, null,null);
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IMfaAuthorizationService mockMfaAuthorizationService = new MockMfaAuthorizationService();
            ILoginApplicationService loginApplicationService = new LoginApplicationService(userRepository, passwordEncryptionService,
                securityKeysApplicationService, new MockPersistenceRepository(false), mockMfaAuthorizationService);
            string enteredPassword = "whereistheJoker";
            User user = new User("batman@gotham.com", "brucewayne", passwordEncryptionService.EncryptPassword(enteredPassword),
                "Ninja County", TimeZone.CurrentTimeZone, "", "");
            user.AutoLogout = new TimeSpan(0, 0, 0, 60);
            // Add this user to the MockUserRepository
            (userRepository as MockUserRepository).AddUser(user);

            loginApplicationService.Login(new LoginCommand("brucewayne", ""));
        }
    }
}
