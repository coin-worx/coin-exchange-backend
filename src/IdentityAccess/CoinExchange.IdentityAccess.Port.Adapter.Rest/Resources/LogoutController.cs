using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    public class LogoutController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ILogoutApplicationService _logoutApplicationService;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="logoutApplicationService"></param>
        public LogoutController(ILogoutApplicationService logoutApplicationService)
        {
            _logoutApplicationService = logoutApplicationService;
        }

        /// <summary>
        /// Call for activating user account
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("admin/logout")]
        [FilterIP]
        public IHttpActionResult Logout([FromBody]LogoutParams param)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Logout Call Recevied, parameters:" + param);
                }
                return
                    Ok(_logoutApplicationService.Logout(new LogoutCommand(param.ApiKey, param.SecretKey)));
            }
            catch (InstanceNotFoundException exception)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("Logout Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("Logout Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("Logout Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}