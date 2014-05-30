using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
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
    }
}
