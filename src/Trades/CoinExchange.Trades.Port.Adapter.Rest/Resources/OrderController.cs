using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.Order;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Handles HTTP requests related to Orders
    /// </summary>
    public class OrderController : ApiController
    {
        private OrderApplicationService _orderApplicationService;
        private OrderQueryService _orderQueryService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderController()
        {
            _orderApplicationService = new OrderApplicationService();
            _orderQueryService = new OrderQueryService();
        }

        /// <summary>
        /// private call to cancel user orders
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        [Route("trades/CancelOrder")]
        [HttpPost]
        public IHttpActionResult CancelOrder([FromBody]string txid)
        {
            try
            {
                if (txid != string.Empty)
                {
                    return Ok(_orderApplicationService.CancelOrder(txid));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
        public IHttpActionResult OpenOrderList(bool includeTrades = false, string userRefId = "")
        {
            try
            {
                // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
                // separated list
                List<Order> openOrderList = _orderQueryService.GetOpenOrders(new TraderId(1), includeTrades, userRefId);

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
        /// 1. includeTrades(bool): Include trades as well in the response(optional)
        /// 2. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/closedorders")]
        [HttpPost]
        public IHttpActionResult GetClosedOrders(bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            try
            {
                List<Order> closedOrders = _orderQueryService.GetClosedOrders(new TraderId(1), includeTrades, userRefId, startTime, endTime, offset,
                    closetime);

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
