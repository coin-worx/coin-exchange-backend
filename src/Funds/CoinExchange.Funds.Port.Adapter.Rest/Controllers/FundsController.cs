using System;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model;
using CoinExchange.Funds.Port.Adapter.Rest.Services;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Funds.Port.Adapter.Rest.Controllers
{
    /// <summary>
    /// Serves the requests that need a command to get fetched, like the data for a particular user, portfolio or balance details
    /// </summary>
    public class FundsController : ApiController
    {
        /// <summary>
        /// Gives access to the service responsible for carrying out operations for the Private Controller
        /// </summary>
        private FundsRepository _privateRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FundsController()
        {
            _privateRepository = new FundsRepository();
        }

        /// <summary>
        /// Returns the Account balance for a number of currencies for a particular user
        /// </summary>
        /// <returns></returns>
        [Route("private/balance")]
        [HttpPost]
        public IHttpActionResult PostBalance([FromBody]TraderId traderId, string assetClass = null, string asset = null)
        {
            try
            {
                AccountBalance[] accountBalances = _privateRepository.GetAccBalance();

                if (accountBalances != null)
                {
                    return Ok<AccountBalance[]>(accountBalances);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Returns the queried Ledger Info for all or the specified ledger for the given Trader
        /// </summary>
        /// <returns></returns>
        [Route("private/ledgerinfo")]
        [HttpPost]
        // ToDo: Need to verify this call by sending data from URL
        public IHttpActionResult PostLedger([FromBody]TraderId traderId, [FromBody]LedgerInfoRequest ledger)
        {
            try
            {
                LedgerInfo[] ledgers = _privateRepository.GetLedgers();

                if (ledgers != null)
                {
                    return Ok<LedgerInfo[]>(ledgers);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}