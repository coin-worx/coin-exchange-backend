using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model;
using CoinExchange.Funds.Domain.Model.Entities;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Funds.Infrastructure.Services;

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
        private FundsService _fundsRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FundsController()
        {
            _fundsRepository = new FundsService();
        }

        /// <summary>
        /// Returns the Account balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        [Route("funds/balance")]
        [HttpPost]
        public IHttpActionResult AccountBalance([FromBody]TraderId traderId, string assetClass = null, string asset = null)
        {
            try
            {
                AccountBalance[] accountBalances = _fundsRepository.GetAccountBalance();

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
        /// Returns the Trade balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        [Route("funds/tradebalance")]
        [HttpPost]
        public IHttpActionResult TradeBalance([FromBody]TraderId traderId, string assetClass = null, string asset = null)
        {
            try
            {
                TradeBalance tradebalance = _fundsRepository.GetTradeBalance();

                if (tradebalance != null)
                {
                    return Ok<TradeBalance>(tradebalance);
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
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. LedgerInfoRequest ledger[FromUri]: Member = AssetClass, Asset, Type, StartTime, EndTime, Offset (optional)
        /// </summary>
        /// <returns></returns>
        [Route("funds/ledgersinfo")]
        [HttpPost]
        public IHttpActionResult LedgerInfo([FromBody]TraderId traderId, [FromUri]LedgerInfoRequest ledger)
        {
            try
            {
                LedgerInfo[] ledgers = _fundsRepository.GetLedgers();

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

        /// <summary>
        /// Accepts a comma separated list of ledgers, if no id is provided, returns all the ledgers. If no ID is given, 
        /// behaves the same as a LedgersInfo call.
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. LedgerIds(int): comma separated list of ledgers (optional)
        /// </summary>
        /// <returns></returns>
        [Route("funds/fetchledgers")]
        [HttpPost]
        public IHttpActionResult FetchLedgers([FromBody]TraderId traderId, string ledgerIds = "")
        {
            try
            {
                // ToDo: In the next sprint related to business logc behind RESTful calls, need to split the ledgersIds comma
                // separated list
                LedgerInfo[] ledgers = _fundsRepository.GetLedgers();

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