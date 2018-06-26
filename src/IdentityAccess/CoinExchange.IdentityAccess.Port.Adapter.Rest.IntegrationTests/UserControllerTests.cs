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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
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
           OkNegotiatedContentResult<string> okResponseMessage1 =
               (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "cancelled");

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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

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
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", userValidationEssentials.ApiKey);
            IHttpActionResult changePasswordResult = userController.ChangePassword(new ChangePasswordParams(
             password, newPassword));
            OkNegotiatedContentResult<string> changePasswordOkResult = (OkNegotiatedContentResult<string>)changePasswordResult;
            Assert.AreEqual(changePasswordOkResult.Content,"changed");

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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

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
            IHttpActionResult resetPasswordResponse = userController.ResetPassword(new ResetPasswordParams(username, newPassword, forgotPasswordOkResponse.Content));            
            OkNegotiatedContentResult<string> resetPasswordOkResponse = (OkNegotiatedContentResult<string>)resetPasswordResponse;
            // Check if the username returned is correct
            Assert.AreEqual(resetPasswordOkResponse.Content,"changed");

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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

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
            IHttpActionResult resetPasswordResponse = userController.ResetPassword(new ResetPasswordParams(username, newPassword,""));
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

            // Confirm the current user settings
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

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, password));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Reqeust to change settings
            string newEMail = "iwillwearthemask@banegotham.sf";
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult changeSettingsResponse = userController.ChangeSettings(new ChangeSettingsParams(
                newEMail, "", Language.French, TimeZone.CurrentTimeZone, false, 66));
            OkNegotiatedContentResult<string> changePasswordOkResponse = (OkNegotiatedContentResult<string>)changeSettingsResponse;
            // Check if the username returned is correct
            Assert.AreEqual(changePasswordOkResponse.Content,"changed");

            userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(newEMail, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.French, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 66, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);
        }

        [Test]
        [Category("Integration")]
        public void GetLastLoginOfTheUser_IfTheCallGetsAuthenticated_LastLoginWillBeReturned()
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Confirm the current user settings
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

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, password));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            httpActionResult = userController.LastLogin();
            OkNegotiatedContentResult<DateTime> lastLogin = (OkNegotiatedContentResult<DateTime>) httpActionResult;
            Assert.AreEqual(keys.Content.LastLogin.ToString(),lastLogin.Content.ToString());

        }

        [Test]
        [Category("Integration")]
        public void ChangeSettingsFailTest_ChecksThatUserLogsInThenLogsOutAndThenTriesToChangeSettingsAgainUsingTheSameApiKeyThenExceptionShouldBeThrown_VerifiesAndAssertsTheReturnedValueAndQueriesDatabase()
        {
            string username = "user";
            string password = "123";
            string email = "user@user123.com";
            // Register User
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, username, password, "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            
            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, password, activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, password));
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

            // Reqeust to change settings
            string newEMail = "iwillwearthemask@banegotham.sf";
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult changeSettingsResponse = userController.ChangeSettings(new ChangeSettingsParams(
                newEMail, "", Language.French, TimeZone.CurrentTimeZone, false, 66));
            OkNegotiatedContentResult<string> changePasswordOkResponse = (OkNegotiatedContentResult<string>)changeSettingsResponse;
            // Check if the username returned is correct
            Assert.AreEqual(changePasswordOkResponse.Content,"changed");

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(newEMail, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.French, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 66, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);

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

            string lastInvalidEmail = "errre@user123.com";
            // Invalid attempt to change settings
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            changeSettingsResponse = userController.ChangeSettings(new ChangeSettingsParams(
                lastInvalidEmail, "", Language.German, TimeZone.CurrentTimeZone, false, 97));

            BadRequestErrorMessageResult badRequestErrorMessage = (BadRequestErrorMessageResult) changeSettingsResponse;
            Assert.IsNotNull(badRequestErrorMessage);

            userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(newEMail, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.French, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 66, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);
        }

        [Test]
        [Category("Integration")]
        public void GetAccountSettingSuccessfulTest_ChecksThatTheAccountSettingsAreReceivedAsExpected_ChecksThroughDatabaseQuerying()
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content,"activated");

            // Confirm the current user settings
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

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, password));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Get the Account Settings
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult accountSettingsResponse = userController.GetAccountSettings();
            OkNegotiatedContentResult<AccountSettingsRepresentation> accountSettingsOkResponse =
                (OkNegotiatedContentResult<AccountSettingsRepresentation>) accountSettingsResponse;
            Assert.AreEqual(userByUserName.Email, accountSettingsOkResponse.Content.Email);
            Assert.AreEqual(userByUserName.Username, accountSettingsOkResponse.Content.Username);
            Assert.AreEqual(userByUserName.TimeZone.StandardName, accountSettingsOkResponse.Content.TimeZone.StandardName);
            Assert.AreEqual(userByUserName.Language, accountSettingsOkResponse.Content.Language);
            Assert.AreEqual(userByUserName.AutoLogout.Minutes, accountSettingsOkResponse.Content.AutoLogoutMinutes);
            Assert.IsTrue(accountSettingsOkResponse.Content.IsDefaultAutoLogout);

            // Reqeust to change settings
            string newEMail = "iwillwearthemask@banegotham.sf";
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            IHttpActionResult changeSettingsResponse = userController.ChangeSettings(new ChangeSettingsParams(
                newEMail, "", Language.French, TimeZone.CurrentTimeZone, false, 66));
            OkNegotiatedContentResult<string> changePasswordOkResponse = (OkNegotiatedContentResult<string>)changeSettingsResponse;
            // Check if the username returned is correct
            Assert.AreEqual(changePasswordOkResponse.Content,"changed");

            userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(newEMail, userByUserName.Email);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, userByUserName.Password));
            Assert.AreEqual(Language.French, userByUserName.Language);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, userByUserName.TimeZone.StandardName);
            Assert.AreEqual(new TimeSpan(0, 0, 66, 0), userByUserName.AutoLogout);
            Assert.IsNull(userByUserName.ForgotPasswordCode);
            Assert.IsNull(userByUserName.ForgotPasswordCodeExpiration);
            Assert.AreEqual(0, userByUserName.ForgottenPasswordCodes.Length);

            // Get the Account Settings again
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);
            accountSettingsResponse = userController.GetAccountSettings();
            accountSettingsOkResponse = (OkNegotiatedContentResult<AccountSettingsRepresentation>)accountSettingsResponse;
            Assert.AreEqual(userByUserName.Email, accountSettingsOkResponse.Content.Email);
            Assert.AreEqual(userByUserName.Username, accountSettingsOkResponse.Content.Username);
            Assert.AreEqual(userByUserName.TimeZone.StandardName, accountSettingsOkResponse.Content.TimeZone.StandardName);
            Assert.AreEqual(userByUserName.Language, accountSettingsOkResponse.Content.Language);
            Assert.AreEqual(userByUserName.AutoLogout.Minutes, accountSettingsOkResponse.Content.AutoLogoutMinutes);
            Assert.IsFalse(accountSettingsOkResponse.Content.IsDefaultAutoLogout);
        }

        [Test]
        [Category("Integration")]
        public void UserMfaSubscriptionsSubmissionTest_ChecksThatMfaSubscriptionsForSystemGeneratedKeyAreSubmittedAsExpected_VerifiesThroughDatabaseQuery()
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
            OkNegotiatedContentResult<string> okResponseMessage1 = (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Confirm the current user settings
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

            // Login
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, password));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            // Get the Account Settings
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", keys.Content.ApiKey);

            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)_applicationContext["MfaSubscriptionRepository"];
            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            List<MfaSingleSettingParams> mfaSettingsList = new List<MfaSingleSettingParams>();
            
            foreach (var subscription in allSubscriptions)
            {
                mfaSettingsList.Add(new MfaSingleSettingParams(subscription.MfaSubscriptionId, subscription.MfaSubscriptionName,
                    true));
            }

            IHttpActionResult submissionResponse = userController.SubmitMfaSettings(new MfaSettingsParams(false, null, mfaSettingsList));
            Assert.IsNotNull(submissionResponse);

            OkNegotiatedContentResult<SubmitMfaSettingsResponse> okSubmissionResponse = (OkNegotiatedContentResult<SubmitMfaSettingsResponse>) submissionResponse;
            Assert.IsTrue(okSubmissionResponse.Content.Successful);

            User user = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(user);
            // Check that all the4 expected subscriptions are present in the user instance
            foreach (var subscription in allSubscriptions)
            {
                Assert.IsTrue(user.CheckMfaSubscriptions(subscription.MfaSubscriptionName));
            }
        }

        [Test]
        [Category("Integration")]
        public void ApiKeyMfaSubscriptionsSubmissionTest_ChecksThatMfaSubscriptionsForSystemGeneratedKeyAreSubmittedAsExpected_VerifiesThroughDatabaseQuery()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];
            IPermissionRepository permissionRepository = (IPermissionRepository)_applicationContext["PermissionRespository"];
            UserController userController = (UserController)_applicationContext["UserController"];
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];

            IList<Permission> allPermissions = permissionRepository.GetAllPermissions();
            List<SecurityKeysPermission> securityKeysPermissions = new List<SecurityKeysPermission>();

            string apiKey = "apikey123";
            string secretKey = "secretkey123";
            foreach (var permission in allPermissions)
            {
                securityKeysPermissions.Add(new SecurityKeysPermission(apiKey, permission, true));
            }
            
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(apiKey, secretKey, "#1", 1, false, securityKeysPermissions);
            persistenceRepository.SaveUpdate(securityKeysPair);
            
            // Get the Account Settings
            userController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            userController.Request.Headers.Add("Auth", securityKeysPair.ApiKey);

            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)_applicationContext["MfaSubscriptionRepository"];
            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            List<MfaSingleSettingParams> mfaSettingsList = new List<MfaSingleSettingParams>();

            foreach (var subscription in allSubscriptions)
            {
                mfaSettingsList.Add(new MfaSingleSettingParams(subscription.MfaSubscriptionId, subscription.MfaSubscriptionName,
                    true));
            }

            string mfaCode = "mfacode123";
            IHttpActionResult submissionResponse = userController.SubmitMfaSettings(new MfaSettingsParams(true, mfaCode, mfaSettingsList));
            Assert.IsNotNull(submissionResponse);
            OkNegotiatedContentResult<SubmitMfaSettingsResponse> okSubmissionResponse = (OkNegotiatedContentResult<SubmitMfaSettingsResponse>) submissionResponse;
            Assert.IsTrue(okSubmissionResponse.Content.Successful);
            SecurityKeysPair retreivedKey = securityKeysRepository.GetByApiKey(apiKey);
            Assert.IsNotNull(retreivedKey);
            // Check that all the expected subscriptions are present in the user instance
            foreach (var subscription in allSubscriptions)
            {
                Assert.IsTrue(retreivedKey.CheckMfaSubscriptions(subscription.MfaSubscriptionName));
            }
            Assert.AreEqual(mfaCode, retreivedKey.MfaCode);
        }
    }
}
