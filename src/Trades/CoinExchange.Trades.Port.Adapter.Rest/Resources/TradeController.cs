using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.Trades;
using CoinExchange.Trades.Application.Trades.Representation;
using CoinExchange.Trades.Domain.Model.Order;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    public class TradeController : ApiController
    {
        private TradeQueryService _tradeQueryService = null;
        
        public TradeController()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            _tradeQueryService = (TradeQueryService)applicationContext["tradeQueryService"];
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="offset">Result offset</param>
        /// <param name="type">Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// <param name="trades">Whether or not to include trades related to position in output (optional.  default = false)</param>
        /// <param name="start">Starting unix timestamp or trade tx id of results (optional.  exclusive)</param>
        /// <param name="end">Ending unix timestamp or trade tx id of results (optional.  inclusive)</param>
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradehistory")]
        [HttpPost]
        public IHttpActionResult GetTradeHistory(string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "")
        {
            try
            {
                List<Order> closedOrders = _tradeQueryService.GetTradesHistory(new TraderId(1), offset, type, trades, start, end);

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
        /// Returns orders of the user that have been filled/executed
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        [Route("trades/querytrades")]
        [HttpPost]
        public IHttpActionResult QueryTrades(string txId = "", bool includeTrades = false)
        {
            try
            {
                List<Order> trades = _tradeQueryService.QueryTrades(new TraderId(1), txId, includeTrades);

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
        /// <param name="pair"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        [Route("trades/recenttrades")]
        [HttpGet]
        public IHttpActionResult RecentTrades(string pair, string since = "")
        {
            try
            {
                try
                {
                    TradeListRepresentation trades = _tradeQueryService.GetRecentTrades(new TraderId(1), pair, since);

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
        [Route("trades/TradeVolume")]
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
