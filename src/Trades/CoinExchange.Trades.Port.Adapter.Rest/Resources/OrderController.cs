using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Handles HTTP requests related to Orders
    /// </summary>
    public class OrderController : ApiController
    {
        private IOrderApplicationService _orderApplicationService;
        private IOrderQueryService _orderQueryService;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderController(IOrderApplicationService orderApplicationService, IOrderQueryService orderQueryService)
        {
            _orderApplicationService = orderApplicationService;
            _orderQueryService = orderQueryService;
        }
        /// <summary>
        /// Private call to cancel user orders
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        [Route("orders/cancelorder")]
        [Authorize]
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
        /// Private call for user to create order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Route("orders/createorder")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult CreateOrder([FromBody]CreateOrderParam order)
        {
            try
            {
                if (order != null)
                {
                    return
                        Ok(
                            _orderApplicationService.CreateOrder(new CreateOrderCommand(order.Price, order.Type,
                                order.Side, order.Pair,order.Volume,"")));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// Private call that returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. includeTrades(bool): Include trades as well in the response(optional)
        /// 2. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        [Route("orders/openorders")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult QueryOpenOrders([FromBody] QueryOpenOrdersParams queryOpenOrdersParams)
        {
            try
            {
                // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
                // separated list
                List<OrderRepresentation> openOrderList = _orderQueryService.GetOpenOrders(new TraderId(1),
                    queryOpenOrdersParams.IncludeTrades, queryOpenOrdersParams.UserRefId);

                if (openOrderList != null)
                {
                    return Ok<List<OrderRepresentation>>(openOrderList);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// Private call returns orders of the user that have been filled/executed
        /// Params:
        /// 1. includeTrades(bool): Include trades as well in the response(optional)
        /// 2. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        [Route("orders/closedorders")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult QueryClosedOrders([FromBody] QueryClosedOrdersParams closedOrdersParams)
        {
            try
            {
                List<OrderRepresentation> closedOrders = _orderQueryService.GetClosedOrders(new TraderId(1),
                    closedOrdersParams.IncludeTrades, closedOrdersParams.UserRefId, closedOrdersParams.StartTime,
                    closedOrdersParams.EndTime, closedOrdersParams.Offset, closedOrdersParams.CloseTime);

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
    }
}
