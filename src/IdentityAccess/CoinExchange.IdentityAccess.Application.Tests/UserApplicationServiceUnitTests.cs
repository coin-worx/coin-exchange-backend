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
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    class UserApplicationServiceUnitTests
    {
        #region Change Password Tests

        [Test]
        [Category("Unit")]
        public void ChangePasswordSuccessTest_ChecksIfThePasswordIsChangedSuccessfully_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(new User("linkinpark@rock.com", "linkinpark",
                passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", ""));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            //UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
            //    new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0,0,10,0));

            ChangePasswordResponse changePasswordResponse = userApplicationService.ChangePassword(new ChangePasswordCommand(
                "123456789", "burnitdown", "burnitdowntwice"));

            Assert.IsTrue(changePasswordResponse.ChangeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ChangePasswordFailTest_ChecksThePasswordDoesNotGetChangedWhenInvalidOldPasswordIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(new User("linkinpark@rock.com", "linkinpark",
                passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", ""));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            //UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
            //    new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 100));

            // Wrong password given
            userApplicationService.ChangePassword(new ChangePasswordCommand("123456789", "burnitdow", "burnitdowntwice"));

            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Unit")]
        //[ExpectedException(typeof(Exception))]
        public void ChangePasswordFailDueToSessionTimeoutTest_ChecksThePasswordDoesNotGetChangedWhenSessionTimeoutHasExpired_VerifiesByExpectingException()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            var user = new User("linkinpark@rock.com", "linkinpark", passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", "");
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            //UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
            //    new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 0, 1));
            (userRepository as MockUserRepository).DeleteUser(user);
            user.AutoLogout = new TimeSpan(0, 0, 0, 0, 1);            
            (userRepository as MockUserRepository).AddUser(user);

                // Wrong password given
            userApplicationService.ChangePassword(new ChangePasswordCommand("123456789", "burnitdown",
                                                      "burnitdowntwice"));           
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        #endregion Change Password Tests

        #region Activate Account Tests

        [Test]
        [Category("Unit")]
        public void ActivateAccountSuccessTest_ChecksIfTheAccountIsActivatedSuccessfully_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            bool changeSuccessful = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));

            Assert.IsTrue(changeSuccessful);
            User user1 = (persistenceRepository as MockPersistenceRepository).GetUser(username);
            Assert.IsNotNull(user1);
            Assert.IsTrue(user1.IsActivationKeyUsed.Value);
            Assert.IsFalse(user1.IsUserBlocked.Value);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ActivateAccountTwiceFailTest_ChecksIfTheAccountIsNotActivatedTwice_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            bool changeSuccessful = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));

            Assert.IsTrue(changeSuccessful);
            User user1 = (persistenceRepository as MockPersistenceRepository).GetUser(username);
            Assert.IsNotNull(user1);
            Assert.IsTrue(user1.IsActivationKeyUsed.Value);
            Assert.IsFalse(user1.IsUserBlocked.Value);
            // This time, account should not be activated
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ActivateAccountFailDueToInvalidActivationKeyTest_MakesSureThatTheAccountIsNotActivatedWhenInvalidActivationKeyIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey + "1", username, password));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankActivationKeyTest_MakesSureThatTheAccountIsNotActivatedWhenBlankActivationKeyIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand("", username, password));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToInvalidUsernameTest_MakesSureThatTheAccountIsNotActivatedWhenInvalidUsernameIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username + "r", password));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankUsernameTest_MakesSureThatTheAccountIsNotActivatedWhenBlankUsernameIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "", password));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToInvalidPasswordTest_MakesSureThatTheAccountIsNotActivatedWhenInvalidPasswordIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password + "pass"));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankPasswordTest_MakesSureThatTheAccountIsNotActivatedWhenBlankPasswordIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", 0, true));

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username,
                                 passwordEncryptionService.EncryptPassword(password), "USA", TimeZone.CurrentTimeZone,
                                 "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password + "pass"));
        }

        #endregion Activate Account Tests

        #region Cancel Account Activation Tests

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelAccountActivationFailedDueToInvalidActivationKey_MakesSureAccountActivationDoesNotGetCancelledWhenInvfalidActivationKeyIsGiven_VerifiesByExpectingException()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username, passwordEncryptionService.EncryptPassword(password),
                                 "USA", TimeZone.CurrentTimeZone, "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.CancelAccountActivation(new CancelActivationCommand(activationKey + "2"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void CancelAccountActivationFailedDueToBlankActivationKey_MakesSureAccountActivationDoesNotGetCancelledWhenBlankActivationKeyIsGiven_VerifiesByExpectingException()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username, passwordEncryptionService.EncryptPassword(password),
                                 "USA", TimeZone.CurrentTimeZone, "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            userApplicationService.CancelAccountActivation(new CancelActivationCommand(""));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelAccountActivationFailedBecasueNoSuchAccountExists_MakesSureTHisDoesntCreateAnyBreach_VerifiesByExpectingException()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            userApplicationService.CancelAccountActivation(new CancelActivationCommand("123ffdsdsaewr43212"));
        }

        [Test]
        [Category("Integration")]
        public void CancelAccountActivationSuccessfulTest_MakesSureAccountActivationGetsCancelledWhenEverythingIsGivenAsExpected_VerifiesByReturnedValueAndQueryingRepository()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository, new MockEmailService(), new PasswordCodeGenerationService());

            string activationKey = "123456789";
            string username = "linkinpark";
            string password = "burnitdown";
            User user = new User("linkinpark@rock.com", username, passwordEncryptionService.EncryptPassword(password),
                                 "USA", TimeZone.CurrentTimeZone, "", activationKey);
            user.AddTierStatus(Status.NonVerified, new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0));
            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(user);

            bool accountActivationCancelled = userApplicationService.CancelAccountActivation(new CancelActivationCommand(activationKey));
            Assert.IsTrue(accountActivationCancelled);

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNull(userByUserName);
        }

        #endregion Cancel Account Activation Tests
    }
}
