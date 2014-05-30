using System;
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
           httpActionResult=userController.ActivateUser(new UserActivationParam("user", "123", activationKey));
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
            OkNegotiatedContentResult<bool> okResponseMessage1 =
                (OkNegotiatedContentResult<bool>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, true);

        }
    }
}
