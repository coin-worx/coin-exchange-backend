using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.Trades;
using CoinExchange.Trades.Application.Trades.Representation;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;
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
                List<Order> closedOrders = _tradeQueryService.GetTradesHistory(new TraderId(1), tradeHistoryParams.Offset,
                    tradeHistoryParams.Type, tradeHistoryParams.Trades, tradeHistoryParams.Start, tradeHistoryParams.End);

                if (closedOrders != null)
                {
                    return Ok<List<Order>>(closedOrders);
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
                List<Order> trades = _tradeQueryService.QueryTrades(new TraderId(1), queryTradeParams.TxId, 
                    queryTradeParams.IncludeTrades);

                if (trades != null)
                {
                    return Ok<List<Order>>(trades);
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
                TradeListRepresentation trades = _tradeQueryService.GetRecentTrades(new TraderId(1),
                                                                                    currencyPair, since);

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
            {
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
