using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    public class TradeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ITradeApplicationService _tradeApplicationService;

        public TradeController(ITradeApplicationService tradeApplicationService)
        {
            _tradeApplicationService = tradeApplicationService;
        }

        /// <summary>
        /// Private call that returns orders of the user that have been filled/executed
        /// TradeHistoryParams.Offset: Result offset (Optional)
        /// TradeHistoryParamsType.Type: Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// TradeHistoryParamsType.Trades: Whether or not to include trades related to position in output (optional.  default = false)
        /// TradeHistoryParamsType.Start: Starting unix timestamp or trade tx id of results (optional.  exclusive)
        /// TradeHistoryParamsType.End: Ending unix timestamp or trade tx id of results (optional.  inclusive)
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradehistory")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult GetTradeHistory([FromBody]TradeHistoryParams tradeHistoryParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Trade History Call Recevied:" + tradeHistoryParams);
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
                log.Debug("Api Key:" + apikey);
                var closedOrders = _tradeApplicationService.GetTradesHistory(new TraderId(Constants.GetTraderId(apikey)),tradeHistoryParams.Start, tradeHistoryParams.End);

                if (closedOrders != null)
                {
                    return Ok(closedOrders);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Trade History Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Private call that returns orders of the user that have been filled/executed
        /// QueryTradeParams.TxId: Comma separated list of txIds(Optional)
        /// QueryTradeParams.IncludeTrades: Whether or not to include the trades(Optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/querytrades")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult QueryTrades([FromBody]QueryTradeParams queryTradeParams)
        {
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
                var trades = _tradeApplicationService.QueryTrades(new TraderId(Constants.GetTraderId(apikey)), queryTradeParams.TxId, 
                    queryTradeParams.IncludeTrades);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Trades Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Public call to get recent trades
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="since"> </param>
        /// <returns></returns>
        [Route("trades/recenttrades")]
        [HttpGet]
        public IHttpActionResult RecentTrades(string currencyPair, string since = "")
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Recent Trades call: currency pair={0}",currencyPair));
            }
            try
            {
                var trades = _tradeApplicationService.GetRecentTrades(currencyPair, since);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Recent Trades Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// private call to request trade volume
        /// </summary>
        /// <param name="pair"> </param>
        /// <returns></returns>
        [Route("trades/tradevolume")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult TradeVolume([FromBody]string pair)
        {
            try
            {
                if (pair != string.Empty)
                {
                    return Ok(_tradeApplicationService.TradeVolume(pair));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// List of tradeable currency pairs
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradeablecurrencypair")]
        [HttpGet]
        public IHttpActionResult TradeableCurrencyPair()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Tradeabale Currency Pair Call");
            }
            try
            {
                return Ok(_tradeApplicationService.GetTradeableCurrencyPairs());
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Recent Trades Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }
  
    }
}
