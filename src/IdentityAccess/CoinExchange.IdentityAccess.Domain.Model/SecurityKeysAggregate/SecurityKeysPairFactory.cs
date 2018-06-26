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

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Security key pair factory
    /// </summary>
    public class SecurityKeysPairFactory
    {
        /// <summary>
        /// Create system generated security key pair
        /// </summary>
        /// <param name="userId"> </param>
        /// <param name="securityKeysGeneration"></param>
        /// <returns></returns>
        public static SecurityKeysPair SystemGeneratedSecurityKeyPair(int userId, ISecurityKeysGenerationService securityKeysGeneration)
        {
            var keys = securityKeysGeneration.GenerateNewSecurityKeys();
            SecurityKeysPair securityKeysPair=new SecurityKeysPair(keys.Item1,keys.Item2,DateTime.Now.ToString(),userId,true);
            return securityKeysPair;
        }

        /// <summary>
        /// Create user generated api key
        /// </summary>
        /// <returns></returns>
        public static SecurityKeysPair UserGeneratedSecurityPair(int userId,string keyDescription,string apiKey,string secretKey,bool enableExpirationDate,string expirationDate,bool enableStartDate,string startDate,bool enableEndDate,string endDate,List<SecurityKeysPermission> keysPermissions ,ISecurityKeysRepository repository)
        {
            //check if key description already exist
            if (repository.GetByKeyDescriptionAndUserId(keyDescription,userId) != null)
            {
                throw new ArgumentException("The key description already exist");
            }
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(apiKey, secretKey, keyDescription, userId, false,keysPermissions);
            if (enableExpirationDate)
            {
                securityKeysPair.ExpirationDate = Convert.ToDateTime(expirationDate);
            }
            else
            {
                securityKeysPair.ExpirationDate = null;
            }
            if (enableStartDate)
            {
                securityKeysPair.StartDate = Convert.ToDateTime(startDate);
            }
            else
            {
                securityKeysPair.StartDate = null;
            }
            if (enableEndDate)
            {
                securityKeysPair.EndDate = Convert.ToDateTime(endDate);
            }
            else
            {
                securityKeysPair.EndDate = null;
            }
            securityKeysPair.EnableStartDate = enableStartDate;
            securityKeysPair.EnableEndDate = enableEndDate;
            securityKeysPair.EnableExpirationDate = enableExpirationDate;
            return securityKeysPair;
        }
    }
}
