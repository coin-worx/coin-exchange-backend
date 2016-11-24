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
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deals with all services needed for user functionality
    /// </summary>
    [RoutePrefix("v1")]
    public class UserController:ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IUserApplicationService _userApplicationService;

        /// <summary>
        /// paramterized constructor
        /// </summary>
        /// <param name="userApplicationService"></param>
        public UserController(IUserApplicationService userApplicationService)
        {
            _userApplicationService = userApplicationService;
        }

        /// <summary>
        /// Call for activating user account
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("admin/user/activate")]
        [FilterIP]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ActivateUser([FromBody]UserActivationParam param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ActivateUser Call Recevied, parameters:" + param);
                }
                _userApplicationService.ActivateAccount(new ActivationCommand(param.ActivationKey, param.Username,
                    param.Password));
                return
                    Ok("activated");
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActivateUser Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActivateUser Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActivateUser Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActivateUser Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Request to cancel activation for an account
        /// </summary>
        /// <param name="cancelActivationParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("admin/user/cancelactivation")]
        [FilterIP]
        [ResponseType(typeof(bool))]
        public IHttpActionResult CancelUserActivation([FromBody]CancelActivationParams cancelActivationParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("CanceUserActivation Call Recevied, parameters:" + cancelActivationParams);
                }
                _userApplicationService.CancelAccountActivation(
                    new CancelActivationCommand(cancelActivationParams.ActivationKey));
                return
                    Ok("cancelled");
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CanceUserActivation Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CanceUserActivation Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CanceUserActivation Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("private/user/changepassword")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(ChangePasswordResponse))]
        public IHttpActionResult ChangePassword([FromBody]ChangePasswordParams changePasswordParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ChangePassword Call Recevied, parameters:" + changePasswordParams);
                }
                return Ok(_userApplicationService.ChangePassword(new ChangePasswordCommand(
                    HeaderParamUtility.GetApikey(Request), changePasswordParams.OldPassword,
                    changePasswordParams.NewPassword)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangePassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangePassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangePassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangePassword Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("private/user/lastlogin")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(DateTime))]
        public IHttpActionResult LastLogin()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("LastLogin Call Recevied");
                }
               return Ok( _userApplicationService.LastLogin(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LastLogin Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LastLogin Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LastLogin Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LastLogin Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("admin/user/forgotusername")]
        [FilterIP]
        [ResponseType(typeof(string))]
        public IHttpActionResult ForgotUsername([FromBody]ForgotUsernameParams forgotUsernameParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ForgotUsername Call Recevied, parameters:" + forgotUsernameParams);
                }
                return
                    Ok(_userApplicationService.ForgotUsername(new ForgotUsernameCommand(forgotUsernameParams.Email)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotUsername Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotUsername Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotUsername Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("admin/user/forgotpassword")]
        [FilterIP]
        [ResponseType(typeof(string))]
        public IHttpActionResult ForgotPassword([FromBody]ForgotPasswordParams forgotPasswordParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ForgotPassword Call Recevied, parameters:" + forgotPasswordParams);
                }
                return
                    Ok(_userApplicationService.ForgotPassword(new ForgotPasswordCommand(forgotPasswordParams.Email,
                        forgotPasswordParams.Username)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotPassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotPassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ForgotPassword Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("admin/user/resetpassword")]
        [FilterIP]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ResetPassword([FromBody]ResetPasswordParams resetPasswordParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ResetPassword Call Recevied, parameters:" + resetPasswordParams);
                }
                _userApplicationService.ResetPasswordByEmailLink(new ResetPasswordCommand(resetPasswordParams.Username,
                    resetPasswordParams.Password, resetPasswordParams.ResetPasswordCode));
                return
                    Ok("changed");
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ResetPassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ResetPassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ResetPassword Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ResetPassword Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("private/user/changesettings")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(ChangeSettingsResponse))]
        public IHttpActionResult ChangeSettings([FromBody]ChangeSettingsParams changeSettingsParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ChangeSettings Call Recevied, parameters:" + changeSettingsParams);
                }
                ;
                return Ok(_userApplicationService.ChangeSettings(new ChangeSettingsCommand(HeaderParamUtility.GetApikey(Request),
                    changeSettingsParams.Email, changeSettingsParams.PgpPublicKey, changeSettingsParams.Language,
                    changeSettingsParams.TimeZone, changeSettingsParams.IsDefaultAutoLogout,
                    changeSettingsParams.AutoLogoutMinutes)));
            }
            catch (InstanceNotFoundException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("private/user/accountsettings")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(AccountSettingsRepresentation))]
        public IHttpActionResult GetAccountSettings()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetAccountSettings Call Recevied.");
                }
                return Ok(_userApplicationService.GetAccountSettings(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InstanceNotFoundException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ChangeSettings Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/api/submitemailsettings")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(SubmitEmailSettingsResponse))]
        public IHttpActionResult SubmitEmailSettings(EmailSettingsParams emailSettingsParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Submit Email Settings Call Recevied");
                }
                string apikey = HeaderParamUtility.GetApikey(Request);
                if (!string.IsNullOrEmpty(apikey))
                {
                    return Ok(_userApplicationService.SubmitEmailSettings(new EmailSettingsCommand(apikey,
                        emailSettingsParams.AdministrativeEmails, emailSettingsParams.NewsLetterEmails)));
                }
                else
                {
                    throw new Exception("API Key not recieved");
                }
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/api/mfasettings")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(SubmitMfaSettingsResponse))]
        public IHttpActionResult SubmitMfaSettings(MfaSettingsParams mfaSettingsParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Submit Email Settings Call Recevied");
                }
                string apikey = HeaderParamUtility.GetApikey(Request);
                if (!string.IsNullOrEmpty(apikey))
                {
                    List<Tuple<string,string,bool>> mfaSettingsList = new List<Tuple<string, string, bool>>();
                    foreach (var setting in mfaSettingsParams.MfaSettingsList)
                    {
                        mfaSettingsList.Add(new Tuple<string, string, bool>(setting.MfaSubscriptionId, 
                            setting.MfaSubscriptionName,setting.Enabled));
                    }
                    return Ok(_userApplicationService.SubmitMfaSettings(new MfaSettingsCommand(mfaSettingsParams.ApiKeyMfa, 
                        mfaSettingsParams.ApiKeyPassword, apikey, mfaSettingsList)));
                }
                else
                {
                    throw new Exception("API Key not recieved");
                }
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Submit Email Settings Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
