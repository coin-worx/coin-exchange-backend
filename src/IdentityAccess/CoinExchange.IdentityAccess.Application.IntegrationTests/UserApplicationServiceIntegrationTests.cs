using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class UserApplicationServiceIntegrationTests
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

        #region Change Password Tests

        [Test]
        [Category("Integration")]
        public void ChangePasswordSuccessTest_ChecksIfThePasswordIsChangedSuccessfully_VerifiesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
           
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            
            string username = "linkinpark";
            string activatioNKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activatioNKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));
            
            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            bool changeSuccessful = userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdown",
                "burnitdowntwice"));

            Assert.IsTrue(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ChangePasswordFailTest_ChecksIfThePasswordIsNotChangedIfOldPasswordisIncorrect_VerifiesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            
            string username = "linkinpark";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdowner",
                                                      "burnitdowntwice"));            
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void ChangePasswordFailDueToInvalidApiKeyTest_ChecksIfExceptionIsRaisedAfterWrongApiKeyIsGiven_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            UserValidationEssentials validationEssentials2 = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey(validationEssentials.ApiKey.Value + "1"), validationEssentials.SecretKey), validationEssentials.SessionLogoutTime);
            // Give the wrong API Key
            userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials2, "burnitdowner",
                                                      "burnitdowntwice"));
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(Exception))]
        public void ChangePasswordFailDueToUserTimeoutVariableTest_ChecksIfExceptionIsRaisedAfterUsersTimeoutsFoundToBeDifferentThanTheGivenTimeout_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            IIdentityAccessPersistenceRepository persistenceRepository =
                (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;
            User userByUserName = userRepository.GetUserByUserName(username);
            // When the User's Logout time and ValidationEssentials Logout time won't match, test will fail
            userByUserName.AutoLogout = new TimeSpan(0,0,0,67);
            persistenceRepository.SaveUpdate(userByUserName);
            // Give the wrong API Key
            userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdowner",
                                                      "burnitdowntwice"));
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(Exception))]
        public void ChangePasswordFailDueToGivenTimeoutMismatchTest_ChecksIfExceptionIsRaisedAfterGivenTimeoutsFoundToBeDifferentThanTheUserTimeout_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            IIdentityAccessPersistenceRepository persistenceRepository =
                (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;
            UserValidationEssentials validationEssentials2 = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                validationEssentials.ApiKey, validationEssentials.SecretKey), new TimeSpan(0, 0, 67, 0));
            // Give the wrong API Key
            userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials2, "burnitdowner",
                                                      "burnitdowntwice"));
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }


        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(Exception))]
        public void ChangePasswordFailDueToSessionTimeoutTest_ChecksIfExceptionIsRaisedAfterSessionTimeout_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            IIdentityAccessPersistenceRepository persistenceRepository =
                (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;
            UserValidationEssentials validationEssentials2 = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                validationEssentials.ApiKey, validationEssentials.SecretKey), new TimeSpan(0, 0, 0, 0, 1));
            User userByUserName = userRepository.GetUserByUserName(username);
            // When the User's Logout time and ValidationEssentials Logout time won't match, test will fail
            userByUserName.AutoLogout = new TimeSpan(0, 0, 0, 0, 1);
            persistenceRepository.SaveUpdate(userByUserName);
            // Give the wrong API Key
            userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials2, "burnitdown",
                                                      "burnitdowntwice"));
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        #endregion Change Passwords Tests

        #region Forgot Username Tests

        [Test]
        [Category("Integration")]
        public void ForgotUsernameRequestSuccessTest_ProvidesAValidEmailIdToSendTheMailTo_VerifiesByTheReturnedBoolean()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            // Wait for the asynchrnous email sending event to be completed otherwise no more emails can be sent until
            // this operation finishes
            manualResetEvent.WaitOne(6000);
            
            string returnedUsername = userApplicationService.ForgotUsername(new ForgotUsernameCommand(email));
            // Wait for the email to be sent and operation to be completed
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(username, returnedUsername);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForgotUsernameRequestFailTest_ProvidesAnInvalidEmailIdToSendTheMailTo_VerifiesByTheReturnedBoolean()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);

            userApplicationService.ForgotUsername(new ForgotUsernameCommand(email + "1"));
        }

        #endregion Forgot Username Tests

        #region Forgot Password Tests

        [Test]
        [Category("Integration")]
        public void ForgotPasswordSuccessfultest_TestsIfForgotPasswordReqeustIsSentSuccessfullyAndForgotPasswordCodeUpdatedInUserProperly_VerifiesThroughReturnValues()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);

            string returnedPasswordCode = userApplicationService.ForgotPassword(new ForgotPasswordCommand(email, username));
            // Wait for the email to be sent and operation to be completed
            manualResetEvent.WaitOne(5000);
            Assert.IsNotNull(returnedPasswordCode);

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(returnedPasswordCode, userByUserName.ForgotPasswordCode);
            Assert.AreEqual(1, userByUserName.ForgottenPasswordCodes.Length);
            Assert.AreEqual(DateTime.Now.Hour + 2, userByUserName.ForgotPasswordCodeExpiration.Value.Hour);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ForgotPasswordFailDueToInvalidUsernameTest_MakesSureOperationIsAbortedWhenUsernameIsInvalid_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);

            userApplicationService.ForgotPassword(new ForgotPasswordCommand(email, username + "1"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForgotPasswordFailDueToInvalidEmailTest_MakesSureOperationIsAbortedWhenEmailIsInvalid_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            manualResetEvent.WaitOne(6000);

            userApplicationService.ForgotPassword(new ForgotPasswordCommand(email + "1", username));
        }

        #endregion Forgot Password Tests

        #region Activate Account Tests

        [Test]
        [Category("Integration")]
        public void ActivateAccountSuccessTest_ChecksIfTheAccountIsActivatedSuccessfully_VeririesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));

            Assert.IsTrue(accountActivated);
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(Exception))]
        public void ActivateAccountFailWhenTriedTwiceTest_ChecksIfTheAccountIsNotActivatedAgainOnceActivated_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));

            Assert.IsTrue(accountActivated);
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void ActivateAccountFailDueToInvalidActivationTest_ChecksIfTheAccountActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey + "activate", username, password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankActivationTest_ChecksIfTheAccountActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand("", username, password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToInvalidUsernameTest_ChecksIfTheAccountActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username + "user", password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankUsernameTest_ChecksIfTheAccounActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "", password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToInvalidPasswordTest_ChecksIfTheAccounActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password+"pass"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void ActivateAccountFailDueToBlankPasswordTest_ChecksIfTheAccounActivationFails_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, ""));
        }

        #endregion Activate Account Tests

        #region Cancel Account Activation Tests

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelAccountActivationFailedDueToInvalidActivationKey_MakesSureAccountActivationDoesNotGetCancelledWhenInvfalidActivationKeyIsGiven_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            userApplicationService.CancelAccountActivation(new CancelActivationCommand(activationKey + "1"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelAccountActivationNoAccountExistsFail_MakesSureAccountActivationDoesNotGetCancelledWhenNoSuchAccountIsPresent_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            
            userApplicationService.CancelAccountActivation(new CancelActivationCommand("1eew355ygf43rdwq1"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelActivationAfterAccountActivatedFailTest_ChecksIfTheAccountActivationIsNotCancelledAfterAccountHasBeenActivated_VerifiesByExpectingException()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));
            Assert.IsTrue(accountActivated);
            User userBeforeCancel = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userBeforeCancel);
            Assert.IsTrue(userBeforeCancel.IsActivationKeyUsed.Value);
            // Exception is raised here
            userApplicationService.CancelAccountActivation(new CancelActivationCommand(activationKey));

            User userAfterCancel = userRepository.GetUserByUserName(username);
            Assert.IsNull(userAfterCancel);
            Assert.IsTrue(userBeforeCancel.IsActivationKeyUsed.Value);
        }

        [Test]
        [Category("Integration")]
        public void CancelAccountActivationSuccessfulTest_ChecksIfTheAccountActivationIsCancelledAsExpectedWhenEverythingIsInOrder_VerifiesByQueryingTheDatabase()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));
            bool cancelAccountActivation = userApplicationService.CancelAccountActivation(new CancelActivationCommand(activationKey));
            Assert.IsTrue(cancelAccountActivation);

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNull(userByUserName);
        }

        #endregion Cancel Account Activation Tests

        #region Reset Password Tests

        [Test]
        [Category("Integration")]
        public void ResetPasswordSuccessfultest_TestsIfPasswordIsResetProperly_VerifiesThroughReturnValues()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            Assert.IsTrue(accountActivated);
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(6000);
            string returnedPasswordCode = userApplicationService.ForgotPassword(new ForgotPasswordCommand(email, username));

            Assert.IsNotNull(returnedPasswordCode);
            string newPassword = "newpassword";

            bool resetPasswordReponse = userApplicationService.ResetPasswordByEmailLink(new ResetPasswordCommand(username, newPassword));
            Assert.IsTrue(resetPasswordReponse);

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(passwordEncryption.VerifyPassword(newPassword, userByUserName.Password));
        }

        [Test]
        [Category("Integration")]
        public void ResetPasswordSuccessfultest_TestsIfPasswordIsResetProperly_VerifiesForgotPasswordCodeValues()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, "burnitdown"));
            Assert.IsTrue(accountActivated);
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(6000);
            string returnedPasswordCode = userApplicationService.ForgotPassword(new ForgotPasswordCommand(email, username));

            Assert.IsNotNull(returnedPasswordCode);
            string newPassword = "newpassword";

            bool resetPasswordReponse = userApplicationService.ResetPasswordByEmailLink(new ResetPasswordCommand(username, newPassword));
            Assert.IsTrue(resetPasswordReponse);

            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(passwordEncryption.VerifyPassword(newPassword, userByUserName.Password));
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(1, userByUserName.ForgottenPasswordCodes.Length);
            Assert.IsTrue(userByUserName.ForgottenPasswordCodes[0].IsUsed);
        }

        #endregion Reset Password Tests
    }
}
