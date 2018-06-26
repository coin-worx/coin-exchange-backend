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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Authentication;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;

namespace CoinExchange.IdentityAccess.Application.RegistrationServices
{
    /// <summary>
    /// Registration Application Service
    /// </summary>
    public class RegistrationApplicationService : IRegistrationApplicationService
    {
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPasswordEncryptionService _passwordEncryptionService;
        private IActivationKeyGenerationService _activationKeyGenerationService;
        private IEmailService _emailService;
        private ITierRepository _tierRepository;
        private IUserRepository _userRepository;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public RegistrationApplicationService(IIdentityAccessPersistenceRepository persistenceRepository, 
        IPasswordEncryptionService passwordEncryptionService, IActivationKeyGenerationService activationKeyGenerationService
            , IEmailService emailService, ITierRepository tierRepository, IUserRepository userRepository)
        {
            _persistenceRepository = persistenceRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _activationKeyGenerationService = activationKeyGenerationService;
            _emailService = emailService;
            _tierRepository = tierRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Request from the client to create a new account
        /// </summary>
        /// <param name="signupUserCommand"> </param>
        /// <returns></returns>
        public string CreateAccount(SignupUserCommand signupUserCommand)
        {
            // Check the given credential strings
            if (signupUserCommand != null &&
                !string.IsNullOrEmpty(signupUserCommand.Email) &&
                !string.IsNullOrEmpty(signupUserCommand.Username) && 
                !string.IsNullOrEmpty(signupUserCommand.Password))
            {
                User userByUserName = _userRepository.GetUserByUserName(signupUserCommand.Username);
                if (userByUserName != null)
                {
                    throw new InvalidOperationException("Username already exists. Operation aborted");
                }
                User userByEmail = _userRepository.GetUserByEmail(signupUserCommand.Email);
                if (userByEmail != null)
                {
                    throw new InvalidOperationException("Email already exists. Operation aborted.");
                }

                // Hash the Password
                string hashedPassword = _passwordEncryptionService.EncryptPassword(signupUserCommand.Password);
                // Generate new activation key
                string activationKey = _activationKeyGenerationService.GenerateNewActivationKey();
                if (!string.IsNullOrEmpty(activationKey))
                {
                    // Create new user
                    User user = new User(signupUserCommand.Email, signupUserCommand.Username, hashedPassword,
                                         signupUserCommand.Country, signupUserCommand.TimeZone,
                                         signupUserCommand.PgpPublicKey, activationKey);
                    user.IsActivationKeyUsed = new IsActivationKeyUsed(false);
                    user.IsUserBlocked = new IsUserBlocked(false);
                    //persist so that user will be assigned an ID
                    _persistenceRepository.SaveUpdate(user);

                    IList<Tier> tiers = _tierRepository.GetAllTierLevels();
                    for (int i = 0; i < tiers.Count; i++)
                    {
                        user.AddTierStatus(Status.NonVerified, tiers[i]);
                    }

                     // Save to persistence
                    _persistenceRepository.SaveUpdate(user);
                    _emailService.SendPostSignUpEmail(user.Email, user.Username, user.ActivationKey,
                                                      user.AdminEmailsSubscribed);
                    // return Activation Key
                    return activationKey;
                }
                else
                {
                   throw new DataException("Not able to generate an activation key for New User request");
                }
            }
            else
            {
                throw new InvalidCredentialException("Email, username and/or Password not provided for reister new user request.");
            }
        }
    }
}
