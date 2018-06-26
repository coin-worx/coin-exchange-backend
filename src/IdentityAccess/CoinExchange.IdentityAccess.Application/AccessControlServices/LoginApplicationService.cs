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
using System.Management.Instrumentation;
using System.Security.Authentication;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices
{
    /// <summary>
    /// Serves the login operation(s)
    /// </summary>
    public class LoginApplicationService : ILoginApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUserRepository _userRepository;
        private IPasswordEncryptionService _passwordEncryptionService;
        private ISecurityKeysApplicationService _securityKeysApplicationService;
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private IMfaAuthorizationService _mfaAuthorizationService;

        /// <summary>
        /// Initializes with the UserRepository and PasswordEncryption service 
        /// </summary>
        public LoginApplicationService(IUserRepository userRepository, IPasswordEncryptionService passwordEncryptionService,
        ISecurityKeysApplicationService securityKeysApplicationService, IIdentityAccessPersistenceRepository persistenceRepository,
            IMfaAuthorizationService mfaAuthorizationService)
        {
            _userRepository = userRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _securityKeysApplicationService = securityKeysApplicationService;
            _persistenceRepository = persistenceRepository;
            _mfaAuthorizationService = mfaAuthorizationService;
        }

        /// <summary>
        /// Login call by the user, logs user in if username and password are correct
        /// </summary>
        /// <returns></returns>
        public UserValidationEssentials Login(LoginCommand loginCommand)
        {
            if (loginCommand != null && 
                !string.IsNullOrEmpty(loginCommand.Username) &&
                !string.IsNullOrEmpty(loginCommand.Password))
            {
                User user = _userRepository.GetUserByUserName(loginCommand.Username);
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException(
                            "This account is not yet activated. Please activate the account before trying to login.");
                    }
                    if (_passwordEncryptionService.VerifyPassword(loginCommand.Password, user.Password))
                    {
                        Tuple<bool, string> mfaAccessResponse =
                            _mfaAuthorizationService.AuthorizeAccess(user.Id, MfaConstants.Login, loginCommand.MfaCode);
                       
                        // Check if the user has susbcribed for mfa authorization. If yes, check the Mfa Code
                        if (mfaAccessResponse.Item1)
                        {
                            Tuple<ApiKey, SecretKey, DateTime> securityKeys =
                            _securityKeysApplicationService.CreateSystemGeneratedKey(user.Id);
                            user.LastLogin = DateTime.Now;
                            _persistenceRepository.SaveUpdate(user);
                            return new UserValidationEssentials(mfaAccessResponse.Item1, mfaAccessResponse.Item2, 
                                securityKeys.Item1.Value, securityKeys.Item2.Value, user.AutoLogout, user.LastLogin);
                        }
                        // If MFA authorization passes successfuly, return the user with the security keys
                        else
                        {
                            return new UserValidationEssentials(mfaAccessResponse.Item1, mfaAccessResponse.Item2,
                                                                null, null, user.AutoLogout, user.LastLogin);
                        }
                    }
                    else
                    {
                        throw new InvalidCredentialException(string.Format("Incorrect password for username: {0}",
                                                                           loginCommand.Username));
                    }
                }
                else
                {
                    throw new InvalidCredentialException(string.Format("Invalid username: {0}", loginCommand.Username));
                }
            }
            else
            {
                throw new InvalidCredentialException("Invalid Username and/or Password.");
            }
        }
    }
}
