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
using System.Security.Authentication;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices
{
    /// <summary>
    /// User Authentication Service
    /// </summary>
    public class UserAuthenticationService : IAuthenticationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISecurityKeysRepository _securityKeysRepository = null;
        private IUserRepository _userRepository = null;
        private IIdentityAccessPersistenceRepository _persistenceRepository;

        /// <summary>
        /// Initializes with the Secrutiy Keys Repository
        /// </summary>
        /// <param name="userRepository"> </param>
        /// <param name="securityKeysRepository"></param>
        public UserAuthenticationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository,IIdentityAccessPersistenceRepository persistenceRepository)
        {
            _securityKeysRepository = securityKeysRepository;
            _userRepository = userRepository;
            _persistenceRepository = persistenceRepository;
        }

        /// <summary>
        /// Authenticates a request
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool Authenticate(AuthenticateCommand command)
        {
            if (Nonce.IsValid(command.Nonce, command.Counter))
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.Apikey);
                string computedHash = CalculateHash(command.Apikey, command.Uri, securityKeysPair.SecretKey);
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Computed Hash:" + computedHash);
                    Log.Debug("Received Hash:" + command.Response);
                }
                if (String.CompareOrdinal(computedHash, command.Response) == 0)
                {
                    return ApiKeyValidation(command);
                }
                throw new InvalidCredentialException("API, URI and Secret Key Hash not found as expected.");
            }
            return false;
        }

        /// <summary>
        /// Validates the credentials related to the API Key
        /// </summary>
        /// <returns></returns>
        private bool ApiKeyValidation(AuthenticateCommand authenticateCommand)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(authenticateCommand.Apikey);

            if (securityKeysPair != null)
            {
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user != null)
                {
                    // If the keys are system generated, then we only need to check the session timeout for the user
                    if (securityKeysPair.SystemGenerated)
                    {
                        // Calculate for how much time is allowed in the session timeout for SystemGenerated key, saved in user
                        //int activeWindow = securityKeysPair.CreationDateTime.AddMinutes(user.AutoLogout.Minutes).Minute;
                        if (securityKeysPair.LastModified.AddMinutes(user.AutoLogout.Minutes) > DateTime.Now)
                        {
                            //update activity time
                            securityKeysPair.LastModified = DateTime.Now;
                            _persistenceRepository.SaveUpdate(securityKeysPair);
                            return true;
                        }
                        else
                        {
                            _securityKeysRepository.DeleteSecurityKeysPair(securityKeysPair);
                            throw new InvalidOperationException("Session timeout for the API Key.");
                        }
                    }
                    // Else we need to check the expiration date of the keys, and whetehr the user has permissions for 
                    // commencing with the desired operation
                    else
                    {
                        if (securityKeysPair.EnableExpirationDate)
                        {
                            if (securityKeysPair.ExpirationDate > DateTime.Now)
                            {
                                return CheckPermissions(authenticateCommand, securityKeysPair);
                            }
                            throw new InvalidOperationException("Key Expired");
                        }
                        else
                        {
                            return CheckPermissions(authenticateCommand, securityKeysPair);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}", "No user found against userId: ",
                                                                      securityKeysPair.UserId));
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} {1}",
                                                                  "No SecurityKeysPair found against the given API Key."));
            }
            return false;
        }

        /// <summary>
        /// Checks for permissions for the currently requested operation
        /// </summary>
        /// <param name="authenticateCommand"></param>
        /// <param name="securityKeysPair"></param>
        /// <returns></returns>
        private bool CheckPermissions(AuthenticateCommand authenticateCommand, SecurityKeysPair securityKeysPair)
        {
            bool permissionGained = false;
            if (authenticateCommand.Uri.Contains("/orders/cancelorder"))
            {
                return securityKeysPair.ValidatePermission(PermissionsConstant.Cancel_Order);
            }
            else if (authenticateCommand.Uri.Contains("/orders/openorders"))
            {
                return securityKeysPair.ValidatePermission(PermissionsConstant.Query_Open_Orders);
            }
            else if (authenticateCommand.Uri.Contains("/orders/closedorders"))
            {
                return securityKeysPair.ValidatePermission(PermissionsConstant.Query_Closed_Orders);
            }
            else if (authenticateCommand.Uri.Contains("/orders/createorder"))
            {
                return securityKeysPair.ValidatePermission(PermissionsConstant.Place_Order);
            }
            else if (authenticateCommand.Uri.Contains("trades/tradehistory") || authenticateCommand.Uri.Contains("trades/querytrades") || authenticateCommand.Uri.Contains("orders/queryorders") || authenticateCommand.Uri.Contains("trades/tradedetails"))
            {
                return securityKeysPair.ValidatePermission(PermissionsConstant.Query_Open_Orders) || securityKeysPair.ValidatePermission(PermissionsConstant.Query_Closed_Orders);
            }
            throw new InvalidOperationException("Permission not allowed for this operation.");
        }

        /// <summary>
        /// Calculates Hash
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="uri"></param>
        /// <param name="secretkey"></param>
        /// <returns></returns>
        private string CalculateHash(string apikey,string uri,string secretkey)
        {
            return String.Format("{0}:{1}:{2}", apikey, uri, secretkey).ToMD5Hash();
        }
        
        /// <summary>
        /// Generates Nounce
        /// </summary>
        /// <returns></returns>
        public string GenerateNonce()
        {
            return Nonce.Generate();
        }
    }
}
