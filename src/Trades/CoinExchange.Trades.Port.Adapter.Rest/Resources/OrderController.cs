using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.ReadModel.DTO;

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
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Route("orders/cancelorder")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult CancelOrder([FromBody]string orderId)
        {
            try
            {
                if (orderId != string.Empty)
                {
                    try
                    {
                        //TODO: Fetch TraderId from api signature provided in header. Remove the Constant value of the GetSecretKey
                        return Ok(_orderApplicationService.CancelOrder(
                                    new CancelOrderCommand(new OrderId(int.Parse(orderId)), new TraderId(Int32.Parse(Constants.GetTraderId("123456789"))))));
                    }
                    catch (Exception exception)
                    {
                        return InternalServerError(exception);
                    }
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
                                // ToDo: Need to perform check on the API key and then provide the corresponding TraderId
                                order.Side, order.Pair,order.Volume,Constants.GetTraderId("123456789"))));
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
                // ToDo: TraderId should be retreived after authorizing the API Key and getting the TraderId if API Key is valid
                object value = _orderQueryService.GetOpenOrders(new TraderId(int.Parse(Constants.GetTraderId("123456789"))),
                    queryOpenOrdersParams.IncludeTrades, queryOpenOrdersParams.UserRefId);

                if (value is List<OrderRepresentation>)
                {
                    List<OrderRepresentation> openOrderRepresentation = (List<OrderRepresentation>) value;
                    return Ok<List<OrderRepresentation>>(openOrderRepresentation);
                }
                else if (value is List<OrderReadModel>)
                {
                    List<OrderReadModel> openOrderList = (List<OrderReadModel>) value;
                    return Ok<List<OrderReadModel>>(openOrderList);
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
                // ToDo: Get the Trader ID after authentication using API key.
                object orders = _orderQueryService.GetClosedOrders(new TraderId(int.Parse(Constants.GetTraderId("123456789"))),
                    closedOrdersParams.IncludeTrades, closedOrdersParams.UserRefId, closedOrdersParams.StartTime,
                    closedOrdersParams.EndTime, closedOrdersParams.Offset, closedOrdersParams.CloseTime);

                if (orders is List<OrderRepresentation>)
                {
                    List<OrderRepresentation> openOrderList = (List<OrderRepresentation>)orders;
                    return Ok<List<OrderRepresentation>>(openOrderList);
                }
                else if (orders is List<OrderReadModel>)
                {
                    List<OrderReadModel> openOrderList = (List<OrderReadModel>)orders;
                    return Ok<List<OrderReadModel>>(openOrderList);
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
