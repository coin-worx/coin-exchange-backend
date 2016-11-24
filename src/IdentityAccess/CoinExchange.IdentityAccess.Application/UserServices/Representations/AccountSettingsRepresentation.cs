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
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Response for hte Account Settings for a user
    /// </summary>
    public class AccountSettingsRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountSettingsRepresentation"/> class.
        /// </summary>
        public AccountSettingsRepresentation(string username, string email, string pgpPublicKey, Language language, TimeZone timeZone, bool isDefaultAutoLogout, int autoLogoutMinutes)
        {
            Username = username;
            Email = email;
            PgpPublicKey = pgpPublicKey;
            Language = language;
            TimeZone = timeZone;
            IsDefaultAutoLogout = isDefaultAutoLogout;
            AutoLogoutMinutes = autoLogoutMinutes;

            LanguagesAvailable = Enum.GetNames(typeof (Language));
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// PGP Public Key
        /// </summary>
        public string PgpPublicKey { get; private set; }

        /// <summary>
        /// Language
        /// </summary>
        public Language Language { get; private set; }

        /// <summary>
        /// TimeZone
        /// </summary>
        public TimeZone TimeZone { get; private set; }

        /// <summary>
        /// Specifies if the Auto logout time is the Custom(specified by the user betwenn 2 and 240 minutes) or Default
        /// </summary>
        public bool IsDefaultAutoLogout { get; private set; }

        /// <summary>
        /// The minutes after which the User will logout automatically
        /// </summary>
        public int AutoLogoutMinutes { get; private set; }

        /// <summary>
        /// The list of languages availabel to offer
        /// </summary>
        public string[] LanguagesAvailable { get; private set; }
    }
}
