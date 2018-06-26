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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    public class MockSecurityKeysRepository : ISecurityKeysRepository
    {
        private List<SecurityKeysPair> _securityKeysPairsList = new List<SecurityKeysPair>();

        /// <summary>
        /// Get by Key Desc
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SecurityKeysPair GetByKeyDescriptionAndUserId(string keyDescription, int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get by API key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public SecurityKeysPair GetByApiKey(string apiKey)
        {
            foreach (var securityKeysPair in _securityKeysPairsList)
            {
                if (securityKeysPair.ApiKey == apiKey)
                {
                    return securityKeysPair;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete by Security Pair
        /// </summary>
        /// <param name="securityKeysPair"></param>
        /// <returns></returns>
        public bool DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            return true;
        }

        /// <summary>
        /// Add Security KeysPair
        /// </summary>
        /// <param name="securityKeysPair"></param>
        public void AddSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            _securityKeysPairsList.Add(securityKeysPair);
        }

        SecurityKeysPair ISecurityKeysRepository.GetByApiKey(string apiKey)
        {
            foreach (var securityKeysPair in _securityKeysPairsList)
            {
                if (securityKeysPair.ApiKey == apiKey)
                {
                    return securityKeysPair;
                }
            }
            return null;
        }

        bool ISecurityKeysRepository.DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            return true;
        }
        
        public SecurityKeysPair GetByDescriptionAndApiKey(string description, string apiKey)
        {
            throw new NotImplementedException();
        }
        SecurityKeysPair ISecurityKeysRepository.GetByKeyDescriptionAndUserId(string keyDescription, int userId)
        {
            throw new NotImplementedException();
        }

        SecurityKeysPair ISecurityKeysRepository.GetByDescriptionAndApiKey(string description, string apiKey)
        {
            throw new NotImplementedException();
        }

        object ISecurityKeysRepository.GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
