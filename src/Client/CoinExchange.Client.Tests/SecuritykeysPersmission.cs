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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoinExchange.Client.Tests
{
    
    public class SecuritykeysPersmission
    {
        [JsonProperty(PropertyName = "KeyDescritpion")]
        public string KeyDescritpion { get; set; }
        [JsonProperty(PropertyName = "EnableStartDate")]
        public bool EnableStartDate { get; set; }
        [JsonProperty(PropertyName = "EnableEndDate")]
        public bool EnableEndDate { get; set; }
        [JsonProperty(PropertyName = "EnableExpirationDate")]
        public bool EnableExpirationDate { get; set; }
        [JsonProperty(PropertyName = "EndDateTime")]
        public string EndDateTime { get; set; }
        [JsonProperty(PropertyName = "StartDateTime")]
        public string StartDateTime { get; set; }
        [JsonProperty(PropertyName = "ExpirationDateTime")]
        public string ExpirationDateTime { get; set; }
        [JsonProperty(PropertyName = "SecurityKeyPermissions")]
        public PermissionRepresentation[] SecurityKeyPermissions { get; set; }

        public SecuritykeysPersmission(string expirationDateTime, string startDateTime, string endDateTime, bool enableExpirationDate, bool enableEndDate, bool enableStartDate, string keyDescritpion,PermissionRepresentation[] securityKeyPermissions)
        {
            SecurityKeyPermissions = securityKeyPermissions;
            ExpirationDateTime = expirationDateTime;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            EnableExpirationDate = enableExpirationDate;
            EnableEndDate = enableEndDate;
            EnableStartDate = enableStartDate;
            KeyDescritpion = keyDescritpion;
       }
    }
}
