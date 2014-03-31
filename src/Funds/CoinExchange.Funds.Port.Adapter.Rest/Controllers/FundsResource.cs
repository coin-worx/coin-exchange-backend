using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoinExchange.Funds.Application.Funds;
using CoinExchange.Funds.Domain.Model.Entities;
using CoinExchange.Funds.Domain.Model.VOs;

namespace CoinExchange.Funds.Port.Adapter.Rest.Controllers
{
    // ToDo: Need to further divide this service into granular peices for Funds bounded contexts. For now, we leave it as it is 
    /// <summary>
    /// Serves the requests for the resources related to funds
    /// </summary>
   // [RoutePrefix("api/funds")]
    public class FundsResource : ApiController
    {
        /// <summary>
        /// Gives access to the service responsible for carrying out operations for the Private Controller
        /// </summary>
        private FundsApplicationService _fundsApplicationService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FundsResource()
        {
            _fundsApplicationService = new FundsApplicationService();
        }

        /// <summary>
        /// Returns the Account balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        [Route("api/funds/balance")]
        [HttpPost]
        public IHttpActionResult GetFunds(int id)
        {
            try
            {
                AccountBalance[] accountBalances = _fundsApplicationService.GetAccountBalance();

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
        /// Returns the Account balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        [Route("~funds/balance")]
        [HttpPost]
        public IHttpActionResult AccountBalance([FromBody]TraderId traderId, string assetClass = null, string asset = null)
        {
            try
            {
                AccountBalance[] accountBalances = _fundsApplicationService.GetAccountBalance();

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
                TradeBalance tradebalance = _fundsApplicationService.GetTradeBalance();

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
                LedgerInfo[] ledgers = _fundsApplicationService.GetLedgers();

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
                LedgerInfo[] ledgers = _fundsApplicationService.GetLedgers();

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
