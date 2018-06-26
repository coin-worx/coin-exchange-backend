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
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Performs operations related to the User and User Account
    /// </summary>
    public class UserApplicationService : IUserApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUserRepository _userRepository = null;
        private ISecurityKeysRepository _securityKeysRepository = null;
        private IPasswordEncryptionService _passwordEncryptionService = null;
        private IIdentityAccessPersistenceRepository _persistenceRepository = null;
        private IEmailService _emailService = null;
        private IPasswordCodeGenerationService _passwordCodeGenerationService = null;

        /// <summary>
        /// Initializes with the User Repository
        /// </summary>
        public UserApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository,
            IPasswordEncryptionService passwordEncryptionService, IIdentityAccessPersistenceRepository persistenceRepository,
            IEmailService emailService, IPasswordCodeGenerationService passwordCodeGenerationService)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _persistenceRepository = persistenceRepository;
            _emailService = emailService;
            _passwordCodeGenerationService = passwordCodeGenerationService;
        }

        /// <summary>
        /// Request to change Password
        /// </summary>
        /// <param name="changePasswordCommand"> </param>
        /// <returns></returns>
        public ChangePasswordResponse ChangePassword(ChangePasswordCommand changePasswordCommand)
        {
            // Get the SecurityKeyspair instance related to this API Key
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(changePasswordCommand.ApiKey.Value);

            // See if the SecurityKeysPair instance exists for this API Key
            if (securityKeysPair != null)
            {
                // Get the User by specifying the Username in the SecurityKeysPair instance
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user != null)
                {
                    //// Make sure the session has not expired
                    //if (securityKeysPair.CreationDateTime.Add(user.AutoLogout) > DateTime.Now)
                    //{
                    // Check if the old password is the same as new one
                    if (_passwordEncryptionService.VerifyPassword(changePasswordCommand.OldPassword,
                                                                  user.Password))
                    {
                        // Create new password and save for the user in the database
                        string newEncryptedPassword =
                            _passwordEncryptionService.EncryptPassword(changePasswordCommand.NewPassword);
                        user.Password = newEncryptedPassword;
                        _persistenceRepository.SaveUpdate(user);
                        _emailService.SendPasswordChangedEmail(user.Email, user.Username, user.AdminEmailsSubscribed);
                        return new ChangePasswordResponse(true, "Password Change Successful");
                    }
                    else
                    {
                        throw new InvalidCredentialException(string.Format("Current password incorrect."));
                    }
                    //}
                    //else
                    //{
                    //    _securityKeysRepository.DeleteSecurityKeysPair(securityKeysPair);
                    //    throw new InvalidOperationException("Session Timeout expired for this API Key.");
                    //}
                }
                else
                {
                    throw new InstanceNotFoundException("User not found for the given SecurityKeysPair's Username.");
                }
            }
            else
            {
                throw new InstanceNotFoundException("SecurityKeysPair not found for the given API Key");
            }
        }

        /// <summary>
        /// Activates Account
        /// </summary>
        /// <param name="activationCommand"> </param>
        /// <returns></returns>
        public bool ActivateAccount(ActivationCommand activationCommand)
        {
            // Make sure all given credentials contain values
            if (!string.IsNullOrEmpty(activationCommand.ActivationKey) && 
                !string.IsNullOrEmpty(activationCommand.Username) && 
                !string.IsNullOrEmpty(activationCommand.Password))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByActivationKey(activationCommand.ActivationKey);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (activationCommand.Username == user.Username &&
                        _passwordEncryptionService.VerifyPassword(activationCommand.Password, user.Password))
                    {
                        // Activate the user only it either the activationKeyUser VO is null, or if the activation key has not been used
                        if (user.IsActivationKeyUsed == null || (user.IsActivationKeyUsed != null && !user.IsActivationKeyUsed.Value))
                        {
                            // Mark that this user is now activated
                            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
                            user.IsUserBlocked = new IsUserBlocked(false);
                            //update user's tier0 status to verified
                            user.UpdateTierStatus(TierLevelConstant.Tier0, Status.Verified);

                            // Update the user instance in repository
                            _persistenceRepository.SaveUpdate(user);
                            _emailService.SendWelcomeEmail(user.Email, user.Username, user.AdminEmailsSubscribed);
                            return true;
                        }
                        else
                        {
                            _emailService.SendReactivaitonNotificationEmail(user.Email, user.Username, user.AdminEmailsSubscribed);
                            throw new InvalidOperationException("The activation key has already been used ");
                        }
                    }
                    else
                    {
                        throw new InvalidCredentialException("Invalid Username or Password.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("No user instance found for the given activation key.");
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Activation Key, Username and/or Password not provided");
            }
            return false;
        }

        /// <summary>
        /// Cancel the account activation for this user
        /// </summary>
        /// <param name="cancelActivationCommand"> </param>
        /// <returns></returns>
        public bool CancelAccountActivation(CancelActivationCommand cancelActivationCommand)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(cancelActivationCommand.ActivationKey))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByActivationKey(cancelActivationCommand.ActivationKey);
                
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException("This account has already been activated. Operation aborted.");
                    }
                    _emailService.SendCancelActivationEmail(user.Email, user.Username, user.AdminEmailsSubscribed);
                    _userRepository.DeleteUser(user);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}",
                                                                      "No user exists against activation key: ",
                                                                      cancelActivationCommand.ActivationKey));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Activation Key not provided");
            }
        }

        /// <summary>
        /// Request for providing the Username by email
        /// </summary>
        /// <param name="forgotUsernameCommand"> </param>
        /// <returns></returns>
        public string ForgotUsername(ForgotUsernameCommand forgotUsernameCommand)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(forgotUsernameCommand.Email))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(forgotUsernameCommand.Email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException("Account is not activated yet. In case you have forgotten your " +
                                                            "username, you can cancel your account activation" +
                                                            " and then sign up for an account again.");
                    }
                    return user.Username;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}",
                                                                      "No user exists against email address: ", forgotUsernameCommand.Email));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
        }

        /// <summary>
        /// Request to reset the password in case it is forgotten by the user
        /// </summary>
        /// <param name="forgotPasswordCommand"> </param>
        /// <returns></returns>
        public string ForgotPassword(ForgotPasswordCommand forgotPasswordCommand)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(forgotPasswordCommand.Email) && (!string.IsNullOrEmpty(forgotPasswordCommand.Username)))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(forgotPasswordCommand.Email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException("Account is not activated yet. In case you have forgotten your " +
                                                            "password, you can cancel your account activation" +
                                                            " and then sign up for an account again.");
                    }
                    if (user.Username.Equals(forgotPasswordCommand.Username))
                    {
                        string newForgotPasswordCode = _passwordCodeGenerationService.CreateNewForgotPasswordCode();
                        user.AddForgotPasswordCode(newForgotPasswordCode);
                        _persistenceRepository.SaveUpdate(user);
                        return newForgotPasswordCode;
                    }
                    else
                    {
                        throw new InvalidCredentialException("Wrong username.");
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}", "No user could be found for Email: ",forgotPasswordCommand.Email));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
        }

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="resetPasswordCommand"> </param>
        /// <returns></returns>
        public bool ResetPasswordByEmailLink(ResetPasswordCommand resetPasswordCommand)
        {
            // Make sure given credential contains value
            if (resetPasswordCommand != null &&
                (!string.IsNullOrEmpty(resetPasswordCommand.Username))&&
                (!string.IsNullOrEmpty(resetPasswordCommand.Password))&&(!string.IsNullOrEmpty(resetPasswordCommand.ResetPasswordCode)))
            {
                // Get the user tied to this username
                User user = _userRepository.GetUserByUserName(resetPasswordCommand.Username);

                if (user != null)
                {
                    // Check if the password code is the same as expected and the validity period for this user's last 
                    // forgot password code validation has not expired
                    if (user.IsPasswordCodeValid(resetPasswordCommand.ResetPasswordCode))
                    {
                        // If code validation has not expired, update the password
                        user.Password = _passwordEncryptionService.EncryptPassword(resetPasswordCommand.Password);
                        _persistenceRepository.SaveUpdate(user);
                        return true;
                    }
                    else
                    {
                        // else, throw an exception
                        throw new InvalidOperationException(string.Format("{0} {1} {2}",
                                                                          "Validity Period to change password for user ",
                                                                          user.Username, " has expired."));
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}",
                                                                      "No user found for the given password code"));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Missing paramters");
            }
        }

        /// <summary>
        /// Cahnges the settigns for a user
        /// </summary>
        /// <param name="changeSettingsCommand"></param>
        /// <returns></returns>
        public ChangeSettingsResponse ChangeSettings(ChangeSettingsCommand changeSettingsCommand)
        {
            if (changeSettingsCommand != null)
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(changeSettingsCommand.ApiKey);
                if (securityKeysPair != null)
                {
                    User user = _userRepository.GetUserById(securityKeysPair.UserId);

                    if (user != null)
                    {
                        user.ChangeSettings(changeSettingsCommand.Email, changeSettingsCommand.PgpPublicKey,
                                            changeSettingsCommand.Language, changeSettingsCommand.TimeZone,
                                            changeSettingsCommand.IsDefaultAutoLogout,
                                            changeSettingsCommand.AutoLogoutMinutes);

                        _persistenceRepository.SaveUpdate(user);
                        return new ChangeSettingsResponse(true, "Change Successful");
                    }
                    else
                    {
                        throw new InstanceNotFoundException(string.Format("{0} {1}",
                                                                          "No User found for the given SecurityKeysPair: ",
                                                                          changeSettingsCommand.ApiKey));
                    }
                }
                else
                {
                    throw new InstanceNotFoundException("No SecurityKeysPair instance found for the given API key");
                }
            }
            else
            {
                throw new NullReferenceException("Given ChangeSettingsCommand is null.");
            }
        }

        /// <summary>
        /// Gets the account settings for the user
        /// </summary>
        public AccountSettingsRepresentation GetAccountSettings(string apiKey)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (securityKeysPair != null)
            {
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user != null)
                {
                    return new AccountSettingsRepresentation(user.Username, user.Email, user.PgpPublicKey, user.Language,
                        user.TimeZone, user.IsDefaultAutoLogout, user.AutoLogout.Minutes);
                }
                else
                {
                    throw new InstanceNotFoundException("No User instance found for the UserId associated with SecurityKeysPair");
                }
            }
            else
            {
                throw new InstanceNotFoundException("No SecurityKeysPair instance found for the given API key");
            }
        }

        /// <summary>
        /// Return last login of user.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public DateTime LastLogin(string apiKey)
        {
            return _securityKeysRepository.GetByApiKey(apiKey).CreationDateTime;
        }

        /// <summary>
        /// Submit notification settings for a user
        /// </summary>
        /// <param name="emailSettingsCommandy"> </param>
        /// <returns></returns>
        public SubmitEmailSettingsResponse SubmitEmailSettings(EmailSettingsCommand emailSettingsCommandy)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(emailSettingsCommandy.ApiKey);
            if (securityKeysPair != null)
            {
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user != null)
                {
                    user.SetAdminEmailSubscription(emailSettingsCommandy.AdminEmailsSubscribed);
                    user.SetNewsletterEmailSubscription(emailSettingsCommandy.NewsLetterEmailsSubscribed);
                    _persistenceRepository.SaveUpdate(user);
                    return new SubmitEmailSettingsResponse(true);
                }
                else
                {
                    throw new InstanceNotFoundException("No User instance found for the UserId associated with SecurityKeysPair");
                }
            }
            else
            {
                throw new InstanceNotFoundException("No SecurityKeysPair instance found for the given API key");
            }
        }

        /// <summary>
        /// Submit Mfa Subscription settings for user
        /// </summary>
        /// <returns></returns>
        public SubmitMfaSettingsResponse SubmitMfaSettings(MfaSettingsCommand mfaSettingsCommand)
        {
            // Find security keys
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(mfaSettingsCommand.ApiKey);
            if (securityKeysPair != null)
            {
                // If the securityKeysPair instance is system generated, then assign the TFA susbcriptions to the user instance
                if (securityKeysPair.SystemGenerated)
                {
                    // Find user
                    User user = _userRepository.GetUserById(securityKeysPair.UserId);
                    if (user != null)
                    {
                        user.AssignMfaSubscriptions(mfaSettingsCommand.MfaSettingsList);
                        _persistenceRepository.SaveUpdate(user);
                        return new SubmitMfaSettingsResponse(true, "Mfa Subscription successful");
                    }
                    // If user is not found
                    else
                    {
                        Log.Error(string.Format("No user instance found for UserId = {0}",
                                                securityKeysPair.UserId));
                        throw new NullReferenceException(string.Format("No user instance found for UserId = {0}",
                                                                       securityKeysPair.UserId));
                    }
                }
                // If the securityKeysPair instance is user generated, them assign the TFA subscriptions to the SecurityKeysPair instance
                else
                {
                    securityKeysPair.AssignMfaSubscriptions(mfaSettingsCommand.MfaSettingsList);
                    securityKeysPair.AssignMfaCode(mfaSettingsCommand.ApiKeyPassword);
                    _persistenceRepository.SaveUpdate(securityKeysPair);
                    return new SubmitMfaSettingsResponse(true, "Mfa Subscription successful");
                }
            }
            // If security keys pair instance is not found
            else
            {
                Log.Error(string.Format("No Security keys pair instance found for Api Key = {0}",
                    mfaSettingsCommand.ApiKey));
                throw new NullReferenceException(string.Format("No Security keys pair instance found for Api Key = {0}", 
                    mfaSettingsCommand.ApiKey));
            }
        }
    }
}
