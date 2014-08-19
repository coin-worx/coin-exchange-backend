using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.Common.Services;
using CoinExchange.Funds.Application.LedgerServices;
using CoinExchange.Funds.Application.WithdrawServices;

namespace CoinExchange.Funds.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Ledger Controller
    /// </summary>
    [RoutePrefix("v1")]
    public class LedgerController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ILedgerQueryService _ledgerQueryService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        /// <summary>
        /// Defautl Constructor
        /// </summary>
        /// <param name="ledgerQueryService"></param>
        /// <param name="apiKeyInfoAccess"></param>
        public LedgerController(ILedgerQueryService ledgerQueryService, IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _ledgerQueryService = ledgerQueryService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
        }

        /// <summary>
        /// Get all the ledgers for the provided currency for this user
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Route("funds/getledgers")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult GetAllLedgers([FromBody]string currency)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get All Ledgers call: Currency = {0}", currency));
            }
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Get All Ledgers Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(currency))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_ledgerQueryService.GetAllLedgers(accountId, currency));
                }
                return BadRequest("Currency is not provided.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get All Ledgers Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }
    }
}
