using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deals with user tier levels status fucntionality
    /// </summary>
    [RoutePrefix("v1")]
    public class TierController:ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string ServerUploadFolder = "C:\\Temp";
        private IUserTierLevelApplicationService _userTierLevelApplicationService;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="userTierLevelApplicationService"></param>
        public TierController(IUserTierLevelApplicationService userTierLevelApplicationService)
        {
            _userTierLevelApplicationService = userTierLevelApplicationService;
        }

        /// <summary>
        /// apply for tier 1 verification
        /// </summary>
        /// <param name="param"> </param>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/tier1")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetVerifyForTier1([FromBody]Tier1Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier1 Call Recevied, parameters:" + param);
                }
                _userTierLevelApplicationService.ApplyForTier1Verification(new VerifyTier1Command(param.PhoneNumber,
                    HeaderParamUtility.GetApikey(Request),param.FullName,Convert.ToDateTime(param.DateOfBirth)));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// apply for tier 2 verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/tier2")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetVerifyForTier2([FromBody]Tier2Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:"+param);
                }
                _userTierLevelApplicationService.ApplyForTier2Verification(
                    new VerifyTier2Command(HeaderParamUtility.GetApikey(Request), param.AddressLine1, param.AddressLine2,
                        param.AddressLine3, param.State, param.City, param.ZipCode));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// apply for tier 3 verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/tier3")]
        [FilterIP]
        [Authorize]
        public async Task<IHttpActionResult> GetVerifyForTier3([FromBody]Tier3Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:"+param);
                }
                MemoryStream memoryStream = new MemoryStream();
                var provider = new MultipartFormDataStreamProvider(ServerUploadFolder);
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    file.CopyToAsync(memoryStream);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("GetVerifyForTier2 File Recevied");
                    }
                }
                _userTierLevelApplicationService.ApplyForTier3Verification(
                    new VerifyTier3Command(HeaderParamUtility.GetApikey(Request), param.Ssn, param.Nin,
                        param.DocumentType, param.FileName, memoryStream));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

       /// <summary>
       /// get users tier statuses
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        [Route("private/user/tiers")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetTierStatuses()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTierStatuses Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTierLevelStatuses(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier1 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier1")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetTier1Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier1Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier1Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier 2 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier2")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetTier2Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier2Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier2Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier 3 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier3")]
        [FilterIP]
        [Authorize]
        public IHttpActionResult GetTier3Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier2Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier3Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
