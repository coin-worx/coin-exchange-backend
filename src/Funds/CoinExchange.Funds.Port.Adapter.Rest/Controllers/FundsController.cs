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
        [Route("private/accountbalance/{userId:int}/{assetName?}/{assetClass?}")]
        public IHttpActionResult GetBalance(int userId, string assetName = null, string assetClass = null)
        {
            try
            {
                AccountBalance[] accountBalances = _privateRepository.GetAccBalance(assetName, assetClass);

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
        /// Returns the Ledger Info for all the Ledgers
        /// </summary>
        /// <returns></returns>
        [Route("private/ledgerinfo")]
        public IHttpActionResult GetLedger()
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