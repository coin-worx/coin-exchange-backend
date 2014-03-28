using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Trades.Domain.Model.Entities;
using CoinExchange.Trades.Infrastructure.Services.Services;

namespace CoinExchange.Trades.Port.Adapter.Rest.Controllers
{
    public class TradesController : ApiController
    {
        private TradesService _tradesService;

        public TradesController()
        {
            _tradesService = new TradesService();
        }

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
                List<Order> openOrderList = _tradesService.GetOpenOrders();

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
                List<Order> closedOrders = _tradesService.GetClosedOrders();

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
    }
}
