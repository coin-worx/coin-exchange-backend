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
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    public class RegistrationApplicationServiceUnitTests
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
        public void UserCreatedTest_ChecksIfTheRegistrationServiceCreatesAUserAsExpected()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            RegistrationApplicationService registrationApplicationService = new RegistrationApplicationService(persistenceRepository, 
                new PasswordEncryptionService(), new ActivationKeyGenerationService(), new MockEmailService(),
                new MockTierRepository(), new MockUserRepository());

            string activationKey = registrationApplicationService.CreateAccount(
                new SignupUserCommand("testdriven@agile.com", "iamnotmartinfowler", "butiamjohnskeet", "ProgrammingNation", 
                    TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);
            Assert.IsFalse(string.IsNullOrEmpty(activationKey));
        }

        [Test]
        [Category("Unit")]
        public void EmailNotProvidedTest_TestsIfNewUserIsNotCreatedWhenEmailIsNotGiven_ChecksActivationKeyisNotReturnedToConfirm()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            RegistrationApplicationService registrationApplicationService = new RegistrationApplicationService(persistenceRepository,
                new PasswordEncryptionService(), new ActivationKeyGenerationService(), new MockEmailService(),new MockTierRepository(),
                new MockUserRepository());
            bool exceptionRaised = false;
            try
            {
                registrationApplicationService.CreateAccount(
                    new SignupUserCommand("", "agilegeek", "iammartinfowler", "ProgrammingNation",
                                          TimeZone.CurrentTimeZone, ""));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }
            Assert.IsTrue(exceptionRaised);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void UsernameNotProvidedTest_TestsIfNewUserIsNotCreatedWhenUsernameIsNotGiven_ChecksActivationKeyisNotReturnedToConfirm()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            RegistrationApplicationService registrationApplicationService = new RegistrationApplicationService(persistenceRepository,
                new PasswordEncryptionService(), new ActivationKeyGenerationService(), new MockEmailService(),
                new MockTierRepository(), new MockUserRepository());
            registrationApplicationService.CreateAccount(
                new SignupUserCommand("testdriven@agile.com", null, "iammartinfowler", "ProgrammingNation",
                    TimeZone.CurrentTimeZone, ""));
        }

        [Test]
        [Category("Unit")]
        public void PasswordNotProvidedTest_TestsIfNewUserIsNotCreatedWhenPasswordIsNotGiven_ChecksActivationKeyisNotReturnedToConfirm()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            RegistrationApplicationService registrationApplicationService = new RegistrationApplicationService(persistenceRepository,
                new PasswordEncryptionService(), new ActivationKeyGenerationService(), new MockEmailService(),
                new MockTierRepository(), new MockUserRepository());

            bool exceptionRaised = false;
            try
            {
                registrationApplicationService.CreateAccount(
                    new SignupUserCommand("testdriven@agile.com", "iamnotmartinfowler", "", "ProgrammingNation",
                                          TimeZone.CurrentTimeZone, ""));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }
            Assert.IsTrue(exceptionRaised);
        }

        [Test]
        [Category("Unit")]
        public void DatabaseMockSaveFailTest_TestsIfUserIsNotSavedInDatabaseExceptionIsRaisedFromTheMockClass_HandlesTheExceptionToConfirm()
        {
            // Provide true to the Mock class  so that it raises exception when SaveUpdate method is called inside it
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(true);
            RegistrationApplicationService registrationApplicationService = new RegistrationApplicationService(persistenceRepository,
                new PasswordEncryptionService(), new ActivationKeyGenerationService(), new MockEmailService(),
                new MockTierRepository(), new MockUserRepository());

            bool exceptionRaised = false;
            try
            {
                string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(
                    "testdriven@agile.com", "iamnotmartinfowler", "butiamjacksparrow", "PirateNation", 
                    TimeZone.CurrentTimeZone, ""));
                Assert.IsNull(activationKey);
            }
            catch (Exception e)
            {
                exceptionRaised = true;
            }

            Assert.IsTrue(exceptionRaised);
        }
    }
}
