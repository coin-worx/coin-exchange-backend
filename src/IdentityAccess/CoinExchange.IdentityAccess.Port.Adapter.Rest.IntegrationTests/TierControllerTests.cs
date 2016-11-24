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
            OkNegotiatedContentResult<string> okResponseMessage1 =
                (OkNegotiatedContentResult<string>)httpActionResult;
            Assert.AreEqual(okResponseMessage1.Content, "activated");

            LoginController loginController = _applicationContext["LoginController"] as LoginController;
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            Assert.IsNotNullOrEmpty(keys.Content.ApiKey);
            Assert.IsNotNullOrEmpty(keys.Content.SecretKey);
            Assert.IsNotNullOrEmpty(keys.Content.SessionLogoutTime.ToString());

            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Get, "");
            tierController.Request.Headers.Add("Auth", keys.Content.ApiKey);
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
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);
            tierController.GetVerifyForTier1(new Tier1Param("User",DateTime.Today.AddDays(-10).ToShortDateString(),"656667"));

            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());

            httpActionResult = tierController.GetTier1Details();
            OkNegotiatedContentResult<Tier1Details> detials = (OkNegotiatedContentResult<Tier1Details>)httpActionResult;
            Assert.AreEqual(detials.Content.Country,"Pakistan");
            Assert.AreEqual(detials.Content.FullName, "User");
            Assert.AreEqual(detials.Content.DateOfBirth, DateTime.Today.AddDays(-10));
            Assert.AreEqual(detials.Content.PhoneNumber, "656667");
        }

        [Test]
        [Category("Integration")]
        public void ApplyForTier2_IfCallIsValid_Tier2StatusWillBeChangedToPreVerified()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);
            tierController.GetVerifyForTier1(new Tier1Param("User", DateTime.Now.AddDays(-10).ToShortDateString(), "656667"));
            tierController.GetVerifyForTier2(new Tier2Param("asd","","","punjab","Isb","123"));

            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Preverified.ToString());

            httpActionResult = tierController.GetTier2Details();
            OkNegotiatedContentResult<Tier2Details> detials = (OkNegotiatedContentResult<Tier2Details>)httpActionResult;
            Assert.AreEqual(detials.Content.Country, "Pakistan");
            Assert.AreEqual(detials.Content.AddressLine1, "asd");
            Assert.AreEqual(detials.Content.State, "punjab");
            Assert.AreEqual(detials.Content.City, "Isb");
            Assert.AreEqual(detials.Content.ZipCode, "123");
        }

        [Test]
        [Category("Integration")]
        public void ApplyForTier3_IfCallIsValid_Tier3StatusWillBeChangedToPreVerified()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);
            tierController.GetVerifyForTier1(new Tier1Param("User", DateTime.Now.AddDays(-10).ToShortDateString(), "656667"));
            tierController.GetVerifyForTier2(new Tier2Param("asd", "", "", "punjab", "Isb", "123"));
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new FileStream(@"C:\Logs\Logs.txt", FileMode.Open)));
            tierController.Request.Content = content;
            tierController.GetVerifyForTier3(new Tier3Param("asd", "123", "bill", "logs.txt"));
            ManualResetEvent resetEvent=new ManualResetEvent(false);
            resetEvent.WaitOne(15000);
            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[3].Status, Status.Preverified.ToString());

            httpActionResult = tierController.GetTier3Details();
            OkNegotiatedContentResult<Tier3Details> detials = (OkNegotiatedContentResult<Tier3Details>)httpActionResult;
            Assert.AreEqual(detials.Content.Nin, "123");
            Assert.AreEqual(detials.Content.Ssn, "asd");
        }

        [Test]
        [Category("Integration")]
        public void GetTier2Detials_IfTier1IsNotVerified_InvalidOperationExceptionShouldBeThrown()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);
            IHttpActionResult httpActionResult = tierController.GetTier2Details();
            BadRequestErrorMessageResult result = httpActionResult as BadRequestErrorMessageResult;
            Assert.AreEqual(result.Message, "Tier 2 details are not submitted yet.");
        }

        [Test]
        [Category("Integration")]
        public void GetTier3Detials_IfTier3IsNotVerified_InvalidOperationExceptionShouldBeThrown()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;
            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);
            IHttpActionResult httpActionResult = tierController.GetTier3Details();
            BadRequestErrorMessageResult result = httpActionResult as BadRequestErrorMessageResult;
            Assert.AreEqual(result.Message, "Tier 3 details are not submitted yet.");
        }

        [Test]
        [Category("Integration")]
        public void VerifyTierLevelTest_TestsIfTheTierLevelIsVerifiedAsExpected_QueriesDatabaseToConfirm()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            TierController tierController = _applicationContext["TierController"] as TierController;            
            Assert.IsNotNull(tierController);

            tierController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tierController.Request.Headers.Add("Auth", essentials.ApiKey);

            tierController.GetVerifyForTier1(new Tier1Param("User", DateTime.Now.AddDays(-10).ToShortDateString(), "656667"));
            // Tier 2 will not be considered for verification because Tier 1 is not yet verified
            tierController.GetVerifyForTier2(new Tier2Param("asd", "", "", "punjab", "Isb", "123"));

            IHttpActionResult httpActionResult = tierController.GetTierStatuses();
            OkNegotiatedContentResult<UserTierStatusRepresentation[]> statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Preverified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.NonVerified.ToString());

            IHttpActionResult verifyTierLevelResult = tierController.VerifyTierLevel(new VerifyTierLevelParams("Tier 1", essentials.ApiKey));
            OkNegotiatedContentResult<VerifyTierLevelResponse> verificationResponse = (OkNegotiatedContentResult<VerifyTierLevelResponse>)verifyTierLevelResult;
            Assert.IsTrue(verificationResponse.Content.VerificationSuccessful);

            httpActionResult = tierController.GetTierStatuses();
            statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.NonVerified.ToString());

            // Tier 2 will now be verified as Tier 1 is already verified
            tierController.GetVerifyForTier2(new Tier2Param("asd", "", "", "punjab", "Isb", "123"));
            httpActionResult = tierController.GetTierStatuses();
            statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Preverified.ToString());

            verifyTierLevelResult = tierController.VerifyTierLevel(new VerifyTierLevelParams("Tier 2", essentials.ApiKey));
            verificationResponse = (OkNegotiatedContentResult<VerifyTierLevelResponse>)verifyTierLevelResult;
            Assert.IsTrue(verificationResponse.Content.VerificationSuccessful);

            httpActionResult = tierController.GetTierStatuses();
            statuses = (OkNegotiatedContentResult<UserTierStatusRepresentation[]>)httpActionResult;
            Assert.AreEqual(statuses.Content.Length, 5);
            Assert.AreEqual(statuses.Content[0].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[1].Status, Status.Verified.ToString());
            Assert.AreEqual(statuses.Content[2].Status, Status.Verified.ToString());
        }
    }
}
