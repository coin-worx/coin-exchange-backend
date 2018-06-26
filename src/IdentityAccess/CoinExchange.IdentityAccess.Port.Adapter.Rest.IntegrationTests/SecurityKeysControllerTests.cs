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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Projection;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    public class SecurityKeysControllerTests
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
        public void CreateUsergeneratedSystemKey_ProvideAllParameters_TheKeysShouldBeReturned()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            SecurityKeyPairController securityKeyPairController =
                _applicationContext["SecurityKeyPairController"] as SecurityKeyPairController;
            IPermissionRepository permissionRepository = _applicationContext["PermissionRespository"] as IPermissionRepository;
            IList<Permission> permissions = permissionRepository.GetAllPermissions();
            List<string> securityKeyPermissions=new List<string>();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            securityKeyPairController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            securityKeyPairController.Request.Headers.Add("Auth", essentials.ApiKey);
            CreateUserGeneratedSecurityKeyPair command=new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,"","","",false,false,false,"#1");
            IHttpActionResult httpActionResult=securityKeyPairController.CreateSecurityKey(command);
            OkNegotiatedContentResult<SecurityKeyPair> result = (OkNegotiatedContentResult<SecurityKeyPair>)httpActionResult;
            Assert.IsNotNullOrEmpty(result.Content.ApiKey);
            Assert.IsNotNullOrEmpty(result.Content.SecretKey);

            CreateUserGeneratedSecurityKeyPair command2 = new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions, "", "", "", false, false, false, "#2");
            httpActionResult = securityKeyPairController.CreateSecurityKey(command2);
            result = (OkNegotiatedContentResult<SecurityKeyPair>)httpActionResult;
            Assert.IsNotNullOrEmpty(result.Content.ApiKey);
            Assert.IsNotNullOrEmpty(result.Content.SecretKey);

            httpActionResult = securityKeyPairController.GetUserSecurityKeys();
            OkNegotiatedContentResult<object> result1 = (OkNegotiatedContentResult<object>)httpActionResult;
            List<object> objectPairs = result1.Content as List<object>;
            List<SecurityKeyPairList> pairs = new List<SecurityKeyPairList>();
            foreach (object objectPair in objectPairs)
            {
                pairs.Add(objectPair as SecurityKeyPairList);
            }

            Assert.AreEqual(pairs.Count,2);
            Assert.AreEqual(pairs[1].KeyDescription, "#1");
            Assert.IsNull(pairs[1].ExpirationDate);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#1");
            OkNegotiatedContentResult<SecurityKeyRepresentation> securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion,"#1");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate,false); 
            Assert.AreEqual(securityKey.Content.EnableStartDate, false);

            Assert.AreEqual(pairs[0].KeyDescription, "#2");
            Assert.IsNull(pairs[0].ExpirationDate);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#2");
            securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion, "#2");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate, false);
            Assert.AreEqual(securityKey.Content.EnableStartDate, false);
        }

        [Test]
        [Category("Integration")]
        public void UpdateUsergeneratedSystemKey_ProvideAllParameters_TheKeysDetailsShouldGetUpdated()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            SecurityKeyPairController securityKeyPairController =
                _applicationContext["SecurityKeyPairController"] as SecurityKeyPairController;
            IPermissionRepository permissionRepository = _applicationContext["PermissionRespository"] as IPermissionRepository;
            IList<Permission> permissions = permissionRepository.GetAllPermissions();
            List<string> securityKeyPermissions = new List<string>();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            securityKeyPairController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            securityKeyPairController.Request.Headers.Add("Auth", essentials.ApiKey);
            CreateUserGeneratedSecurityKeyPair command = new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions, "", "", "", false, false, false, "#1");
            IHttpActionResult httpActionResult = securityKeyPairController.CreateSecurityKey(command);
            OkNegotiatedContentResult<SecurityKeyPair> result = (OkNegotiatedContentResult<SecurityKeyPair>)httpActionResult;
            Assert.IsNotNullOrEmpty(result.Content.ApiKey);
            Assert.IsNotNullOrEmpty(result.Content.SecretKey);

            httpActionResult = securityKeyPairController.GetUserSecurityKeys();
            OkNegotiatedContentResult<object> result1 = (OkNegotiatedContentResult<object>)httpActionResult;
            List<object> objectPairs = result1.Content as List<object>;
            Assert.IsNotNull(objectPairs);
            List<SecurityKeyPairList> pairs = new List<SecurityKeyPairList>();
            foreach (object objectPair in objectPairs)
            {
                pairs.Add(objectPair as SecurityKeyPairList);
            }
            Assert.AreEqual(pairs.Count, 1);
            Assert.AreEqual(pairs[0].KeyDescription, "#1");
            Assert.IsNull(pairs[0].ExpirationDate);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#1");
            OkNegotiatedContentResult<SecurityKeyRepresentation> securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion, "#1");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate, false);
            Assert.AreEqual(securityKey.Content.EnableStartDate, false);

            List<SecurityKeyPermissionsRepresentation> permissionsRepresentations = new List<SecurityKeyPermissionsRepresentation>();
            for (int i = 0; i < securityKeyPermissions.Count; i++)
            {
                if (securityKeyPermissions[i] == PermissionsConstant.Cancel_Order)
                    permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(false, new Permission(securityKeyPermissions[i], "Cancel Order")));
                else if (securityKeyPermissions[i] == PermissionsConstant.Query_Open_Orders)
                    permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(false, new Permission(securityKeyPermissions[i], "Query Open Orders")));
                else if (securityKeyPermissions[i] == PermissionsConstant.Place_Order)
                    permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(false, new Permission(securityKeyPermissions[i], "Place Order")));
                else if (securityKeyPermissions[i] == PermissionsConstant.Withdraw_Funds)
                    permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(false, new Permission(securityKeyPermissions[i], "Withdraw Funds")));
                else
                {
                    permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(true, new Permission(securityKeyPermissions[i], securityKeyPermissions[i])));
                }
            }
            UpdateUserGeneratedSecurityKeyPair updateKeyPair =
                new UpdateUserGeneratedSecurityKeyPair(securityKey.Content.ApiKey, "#2", true, false, false, "",
                    DateTime.Today.AddDays(-2).ToString(), permissionsRepresentations.ToArray(), "");
            httpActionResult = securityKeyPairController.UpdateSecurityKey(updateKeyPair);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#2");
            securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion, "#2");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate, false);
            Assert.AreEqual(securityKey.Content.EnableStartDate, true);
            ValidatePermissions(securityKey.Content.SecurityKeyPermissions);

        }

        [Test]
        [Category("Integration")]
        public void UpdateUsergeneratedSystemKey_IfNoPermissionIsAssigned_InvalidOperationExceptionWillBeThrown()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            SecurityKeyPairController securityKeyPairController =
                _applicationContext["SecurityKeyPairController"] as SecurityKeyPairController;
            IPermissionRepository permissionRepository = _applicationContext["PermissionRespository"] as IPermissionRepository;
            IList<Permission> permissions = permissionRepository.GetAllPermissions();
            List<string> securityKeyPermissions = new List<string>();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            securityKeyPairController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            securityKeyPairController.Request.Headers.Add("Auth", essentials.ApiKey);
            CreateUserGeneratedSecurityKeyPair command = new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions, "", "", "", false, false, false, "#1");
            IHttpActionResult httpActionResult = securityKeyPairController.CreateSecurityKey(command);
            OkNegotiatedContentResult<SecurityKeyPair> result = (OkNegotiatedContentResult<SecurityKeyPair>)httpActionResult;
            Assert.IsNotNullOrEmpty(result.Content.ApiKey);
            Assert.IsNotNullOrEmpty(result.Content.SecretKey);

            httpActionResult = securityKeyPairController.GetUserSecurityKeys();
            OkNegotiatedContentResult<object> result1 = (OkNegotiatedContentResult<object>)httpActionResult;
            List<object> objectPairs = result1.Content as List<object>;
            Assert.IsNotNull(objectPairs);
            List<SecurityKeyPairList> pairs = new List<SecurityKeyPairList>();
            foreach (object objectPair in objectPairs)
            {
                pairs.Add(objectPair as SecurityKeyPairList);
            }
            Assert.AreEqual(pairs.Count, 1);
            Assert.AreEqual(pairs[0].KeyDescription, "#1");
            Assert.IsNull(pairs[0].ExpirationDate);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#1");
            OkNegotiatedContentResult<SecurityKeyRepresentation> securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion, "#1");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate, false);
            Assert.AreEqual(securityKey.Content.EnableStartDate, false);

            List<SecurityKeyPermissionsRepresentation> permissionsRepresentations = new List<SecurityKeyPermissionsRepresentation>();
            for (int i = 0; i < securityKeyPermissions.Count; i++)
            {
                permissionsRepresentations.Add(new SecurityKeyPermissionsRepresentation(false, new Permission(securityKeyPermissions[i], "")));

            }
            UpdateUserGeneratedSecurityKeyPair updateKeyPair =
                new UpdateUserGeneratedSecurityKeyPair(securityKey.Content.ApiKey, "#2", true, false, false, "",
                    DateTime.Today.AddDays(-2).ToString(), permissionsRepresentations.ToArray(), "");
            httpActionResult = securityKeyPairController.UpdateSecurityKey(updateKeyPair);
            BadRequestErrorMessageResult errorMessage = (BadRequestErrorMessageResult) httpActionResult;
            Assert.AreEqual(errorMessage.Message,"Please assign atleast one permission.");
        }

        /// <summary>
        /// validate permissions
        /// </summary>
        private void ValidatePermissions(SecurityKeyPermissionsRepresentation[] representation)
        {
            for (int i = 0; i < representation.Length; i++)
            {
                if (representation[i].Permission.PermissionId == PermissionsConstant.Cancel_Order)
                    Assert.False(representation[i].Allowed);
                if (representation[i].Permission.PermissionId == PermissionsConstant.Query_Open_Orders)
                    Assert.False(representation[i].Allowed);
                if (representation[i].Permission.PermissionId == PermissionsConstant.Place_Order)
                    Assert.False(representation[i].Allowed);
                if (representation[i].Permission.PermissionId == PermissionsConstant.Withdraw_Funds)
                    Assert.False(representation[i].Allowed);
            }
        }
    }
}
