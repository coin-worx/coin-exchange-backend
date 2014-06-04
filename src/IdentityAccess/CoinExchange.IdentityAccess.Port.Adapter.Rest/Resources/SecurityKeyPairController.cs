using System;
using System.IO;
using System.Security.Authentication;
using System.Web.Http;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Controller for dealing with all the security keys services
    /// </summary>
    public class SecurityKeyPairController:ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SecurityKeysApplicationService _securityKeysApplicationService;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="securityKeysApplicationService"></param>
        public SecurityKeyPairController(SecurityKeysApplicationService securityKeysApplicationService)
        {
            _securityKeysApplicationService = securityKeysApplicationService;
        }
        
        [HttpPost]
        [Route("user/api")]
        public IHttpActionResult CreateSecurityKey(CreateUserGeneratedSecurityKeyPair command)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("CreateSecurityKey Call Recevied, parameters:");
                }
                return Ok(_securityKeysApplicationService.CreateUserGeneratedKey(command, HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CreateSecurityKey Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CreateSecurityKey Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CreateSecurityKey Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CreateSecurityKey Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user/api")]
        public IHttpActionResult GetAvailablePermissions()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetAvailablePermissions Call Recevied");
                }
                return Ok(_securityKeysApplicationService.GetPermissions());
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetAvailablePermissions Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetAvailablePermissions Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetAvailablePermissions Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetAvailablePermissions Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user/apilist")]
        public IHttpActionResult GetUserSecurityKeys()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetUserSecurityKeys Call Recevied");
                }
                return Ok(_securityKeysApplicationService.GetSecurityKeysPairList(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user/apilist")]
        public IHttpActionResult GetSecurityKeyDetail(string keyDescription)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetUserSecurityKeys Call Recevied");
                }
                return Ok(_securityKeysApplicationService.GetKeyDetails(keyDescription,HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetUserSecurityKeys Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
