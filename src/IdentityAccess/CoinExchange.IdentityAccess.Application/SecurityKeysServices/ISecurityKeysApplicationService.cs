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
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices
{
    /// <summary>
    /// Interface for Digital Signature Application Service
    /// </summary>
    public interface ISecurityKeysApplicationService
    {
        /// <summary>
        /// Generates new key
        /// </summary>
        /// <returns></returns>
        Tuple<ApiKey, SecretKey, DateTime> CreateSystemGeneratedKey(int userId);

        /// <summary>
        /// Item 1=Api Key
        /// Item 2=Secret Key
        /// </summary>
        /// <returns></returns>
       SecurityKeyPair CreateUserGeneratedKey(CreateUserGeneratedSecurityKeyPair command,string apiKey);

        /// <summary>
        /// Update Security Key Pair Info
        /// </summary>
        bool UpdateSecurityKeyPair(UpdateUserGeneratedSecurityKeyPair updateCommand, string apiKey);

        /// <summary>
        /// Delete security key pair
        /// </summary>
        void DeleteSecurityKeyPair(string keyDescription, string systemGeneratedApiKey);

        /// <summary>
        /// Is the API key valid
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        bool ApiKeyValidation(string apiKey);

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns></returns>
        SecurityKeyPermissionsRepresentation[] GetPermissions();

        /// <summary>
        /// Get api keys list
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        object GetSecurityKeysPairList(string apiKey);

        /// <summary>
        /// Get details of specific apikey
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        SecurityKeyRepresentation GetKeyDetails(string keyDescription, string apiKey);

    }
}
