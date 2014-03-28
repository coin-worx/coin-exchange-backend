using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Trades.Domain.Model.Entities;
using CoinExchange.Trades.Port.Adapter.RestService;

namespace CoinExchange.Rest.WebHost.Controllers
{
    /// <summary>
    /// Controller to serve requests related to Trades
    /// </summary>
    public class TradesRequestController : ApiController
    {
        /// <summary>
        /// Returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/openorderlist")]
        [HttpPost]
        public IHttpActionResult OpenOrderList([FromBody]TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            try
            {
                // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
                // separated list
                List<Order> openOrderList = new TradesRestService().OpenOrderList(traderId, includeTrades, userRefId);

                if (openOrderList != null)
                {
                    return Ok<List<Order>>(openOrderList);
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
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/closedorders")]
        [HttpPost]
        public IHttpActionResult GetClosedOrders([FromBody]TraderId traderId, bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            try
            {
                List<Order> closedOrders = new TradesRestService().GetClosedOrders(traderId, includeTrades, userRefId,
                    startTime, endTime, offset, closetime);

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
        /// <param name="offset">Result offset</param>
        /// <param name="type">Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// <param name="trades">Whether or not to include trades related to position in output (optional.  default = false)</param>
        /// <param name="start">Starting unix timestamp or trade tx id of results (optional.  exclusive)</param>
        /// <param name="end">Ending unix timestamp or trade tx id of results (optional.  inclusive)</param>
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradehistory")]
        [HttpPost]
        public IHttpActionResult GetTradeHistory(TraderId traderId, string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "")
        {
            try
            {
                List<Order> closedOrders = new TradesRestService().GetTradeHistory(traderId, offset, type, trades, start, end);

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
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        [Route("trades/querytrades")]
        [HttpPost]
        public IHttpActionResult FetchQueryTrades([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            try
            {
                List<Order> trades = new TradesRestService().FetchQueryTrades(traderId, txId, includeTrades);

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
        /// Returns orders of the user that have been filled/executed
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradebalance")]
        [HttpPost]
        public IHttpActionResult TradeBalance([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            try
            {
                List<Order> trades = new TradesRestService().TradeBalance(traderId, txId, includeTrades);

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
    }
}
