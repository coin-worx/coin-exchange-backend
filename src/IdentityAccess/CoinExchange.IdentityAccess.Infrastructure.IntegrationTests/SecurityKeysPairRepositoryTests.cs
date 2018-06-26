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
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Services;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class SecurityKeysPairRepositoryTests:AbstractConfiguration
    {
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private ISecurityKeysRepository _securityKeysPairRepository;
        private IPermissionRepository _permissionRepository;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        //properties will be injected based on type
        public IIdentityAccessPersistenceRepository PersistenceRepository
        {
            set { _persistenceRepository = value; }
        }

        public ISecurityKeysRepository DigitalSignatureInfoRepository
        {
            set { _securityKeysPairRepository = value; }
        }

        public IPermissionRepository PermissionRepository
        {
            set { _permissionRepository = value; }
        }

        public IApiKeyInfoAccess ApiKeyInfoAccess
        {
            set { _apiKeyInfoAccess = value; }
        }

        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_PersistAndReadFromDatabaseByDescriptionKey_SavedAndReadInfoShouldBeSame()
        {
            SecurityKeysPair digitalSignatureInfo = new SecurityKeysPair("1", "123456", "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, null);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _securityKeysPairRepository.GetByKeyDescriptionAndUserId("1",1);
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription,"1");
            Assert.AreEqual(readInfo.ApiKey, "123456");
            Assert.AreEqual(readInfo.SecretKey, "secretkey");
            Assert.AreEqual(readInfo.UserId, digitalSignatureInfo.UserId);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }
        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_PersistAndReadFromDatabaseByApiKey_SavedAndReadInfoShouldBeSame()
        {
            SecurityKeysPair digitalSignatureInfo = new SecurityKeysPair("1", "123456", "secretkey",1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, null);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey, "123456");
            Assert.AreEqual(readInfo.SecretKey, "secretkey");
            Assert.AreEqual(readInfo.UserId, digitalSignatureInfo.UserId);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_AssignPermissionAndPersist_ItShouldGetSavedInTheDatabase()
        {
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();

            IList<SecurityKeysPermission> securityKeysPermissions=new List<SecurityKeysPermission>();
            for (int i = 0; i < 7; i++)
            {
                SecurityKeysPermission permission = new SecurityKeysPermission("123456", permissions[i], true);
                securityKeysPermissions.Add(permission);
            }
            SecurityKeysPair securityKeys = new SecurityKeysPair("1", "123456", "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, securityKeysPermissions);
            _persistenceRepository.SaveUpdate(securityKeys);
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey, "123456");
            Assert.AreEqual(readInfo.SecretKey, "secretkey");
            Assert.AreEqual(readInfo.UserId, securityKeys.UserId);
            Assert.AreEqual(readInfo.SystemGenerated, securityKeys.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, securityKeys.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, securityKeys.StartDate);
            Assert.AreEqual(readInfo.EndDate, securityKeys.EndDate);
            ValidatePermissions(readInfo,securityKeysPermissions.ToArray());
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_UpdatePermission_ItShouldGetUpdatedInTheDatabase()
        {
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();

            IList<SecurityKeysPermission> securityKeysPermissions = new List<SecurityKeysPermission>();
            for (int i = 0; i < 7; i++)
            {
                SecurityKeysPermission permission = new SecurityKeysPermission("123456", permissions[i], true);
                securityKeysPermissions.Add(permission);
            }
            SecurityKeysPair securityKeys = new SecurityKeysPair("1", "123456", "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, securityKeysPermissions);
            _persistenceRepository.SaveUpdate(securityKeys);
            //update permissions
            securityKeysPermissions[2].IsAllowed = false;
            securityKeysPermissions[4].IsAllowed = false;
            securityKeysPermissions[6].IsAllowed = false;
            securityKeys.UpdatePermissions(securityKeysPermissions.ToArray());
            //save them again
            _persistenceRepository.SaveUpdate(securityKeys);
            //read security key pair
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey, "123456");
            Assert.AreEqual(readInfo.SecretKey, "secretkey");
            Assert.AreEqual(readInfo.UserId, securityKeys.UserId);
            Assert.AreEqual(readInfo.SystemGenerated, securityKeys.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, securityKeys.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, securityKeys.StartDate);
            Assert.AreEqual(readInfo.EndDate, securityKeys.EndDate);
            ValidatePermissions(readInfo,securityKeysPermissions.ToArray());
        }

        [Test]
        [Category("Integration")]
        public void GetUserIDFromApiKey_IfKeyPairExists_UserIdWillBeReturned()
        {
            SecurityKeysPair digitalSignatureInfo = new SecurityKeysPair("1", "123456", "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, null);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            Assert.AreEqual(_apiKeyInfoAccess.GetUserIdFromApiKey("123456"), 1);
        }

        /// <summary>
        /// Validate permissions
        /// </summary>
        /// <param name="keysPair"></param>
        /// <param name="permissions"></param>
        private void ValidatePermissions(SecurityKeysPair keysPair,SecurityKeysPermission[] permissions)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                Assert.AreEqual(permissions[i].IsAllowed,
                    keysPair.ValidatePermission(permissions[i].Permission.PermissionId));
            }
        }

    }
}
