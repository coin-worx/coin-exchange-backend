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
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    public class LoginControllerTests
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
        public void LoginSuccessfulTest_MakesSureAccountIsCreated_VerifiesAndAssertsTheReturnedValue()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", "user", "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>) httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());
        }

        [Test]
        [Category("Integration")]
        public void LoginSuccessfulTest_MakesSureAccountIsCreatedAndUserLogsin_VerifiesByReturnedValueAndDatabaseQuerying()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];

            string username = "user";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", username, "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            int currentHour = DateTime.Now.Hour;
            httpActionResult = loginController.Login(new LoginParams(username, "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(currentHour, userByUserName.LastLogin.Hour);
        }

        [Test]
        [Category("Integration")]
        public void LoginFailTest_ChecksThatUserCannotLoginBeforeActivatingAccount_VerifiesByReturnedValueAndDatabaseQuerying()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];

            string username = "user";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", username, "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Login attempt without Activating Account
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            httpActionResult = loginController.Login(new LoginParams(username, "123"));
            BadRequestErrorMessageResult badRequest = (BadRequestErrorMessageResult)httpActionResult;
            Assert.IsNotNull(badRequest);

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            // Not logged in yet
            Assert.AreEqual(DateTime.MinValue, userByUserName.LastLogin);
            Assert.IsFalse(userByUserName.IsActivationKeyUsed.Value);
        }

        [Test]
        [Category("Integration")]
        public void LoginFailThenSuccessfulTest_ChecksThatUserCannotLoginBeforeActivatingAccountAndThenAllowsLoginAfterActivation_VerifiesByReturnedValueAndDatabaseQuerying()
        {
            RegistrationController registrationController = (RegistrationController)_applicationContext["RegistrationController"];

            string username = "user";
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam("waqasshah047@gmail.com", username, "123", "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            Assert.IsNotNullOrEmpty(activationKey);

            // Login attempt without Activating Account
            LoginController loginController = (LoginController)_applicationContext["LoginController"];
            int currentHour = DateTime.Now.Hour;
            httpActionResult = loginController.Login(new LoginParams(username, "123"));
            BadRequestErrorMessageResult badRequest = (BadRequestErrorMessageResult)httpActionResult;
            Assert.IsNotNull(badRequest);

            // Activate Account
            UserController userController = (UserController)_applicationContext["UserController"];
            httpActionResult = userController.ActivateUser(new UserActivationParam(username, "123", activationKey));
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            // Login again
            httpActionResult = loginController.Login(new LoginParams(username, "123"));
            OkNegotiatedContentResult<UserValidationEssentials> okReponse =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(okReponse.Content.ApiKey);
            Assert.IsNotNullOrEmpty(okReponse.Content.SecretKey);
            Assert.IsNotNullOrEmpty(okReponse.Content.SessionLogoutTime.ToString());

            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.AreEqual(currentHour, userByUserName.LastLogin.Hour);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);
        }
    }
}
