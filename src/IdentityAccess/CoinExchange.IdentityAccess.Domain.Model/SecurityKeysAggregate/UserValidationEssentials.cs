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

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// The pair of the API and Secret Key along with the Session logout item mentioned by the user themselves, returned when
    /// the user logs in for a session
    /// </summary>
    public class UserValidationEssentials
    {
        /// <summary>
        /// Parametrized Constructor
        /// </summary>
        /// <param name="securityKeys"> </param>
        /// <param name="sessionLogoutTime"></param>
        public UserValidationEssentials(Tuple<ApiKey, SecretKey,DateTime> securityKeys, TimeSpan sessionLogoutTime)
        {
            ApiKey = securityKeys.Item1.Value;
            SecretKey = securityKeys.Item2.Value;
            SessionLogoutTime = sessionLogoutTime;
            LastLogin = securityKeys.Item3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserValidationEssentials(bool loginSuccessful, string message, string apiKey, string secretKey, TimeSpan sessionLogoutTime, DateTime lastLogin)
        {
            LoginSuccessful = loginSuccessful;
            Message = message;
            ApiKey = apiKey;
            SecretKey = secretKey;
            SessionLogoutTime = sessionLogoutTime;
            LastLogin = lastLogin;
        }

        /// <summary>
        /// Is the login successful
        /// </summary>
        public bool LoginSuccessful { get; private set; }

        /// <summary>
        /// Response message after login attempt
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; private set; }

        /// <summary>
        /// Logout time mentioned by the user for which these keys are applicable after login
        /// </summary>
        public TimeSpan SessionLogoutTime { get; private set; }

        /// <summary>
        /// The time of last login
        /// </summary>
        public DateTime LastLogin { get; private set; }
    }
}
