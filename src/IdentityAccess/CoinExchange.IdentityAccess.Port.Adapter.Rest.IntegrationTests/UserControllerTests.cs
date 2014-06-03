using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    public class UserControllerTests
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
        public void ActivateAccount_AfterRegisteringProvideActivationKey_UserShouldGetActivated()
        {
            RegistrationController registrationController =
                _applicationContext["RegistrationController"] as RegistrationController;
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("user@user.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

           UserController userController = _applicationContext["UserController"] as UserController;
           httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
           OkNegotiatedContentResult<bool> okResponseMessage1 =
               (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,true);
        }

        [Test]
        [Category("Integration")]
        public void Login()
        {
            RegistrationController registrationController =
                _applicationContext["RegistrationController"] as RegistrationController;
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("user@user.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            UserController userController = _applicationContext["UserController"] as UserController;
            httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, true);
        }

        [Test]
        [Category("Integration")]
        public void CancelActivationTest_MakesSureTheAccountActivaitonIsCancelledProvidedWithTheCorrectCredentials_VerifiesByReturnedValue()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "bruce@batmansgotham.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username, 
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.CancelUserActivation(new CancelActivationParams(activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Confirm that the User account has been deleted
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNull(userByUserName);
        }

        [Test]
        [Category("Integration")]
        public void ChangePasswordTest_MakesSureTheAccountActivaitonIsCancelledProvidedWithTheCorrectCredentials_VerifiesByTheReturnedValueAndQueryingDatabase()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "bruce@batmansgotham.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Confirm that the User has been activated
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            IHttpActionResult loginResult = loginController.Login(new LoginParams(username, password));
            OkNegotiatedContentResult<UserValidationEssentials> loginOkResult = (OkNegotiatedContentResult<UserValidationEssentials>)loginResult;
            UserValidationEssentials userValidationEssentials = loginOkResult.Content;
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            User userAfterLogin = userRepository.GetUserByUserName(username);
            Assert.AreEqual(userAfterLogin.AutoLogout, userValidationEssentials.SessionLogoutTime);

            // Wait for the account activated email async operation to complete before changing password which sends another email
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(6000);
            string newPassword = password + "123";
            // Change Password
            IHttpActionResult changePasswordResult = userController.ChangePassword(new ChangePasswordParams(
             userValidationEssentials.ApiKey.Value, userValidationEssentials.SecretKey.Value, password, newPassword));
            OkNegotiatedContentResult<bool> changePasswordOkResult = (OkNegotiatedContentResult<bool>)changePasswordResult;
            Assert.IsTrue(changePasswordOkResult.Content);

            User userAfterPasswordChange = userRepository.GetUserByUserName(username);
            Assert.AreNotEqual(userAfterLogin.Password, userAfterPasswordChange.Password);

            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(newPassword, userAfterPasswordChange.Password));
        }

        [Test]
        [Category("Integration")]
        public void ForgotUsernameTest_MakesSureUserIsRemindedOfTheTheirUsernameProperlyAfterActivatingAccount_VerifiesByTheReturnedValue()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Reqeust to remind for password
            IHttpActionResult forgotUsernameResponse = userController.ForgotUsername(new ForgotUsernameParams(email));
            OkNegotiatedContentResult<string> forgotUsernameOkResponse = (OkNegotiatedContentResult<string>) forgotUsernameResponse;
            // Check if the username returned is correct
            Assert.AreEqual(username, forgotUsernameOkResponse.Content);
        }

        [Test]
        [Category("Integration")] 
        public void ForgotUsernameFailTest_MakesSureUserIsNotRemindedOfTheirUsernameUntilTheAccountIsNotActivated_VerifiesByTheReturnedValue()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Reqeust to remind for password
            UserController userController = (UserController)_applicationContext["UserController"];
            IHttpActionResult forgotUsernameResponse = userController.ForgotUsername(new ForgotUsernameParams(email));
            // The response should be an error message
            BadRequestErrorMessageResult badRequestErrorMessage = (BadRequestErrorMessageResult)forgotUsernameResponse;
            // Check if the username returned is correct
            Assert.IsNotNull(badRequestErrorMessage);
        }

        [Test]
        [Category("Integration")]
        public void ForgotPasswordSuccessfulTest_MakesSureAUniqueCodeIsSentToThem_VerifiesByTheReturnedValue()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Confirm that the User has been activated
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            // Reqeust to remind for password
            IHttpActionResult forgotPasswordResponse = userController.ForgotPassword(new ForgotPasswordParams(email, username));
            OkNegotiatedContentResult<string> forgotPasswordOkResponse = (OkNegotiatedContentResult<string>)forgotPasswordResponse;
            // Check if the username returned is correct
            Assert.IsNotNull(forgotPasswordOkResponse.Content);
        }

        [Test]
        [Category("Integration")]
        public void ForgotPasswordFailTest_MakesSureUserIsNotRemindedOfTheirPasswordUntilTheAccountIsNotActivated_VerifiesByTheReturnedValue()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Reqeust to remind for password
            UserController userController = (UserController)_applicationContext["UserController"];
            IHttpActionResult forgotUsernameResponse = userController.ForgotPassword(new ForgotPasswordParams(email, username));
            // The response should be an error message
            BadRequestErrorMessageResult badRequestErrorMessage = (BadRequestErrorMessageResult)forgotUsernameResponse;
            // Check if the username returned is correct
            Assert.IsNotNull(badRequestErrorMessage);
        }

        [Test]
        [Category("Integration")]
        public void ResetPasswordSuccessfulTest_MakesSurePasswordGetsResetIfCredentialsAreValid_VerifiesByTheReturnedValueAndDatabaseQuerying()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Reqeust to remind for password
            IHttpActionResult forgotPasswordResponse = userController.ForgotPassword(new ForgotPasswordParams(email, username));
            OkNegotiatedContentResult<string> forgotPasswordOkResponse = (OkNegotiatedContentResult<string>)forgotPasswordResponse;
            // Check if the username returned is correct
            Assert.IsNotNull(forgotPasswordOkResponse.Content);

            // Confirm that the User has been activated and forgot password code has been generated
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);
            Assert.IsNotNull(userByUserName.ForgotPasswordCode);
            Assert.IsNotNull(userByUserName.ForgotPasswordCodeExpiration);

            // Reqeust to reset password
            string newPassword = "iwillwearthemask";
            IHttpActionResult resetPasswordResponse = userController.ResetPassword(new ResetPasswordParams(username, newPassword));            
            OkNegotiatedContentResult<bool> resetPasswordOkResponse = (OkNegotiatedContentResult<bool>)resetPasswordResponse;
            // Check if the username returned is correct
            Assert.IsTrue(resetPasswordOkResponse.Content);

            User userAfterResetPassword = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userAfterResetPassword);
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(newPassword, userAfterResetPassword.Password));
            Assert.IsNull(userAfterResetPassword.ForgotPasswordCode);
            Assert.IsNull(userAfterResetPassword.ForgotPasswordCodeExpiration);
            Assert.AreEqual(1, userAfterResetPassword.ForgottenPasswordCodes.Length);
        }

        [Test]
        [Category("Integration")]
        public void ResetPasswordFailTest_MakesSurePasswordDoesNotGetResetIfNoForgotPasswordRequestIsMade_VerifiesByTheReturnedValueAndDatabaseQuerying()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Confirm that the User has been activated and forgot password code has been generated
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);

            // Reqeust to reset password
            string newPassword = "iwillwearthemask";
            IHttpActionResult resetPasswordResponse = userController.ResetPassword(new ResetPasswordParams(username, newPassword));
            BadRequestErrorMessageResult resetPasswordOkResponse = (BadRequestErrorMessageResult)resetPasswordResponse;
            // Check if the username returned is correct
            Assert.IsNotNull(resetPasswordOkResponse);

            User userAfterResetTry = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userAfterResetTry);            
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
        }

        [Test]
        [Category("Integration")]
        public void ChangeSettingSuccessfulTest_ChecksThatIfTheSettingsChangeForTheUserOnTheGivenParameters_ChecksThroughDatabaseQuerying()
        {
            // Register an account
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            string email = "waqasshah047@gmail.com";
            string username = "Bane";
            string password = "iwearamask";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username,
                password, "Pakistan", TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage = (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<bool> okResponseMessage1 = (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.IsTrue(okResponseMessage1.Content);

            // Confirm the ucrrent user settings
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            Assert.AreEqual(email, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.English, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 10, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);

            // Reqeust to reset password
            string newEMail = "iwillwearthemask@banegotham.sf";
            IHttpActionResult changeSettingsResponse = userController.ChangeSettings(new ChangeSettingsCommand(username,
                newEMail, "", Language.French, TimeZone.CurrentTimeZone, false, 66));
            OkNegotiatedContentResult<bool> resetPasswordOkResponse = (OkNegotiatedContentResult<bool>)changeSettingsResponse;
            // Check if the username returned is correct
            Assert.IsTrue(resetPasswordOkResponse.Content);

            userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(email, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.French, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 66, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);
        }
    }
}
