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
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/tier1")]
        public IHttpActionResult GetVerifyForTier1([FromBody]string phoneNumber)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier1 Call Recevied, parameters:" + phoneNumber);
                }
                _userTierLevelApplicationService.ApplyForTier1Verification(new VerifyTier1Command(phoneNumber,
                    HeaderParamUtility.GetApikey(Request)));
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
            catch (InvalidDataException exception)
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
        [Route("user/tier2")]
        public IHttpActionResult GetVerifyForTier2([FromBody]Tier2Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:");
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
            catch (InvalidDataException exception)
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
        [Route("user/tier3")]
        public async Task<IHttpActionResult> GetVerifyForTier3([FromBody]Tier3Param param)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                var provider = new MultipartFormDataStreamProvider(ServerUploadFolder);
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    file.CopyToAsync(memoryStream);
                }


                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:");
                }
                _userTierLevelApplicationService.ApplyForTier3Verification(
                    new VerifyTier3Command(HeaderParamUtility.GetApikey(Request), param.SocialSecurityNumber, param.Nin,
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
            catch (InvalidDataException exception)
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
        [Route("user/tiers")]
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
            catch (InvalidDataException exception)
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
    }
}
