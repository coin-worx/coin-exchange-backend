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
        private ITradeApplicationService _tradeQueryService;

        public TradeController(ITradeApplicationService tradeQueryService)
        {
            _tradeQueryService = tradeQueryService;
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
                List<OrderRepresentation> closedOrders = _tradeQueryService.GetTradesHistory(new TraderId(int.Parse(Constants.GetTraderId(apikey))), tradeHistoryParams.Offset,
                    tradeHistoryParams.Type, tradeHistoryParams.Trades, tradeHistoryParams.Start, tradeHistoryParams.End);

                if (closedOrders != null)
                {
                    return Ok<List<OrderRepresentation>>(closedOrders);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
                List<OrderRepresentation> trades = _tradeQueryService.QueryTrades(new TraderId(int.Parse(Constants.GetTraderId(apikey))), queryTradeParams.TxId, 
                    queryTradeParams.IncludeTrades);

                if (trades != null)
                {
                    return Ok<List<OrderRepresentation>>(trades);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
            try
            {
                var trades = _tradeQueryService.GetRecentTrades(currencyPair, since);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
            { //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (pair != string.Empty)
                {
                    return Ok(_tradeQueryService.TradeVolume(pair));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
  
    }
}
