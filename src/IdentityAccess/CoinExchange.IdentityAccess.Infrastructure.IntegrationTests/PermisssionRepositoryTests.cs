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
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class PermisssionRepositoryTests:AbstractConfiguration
    {
        private IPermissionRepository _permissionRepository;

        //properties will be injected based on type
        public IPermissionRepository PermissionRepository
        {
            set { _permissionRepository = value; }
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
        public void ReadPermissionMasterData_IfPermissionDataExists_ThereShouldBeSevenPermissions()
        {
            IList<Permission> getPermissions = _permissionRepository.GetAllPermissions();
            Assert.NotNull(getPermissions);
            Assert.AreEqual(8,getPermissions.Count);
            VerifyPermssionValues(getPermissions);
        }

        /// <summary>
        /// Verify permission master data
        /// </summary>
        /// <param name="permissions"></param>
        private void VerifyPermssionValues(IList<Permission> permissions)
        {
            for (int i = 0; i < 8; i++)
            {
                switch (permissions[i].PermissionId)
                {
                    case "QF":
                        Assert.AreEqual("Query Funds", permissions[i].PermissionName);
                        break;
                    case "WF":
                        Assert.AreEqual("Withdraw Funds", permissions[i].PermissionName);
                        break;
                    case "QOOT":
                        Assert.AreEqual("Query Open Orders and Trades", permissions[i].PermissionName);
                        break;
                    case "QCOT":
                        Assert.AreEqual("Query Closed Orders and Trades", permissions[i].PermissionName);
                        break;
                    case "PO":
                        Assert.AreEqual("Place Order", permissions[i].PermissionName);
                        break;
                    case "CO":
                        Assert.AreEqual("Cancel Order", permissions[i].PermissionName);
                        break;
                    case "QLT":
                        Assert.AreEqual("Query Ledger Entries", permissions[i].PermissionName);
                        break;
                    case "DF":
                        Assert.AreEqual("Deposit Funds", permissions[i].PermissionName);
                        break;
                    default:
                        Assert.Fail("Does not exist in permissions:",permissions[i].PermissionId,permissions[i].PermissionName);
                        break;
                }
            }
        }
    }
}
