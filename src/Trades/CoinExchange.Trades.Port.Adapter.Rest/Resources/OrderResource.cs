using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Trades.Application.Order;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.VOs;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Handles HTTP requests related to Orders
    /// </summary>
    public class OrderResource : ApiController
    {
        private OrderApplicationService _orderApplicationService;
        private OrderQueryService _orderQueryService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderResource()
        {
            _orderApplicationService = new OrderApplicationService();
        }

        /// <summary>
        /// private call to cancel user orders
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("trades/CancelOrder")]
        [HttpPost]
        public IHttpActionResult CancelOrder([FromBody]CancelOrderRequest request)
        {
            try
            {
                if (request != null)
                {
                    if (request.TraderId.Equals(string.Empty) || request.TxId.Equals(string.Empty))
                    {
                        return BadRequest();
                    }
                    return Ok(_orderApplicationService.CancelOrder(request));
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
        public IHttpActionResult OpenOrderList([FromBody]TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            try
            {
                // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
                // separated list
                List<Order> openOrderList = _orderQueryService.GetOpenOrders();

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
        public IHttpActionResult GetClosedOrders(bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            try
            {
                List<Order> closedOrders = _orderQueryService.GetClosedOrders(includeTrades, userRefId, startTime, endTime, offset,
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
