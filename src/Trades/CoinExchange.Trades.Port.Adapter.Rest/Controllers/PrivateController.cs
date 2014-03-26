using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoinExchange.Trades.Port.Adapter.Rest.Models;
using CoinExchange.Trades.Port.Adapter.Rest.Services;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.Port.Adapter.Rest.Controllers
{
    /// <summary>
    /// Serves the requests that need a command to get fetched, like the data for a particular user, portfolio or balance details
    /// </summary>
    public class PrivateController : ApiController
    {
        /// <summary>
        /// Gives access to the service responsible for carrying out operations for the Private Controller
        /// </summary>
        private PrivateRepository _privateRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PrivateController()
        {
            _privateRepository = new PrivateRepository();
        }

        /// <summary>
        /// Returns the Account balance for a number of currencies for a particular user
        /// </summary>
        /// <returns></returns>
        [Route("private/accountbalance")]
        public IHttpActionResult GetBalance()
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