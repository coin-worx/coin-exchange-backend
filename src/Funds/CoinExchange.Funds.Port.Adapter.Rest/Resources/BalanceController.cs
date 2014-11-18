using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Services;
using CoinExchange.Funds.Application.BalanceService;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;

namespace CoinExchange.Funds.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// balance controller resource
    /// </summary>
    [RoutePrefix("v1")]
    public class BalanceController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IBalanceQueryService _balanceQueryService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        public BalanceController(IBalanceQueryService balanceQueryService,IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _balanceQueryService = balanceQueryService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
        }

        /// <summary>
        /// Get balances of owner
        /// </summary>
        /// <returns></returns>
        [Route("funds/getbalances")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(List<BalanceDetails>))]
        public IHttpActionResult GetBalances()
        {
            try
            {
                // Get api key from header
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
                    log.Debug(string.Format("Get Balances Call: ApiKey = {0}", apikey));
                }
                int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                return Ok(_balanceQueryService.GetBalances(new AccountId(accountId)));
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get Balances Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }
    }
}
