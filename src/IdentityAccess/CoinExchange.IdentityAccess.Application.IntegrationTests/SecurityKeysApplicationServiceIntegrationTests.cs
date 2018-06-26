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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using Spring.Data.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class SecurityKeysApplicationServiceIntegrationTests
    {
        private IApplicationContext _applicationContext;
        private DatabaseUtility _databaseUtility;
        private ISecurityKeysRepository _securityKeysRepository;
        private IPermissionRepository _permissionRepository;

        [SetUp]
        public void Setup()
        {
            _applicationContext = ContextRegistry.GetContext();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _securityKeysRepository = _applicationContext["SecurityKeysPairRepository"] as ISecurityKeysRepository;
            _permissionRepository = _applicationContext["PermissionRespository"] as IPermissionRepository;
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
        public void ServiceStartuptest_TestsIfTheServiceStartsAsExpectedThroughSpring_VerifiesThroughTheStateOfTheObject()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            Assert.IsNotNull(registrationService);
        }

        [Test]
        public void SecurityKeyGenerationService_TestsIfTheSecurityKeysAreGenerated_VerifiesThroughTheReturnedValues()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];

            Tuple<ApiKey, SecretKey,DateTime> securityKeys = registrationService.CreateSystemGeneratedKey(1);

            Assert.IsNotNull(securityKeys);
            Assert.IsNotNull(securityKeys.Item1);
            Assert.IsNotNull(securityKeys.Item2);
            Assert.IsNotNull(securityKeys.Item3);
        }

        [Test]
        [Category("Integration")]
        public void CreateSystemGeneratedSecurityPair_IfUserNameIsProvided_VerifyKeyPairIsReturnedAndPersistedSuccessfully()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var keys = registrationService.CreateSystemGeneratedKey(1);
            Assert.NotNull(keys);
            Assert.IsNotNullOrEmpty(keys.Item1.Value);
            Assert.IsNotNullOrEmpty(keys.Item2.Value);
            SecurityKeysPair persistedKeysPair = _securityKeysRepository.GetByApiKey(keys.Item1.Value);
            Assert.NotNull(persistedKeysPair);
            Assert.AreEqual(persistedKeysPair.UserId, 1);
            Assert.AreEqual(persistedKeysPair.SystemGenerated, true);
            Assert.AreEqual(persistedKeysPair.ApiKey, keys.Item1.Value);
            Assert.AreEqual(persistedKeysPair.SecretKey,keys.Item2.Value);
            Assert.IsNotNullOrEmpty(persistedKeysPair.KeyDescription);
            Assert.IsNotNullOrEmpty(persistedKeysPair.CreationDateTime.ToString());
        }

        [Test]
        [Category("Integration")]
        public void CreateUserGeneratedSecurityPair_IfAllRequiredParametersAreProvided_VerifyKeyPairIsReturnedAndPersistedSuccessfully()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var systemGeneratedKey = registrationService.CreateSystemGeneratedKey(1);
            List<string> securityKeyPermissions=new List<string>();
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            CreateUserGeneratedSecurityKeyPair command =
                new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                    DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                    DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
            Assert.NotNull(keys);
            Assert.IsNotNullOrEmpty(keys.ApiKey);
            Assert.IsNotNullOrEmpty(keys.SecretKey);
            SecurityKeysPair persistedKeysPair = _securityKeysRepository.GetByApiKey(keys.ApiKey);
            Assert.NotNull(persistedKeysPair);
            Assert.AreEqual(persistedKeysPair.UserId, 1);
            Assert.AreEqual(persistedKeysPair.SystemGenerated, false);
            Assert.AreEqual(persistedKeysPair.ApiKey, keys.ApiKey);
            Assert.AreEqual(persistedKeysPair.SecretKey, keys.SecretKey);
            Assert.IsNotNullOrEmpty(persistedKeysPair.KeyDescription);
            Assert.IsNotNullOrEmpty(persistedKeysPair.CreationDateTime.ToString());
            Assert.AreEqual(persistedKeysPair.EnableStartDate,true);
            Assert.AreEqual(persistedKeysPair.EnableEndDate, true);
            Assert.AreEqual(persistedKeysPair.EnableExpirationDate, true);
            Assert.AreEqual(persistedKeysPair.ExpirationDate, DateTime.Today.AddDays(1));
            Assert.AreEqual(persistedKeysPair.StartDate, DateTime.Today.AddDays(-2));
            Assert.AreEqual(persistedKeysPair.EndDate, DateTime.Today.AddDays(-1));
            ValidatePermissions(persistedKeysPair,securityKeyPermissions);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateUserGeneratedSecurityPair_IfNoPermissionIsAssigned_ArgumentNullExceptionShouldBeRaised()
        {
            ISecurityKeysApplicationService registrationService =
               (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var systemGeneratedKey = registrationService.CreateSystemGeneratedKey(1);
            List<string> securityKeyPermissions = new List<string>();
            CreateUserGeneratedSecurityKeyPair command =
                new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                    DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                    DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateUserGeneratedSecurityPair_IfKeyDescriptionAlreadyExists_ArgumentExceptionShouldBeRaised()
        {
            ISecurityKeysApplicationService registrationService =
               (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var systemGeneratedKey = registrationService.CreateSystemGeneratedKey(1);
            List<string> securityKeyPermissions = new List<string>();
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            CreateUserGeneratedSecurityKeyPair command =
                new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                    DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                    DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
            command=
               new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                   DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                   DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys1 = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
        }

        [Test]
        [Category("Integration")]
        public void UpdateUserGeneratedSecurityPair_UpdatePermissionsAndSomeDescription_VerifyKeyPairIsReturnedAndPersistedSuccessfully()
        {
            ISecurityKeysApplicationService registrationService =
               (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var systemGeneratedKey = registrationService.CreateSystemGeneratedKey(1);
            List<string> securityKeyPermissions = new List<string>();
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            CreateUserGeneratedSecurityKeyPair command =
                new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                    DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                    DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
            
            List<SecurityKeyPermissionsRepresentation> securityKeyPermissions2 = new List<SecurityKeyPermissionsRepresentation>();
            for (int i = 0; i < permissions.Count; i++)
            {
                if (i%2 == 0)
                {
                    securityKeyPermissions2.Add(new SecurityKeyPermissionsRepresentation(false, permissions[i]));
                }
                else
                {
                    securityKeyPermissions2.Add(new SecurityKeyPermissionsRepresentation(true, permissions[i]));
                }
            }

            UpdateUserGeneratedSecurityKeyPair update = new UpdateUserGeneratedSecurityKeyPair(keys.ApiKey, "456", false, false, true, "", "", securityKeyPermissions2.ToArray(), DateTime.Today.AddDays(3).ToString());
            registrationService.UpdateSecurityKeyPair(update,keys.ApiKey);
            Assert.NotNull(keys);
            Assert.IsNotNullOrEmpty(keys.ApiKey);
            Assert.IsNotNullOrEmpty(keys.SecretKey);
            SecurityKeysPair persistedKeysPair = _securityKeysRepository.GetByApiKey(keys.ApiKey);
            Assert.NotNull(persistedKeysPair);
            Assert.AreEqual(persistedKeysPair.UserId, 1);
            Assert.AreEqual(persistedKeysPair.SystemGenerated, false);
            Assert.AreEqual(persistedKeysPair.ApiKey, keys.ApiKey);
            Assert.AreEqual(persistedKeysPair.SecretKey, keys.SecretKey);
            Assert.AreEqual(persistedKeysPair.KeyDescription,"456");
            Assert.IsNotNullOrEmpty(persistedKeysPair.CreationDateTime.ToString());
            Assert.AreEqual(persistedKeysPair.EnableStartDate, false);
            Assert.AreEqual(persistedKeysPair.EnableEndDate, false);
            Assert.AreEqual(persistedKeysPair.EnableExpirationDate, true);
            Assert.AreEqual(persistedKeysPair.ExpirationDate, DateTime.Today.AddDays(3));
        }
        [Test]
        [Category("Integration")]
        public void CreateUserGeneratedSecurityPair_ReadAndDeleteIt_SecurityPairShouldGetDeleted()
        {
            ISecurityKeysApplicationService registrationService =
               (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var systemGeneratedKey = registrationService.CreateSystemGeneratedKey(1);
            List<string> securityKeyPermissions = new List<string>();
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(permissions[i].PermissionId);
            }
            CreateUserGeneratedSecurityKeyPair command =
                new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions,
                    DateTime.Today.AddDays(1).ToString(), DateTime.Today.AddDays(-2).ToString(),
                    DateTime.Today.AddDays(-1).ToString(), true, true, true, "123");
            var keys = registrationService.CreateUserGeneratedKey(command, systemGeneratedKey.Item1.Value);
            registrationService.DeleteSecurityKeyPair("123",systemGeneratedKey.Item1.Value);
            var getKeyPair = _securityKeysRepository.GetByKeyDescriptionAndUserId("123", 1);
            Assert.Null(getKeyPair);
            var getKeyPair1 = _securityKeysRepository.GetByApiKey(keys.ApiKey);
            Assert.Null(getKeyPair1);
        }

        /// <summary>
        /// Validate permissions
        /// </summary>
        /// <param name="keysPair"></param>
        /// <param name="permissions"></param>
        private void ValidatePermissions(SecurityKeysPair keysPair, List<string> permissions)
        {
            for (int i = 0; i < permissions.Count; i++)
            {
                Assert.IsTrue(keysPair.ValidatePermission(permissions[i]));
            }
        }
    }
}
