using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
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
        public IHttpActionResult ChangePassword([FromBody]ChangePasswordParams changePasswordParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ChangePassword Call Recevied, parameters:" + changePasswordParams);
                }
                _userApplicationService.ChangePassword(new ChangePasswordCommand(
                    HeaderParamUtility.GetApikey(Request), changePasswordParams.OldPassword,
                    changePasswordParams.NewPassword));
                return Ok("changed");
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
    }
}
