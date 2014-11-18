using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Controller for dealing with all the security keys services
    /// </summary>
    [RoutePrefix("v1")]
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
        [Route("private/user/api/create")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(SecurityKeyPair))]
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

        [HttpPost]
        [Route("private/user/api/update")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UpdateSecurityKey(UpdateUserGeneratedSecurityKeyPair command)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("CreateSecurityKey Call Recevied, parameters:");
                }
                return Ok(_securityKeysApplicationService.UpdateSecurityKeyPair(command, HeaderParamUtility.GetApikey(Request)));
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
        [Route("private/user/api/permissions")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(SecurityKeyRepresentation[]))]
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
        [Route("private/user/api/list")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(List<object>))]
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
        [Route("private/user/api/keydetail")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(SecurityKeyRepresentation))]
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

        /// <summary>
        /// Get available permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/api/delete")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(bool))]
        public IHttpActionResult DeleteSecurityKeyPair(string keyDescription)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("DeleteSecurityKeyPair Call Recevied, keyDescription:"+keyDescription);
                }
                _securityKeysApplicationService.DeleteSecurityKeyPair(keyDescription,
                    HeaderParamUtility.GetApikey(Request));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DeleteSecurityKeyPair Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DeleteSecurityKeyPair Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidDataException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DeleteSecurityKeyPair Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DeleteSecurityKeyPair Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
