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
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands
{
    /// <summary>
    /// Command to update user generated security params
    /// </summary>
    public class UpdateUserGeneratedSecurityKeyPair
    {
        public UpdateUserGeneratedSecurityKeyPair()
        {
            
        }

        public string ApiKey { get; set; }
        public string KeyDescritpion { get; set; }
        public bool EnableStartDate { get; set; }
        public bool EnableEndDate { get; set; }
        public bool EnableExpirationDate { get; set; }
        public string EndDateTime { get; set; }
        public string StartDateTime { get; set; }
        public string ExpirationDateTime { get; set; }
        public SecurityKeyPermissionsRepresentation[] SecurityKeyPermissions { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="keyDescritpion"></param>
        /// <param name="enableStartDate"></param>
        /// <param name="enableEndDate"></param>
        /// <param name="enableExpirationDate"></param>
        /// <param name="endDateTime"></param>
        /// <param name="startDateTime"></param>
        /// <param name="securityKeyPermissions"></param>
        /// <param name="expirationDateTime"></param>
        public UpdateUserGeneratedSecurityKeyPair(string apiKey, string keyDescritpion, bool enableStartDate, bool enableEndDate, bool enableExpirationDate, string endDateTime, string startDateTime, SecurityKeyPermissionsRepresentation[] securityKeyPermissions, string expirationDateTime)
        {
            ApiKey = apiKey;
           // SystemGeneratedApiKey = systemGeneratedApiKey;
            KeyDescritpion = keyDescritpion;
            EnableStartDate = enableStartDate;
            EnableEndDate = enableEndDate;
            EnableExpirationDate = enableExpirationDate;
            EndDateTime = endDateTime;
            StartDateTime = startDateTime;
            SecurityKeyPermissions = securityKeyPermissions;
            ExpirationDateTime = expirationDateTime;
        }

        /// <summary>
        /// Validate the command
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            for (int i = 0; i < SecurityKeyPermissions.Length; i++)
            {
                //validate atleast one permission is assigned
                if (SecurityKeyPermissions[i].Allowed)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
