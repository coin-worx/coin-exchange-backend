using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            OkNegotiatedContentResult<bool> okResponseMessage1 =
                (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, true);

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
            IHttpActionResult logoutResult = logoutController.Logout(new LogoutParams(keys.Content.ApiKey, keys.Content.SecretKey));
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
            OkNegotiatedContentResult<bool> okResponseMessage1 =
                (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, true);

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
            IHttpActionResult logoutResult = logoutController.Logout(new LogoutParams(keys.Content.ApiKey + "1", keys.Content.SecretKey));
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
            OkNegotiatedContentResult<bool> okResponseMessage1 =
                (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, true);

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
            IHttpActionResult logoutResult = logoutController.Logout(new LogoutParams(keys.Content.ApiKey, keys.Content.SecretKey));
            OkNegotiatedContentResult<bool> logoutOkResponse = (OkNegotiatedContentResult<bool>)logoutResult;
            Assert.IsNotNull(logoutOkResponse);
            Assert.IsTrue(logoutOkResponse.Content);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNull(securityKeysPair);

            // Invalid Logout as the user has logged out already
            logoutResult = logoutController.Logout(new LogoutParams(keys.Content.ApiKey, keys.Content.SecretKey));
            BadRequestErrorMessageResult logoutBadResponse = (BadRequestErrorMessageResult)logoutResult;
            Assert.IsNotNull(logoutBadResponse);

            // Verify that the Security Keys are not in the database
            securityKeysPair = securityKeysRepository.GetByApiKey(keys.Content.ApiKey);
            Assert.IsNull(securityKeysPair);
        }
    }
}
