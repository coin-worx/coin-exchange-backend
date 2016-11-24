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

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters to be taken for signup
    /// </summary>
    public class SignUpParam
    {
        //default constructor
        public SignUpParam()
        {
            
        }
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="pgpPublicKey"></param>
        public SignUpParam(string email, string username, string password, string country, TimeZone timeZone, string pgpPublicKey)
        {
            Email = email;
            Username = username;
            Password = password;
            Country = country;
            TimeZone = timeZone;
            PgpPublicKey = pgpPublicKey;
        }

        /// <summary>
        /// Email
        /// </summary>       
        public string Email { get;  set; }

        /// <summary>
        /// Username
        /// </summary>       
        public string Username { get;  set; }

        /// <summary>
        /// Password
        /// </summary>       
        public string Password { get;  set; }

        /// <summary>
        /// Country
        /// </summary>  
        public string Country { get;  set; }

        /// <summary>
        /// TimeZone
        /// </summary>       
        public TimeZone TimeZone { get;  set; }

        /// <summary>
        /// PGPPublicKey
        /// </summary>       
        public string PgpPublicKey { get;  set; }

        public override string ToString()
        {
            return string.Format("UserName:{0},Password{1},Email:{2},Country:{3},PgpPublic Key:{4},TimeZone:{5}",
                Username, Password.GetHashCode(), Email, Country, PgpPublicKey, TimeZone);
        }
    }
}
