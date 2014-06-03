using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Tier controller integration tests
    /// </summary>
    [TestFixture]
    public class TierControllerTests
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
        public void ActivateAccount_IfAccountIsActivatedSuccessfully_TheTier0LevelWillGetVerified()
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

            LoginController loginController = _applicationContext["LoginController"] as LoginController;
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey.Value);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey.Value);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Get, "");
            tierController.Request.Headers.Add("Auth", keys.Content.ApiKey.Value);
            httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length,5);
            Assert.AreEqual(statuses.Content[0].Status,Status.Verified.ToString());
        }

        [Test]
        [Category("Integration")]
        public void ApplyForTier1_IfCallIsValid_Tier1StatusWillBeChangedToPreVerified()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123",_applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey.Value);
            tierController.GetVerifyForTier1("667778899");

            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
        }

        [Test]
        [Category("Integration")]
        public void ApplyForTier2_IfCallIsValid_Tier2StatusWillBeChangedToPreVerified()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey.Value);
            tierController.GetVerifyForTier1("667778899");
            tierController.GetVerifyForTier2(new Tier2Param("asd","","","punjab","Isb","123"));

            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Preverified.ToString());
        }

        [Test]
        [Category("Integration")]
        public void ApplyForTier3_IfCallIsValid_Tier3StatusWillBeChangedToPreVerified()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey.Value);
            tierController.GetVerifyForTier1("667778899");
            tierController.GetVerifyForTier2(new Tier2Param("asd", "", "", "punjab", "Isb", "123"));
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new FileStream(@"C:\Logs\Logs.txt", FileMode.Open)));
            tierController.Request.Content = content;
            tierController.GetVerifyForTier3(new Tier3Param("asd", "123", "bill", "logs.txt"));
            ManualResetEvent resetEvent=new ManualResetEvent(false);
            resetEvent.WaitOne(10000);
            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[3].Status, Status.Preverified.ToString());
        }
    }
}
