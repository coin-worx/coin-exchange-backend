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
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Interface for operations related to user and modifications in the user account
    /// </summary>
    public interface IUserApplicationService
    {
        /// <summary>
        /// Requests the change of password
        /// </summary>
        /// <param name="changePasswordCommand"> </param>
        /// <returns></returns>
        ChangePasswordResponse ChangePassword(ChangePasswordCommand changePasswordCommand);

        /// <summary>
        /// Request to activate account for which the user has already signed up
        /// </summary>
        /// <param name="activationCommand"> </param>
        /// <returns></returns>
        bool ActivateAccount(ActivationCommand activationCommand);

        /// <summary>
        /// Request to Cancel the Account Activation after the user has signed up for an account but has not activated the account
        /// </summary>
        /// <param name="cancelActivationCommand"> </param>
        /// <returns></returns>
        bool CancelAccountActivation(CancelActivationCommand cancelActivationCommand);

        /// <summary>
        /// Request to remind the user of their username. Returns the username to the caller
        /// </summary>
        /// <returns></returns>
        string ForgotUsername(ForgotUsernameCommand forgotUsernameCommand);

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="forgotPasswordCommand"> </param>
        /// <returns></returns>
        string ForgotPassword(ForgotPasswordCommand forgotPasswordCommand);

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="resetPasswordCommand"> </param>
        /// <returns></returns>
        bool ResetPasswordByEmailLink(ResetPasswordCommand resetPasswordCommand);

        /// <summary>
        /// Change Settings for the account
        /// </summary>
        /// <param name="changeSettingsCommand"></param>
        /// <returns></returns>
        ChangeSettingsResponse ChangeSettings(ChangeSettingsCommand changeSettingsCommand);

        /// <summary>
        /// Get settings for an account
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        AccountSettingsRepresentation GetAccountSettings(string apiKey);

        /// <summary>
        /// Get last login of user
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        DateTime LastLogin(string apiKey);

        /// <summary>
        /// Submit settings for notifications by email
        /// </summary>
        /// <param name="emailSettingsCommand"></param>
        /// <returns></returns>
        SubmitEmailSettingsResponse SubmitEmailSettings(EmailSettingsCommand emailSettingsCommand);

        /// <summary>
        /// Submit the MFA settings for a user
        /// </summary>
        /// <param name="mfaSettingsCommand"></param>
        /// <returns></returns>
        SubmitMfaSettingsResponse SubmitMfaSettings(MfaSettingsCommand mfaSettingsCommand);
    }
}
