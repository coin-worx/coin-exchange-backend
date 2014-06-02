using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deals with all services needed for user functionality
    /// </summary>
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
        [Route("user/activate")]
        public IHttpActionResult ActivateUser([FromBody]UserActivationParam param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ActivateUser Call Recevied, parameters:" + param);
                }
                return
                    Ok(_userApplicationService.ActivateAccount(new ActivationCommand(param.ActivationKey, param.UserName, param.Password)));
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
        [Route("user/cancelactivation")]
        public IHttpActionResult CancelUserActivation([FromBody]CancelActivationParams cancelActivationParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("CanceUserActivation Call Recevied, parameters:" + cancelActivationParams);
                }
                return
                    Ok(_userApplicationService.CancelAccountActivation(new CancelActivationCommand(cancelActivationParams.ActivationKey)));
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
        [Route("user/cancelactivation")]
        public IHttpActionResult ChangePassword([FromBody]ChangePasswordParams changePasswordParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("ChangePassword Call Recevied, parameters:" + changePasswordParams);
                }
                return
                    Ok(_userApplicationService.ChangePassword(new ChangePasswordCommand(
                        changePasswordParams.ApiKey,changePasswordParams.SecretKey, changePasswordParams.OldPassword, changePasswordParams.NewPassword)));
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
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActivateUser Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
