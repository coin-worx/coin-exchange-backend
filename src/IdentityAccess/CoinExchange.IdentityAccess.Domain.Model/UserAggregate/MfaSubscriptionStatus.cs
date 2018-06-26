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


﻿
namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies the Status of a subscription
    /// </summary>
    public class MfaSubscriptionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus(int userId, MfaSubscription mfaSubscription, bool enabled)
        {
            UserId = userId;
            MfaSubscription = mfaSubscription;
            Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus(string apiKey, MfaSubscription mfaSubscription, bool enabled)
        {
            ApiKey = apiKey;
            MfaSubscription = mfaSubscription;
            Enabled = enabled;
        }

        /// <summary>
        /// Primary key of database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// API key in case the subscription is for an API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// MFA Subscription
        /// </summary>
        public MfaSubscription MfaSubscription { get; private set; }

        /// <summary>
        /// Is subscription enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
