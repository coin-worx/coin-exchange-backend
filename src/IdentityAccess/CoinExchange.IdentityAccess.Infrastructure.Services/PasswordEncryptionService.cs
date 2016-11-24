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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using BCrypt.Net;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Service for Password Encryption
    /// </summary>
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private const string HashingValue = "]3s`!^^";

        /// <summary>
        /// Encrypts the given string password and returns the encrypted password as string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncryptPassword(string password)
        {
            // Using a hard-coded value with every password, so that if the databse is copromised by hackers, they don't know
            // the hard coded value
            return BCrypt.Net.BCrypt.HashPassword(password + HashingValue, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verify if the entered password is the same as the one in the database
        /// </summary>
        /// <returns></returns>
        public bool VerifyPassword(string passwordEntered, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordEntered + HashingValue, hash);
        }
    }
}
