using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deals with regitration and account creation(sign up)
    /// </summary>
    public class RegistrationController:ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IRegistrationApplicationService _registrationApplicationService;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="registrationApplicationService"></param>
        public RegistrationController(IRegistrationApplicationService registrationApplicationService)
        {
            _registrationApplicationService = registrationApplicationService;
        }

        [HttpPost]
        [Route("admin/signup")]
        [FilterIP]
        public IHttpActionResult Register([FromBody]SignUpParam param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Register Call Recevied, parameters:"+param);
                }
                return
                    Ok(
                        _registrationApplicationService.CreateAccount(new SignupUserCommand(param.Email, param.Username,
                            param.Password, param.Country, param.TimeZone, param.PgpPublicKey)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Register Call Exception ",exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Register Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Register Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

    }
}
