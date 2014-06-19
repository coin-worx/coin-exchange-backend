using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Services;
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
    [RoutePrefix("v1")]
    public class OrderController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IOrderApplicationService _orderApplicationService;
        private IOrderQueryService _orderQueryService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderController(IOrderApplicationService orderApplicationService, IOrderQueryService orderQueryService,IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _orderApplicationService = orderApplicationService;
            _orderQueryService = orderQueryService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Cancel Order Call: OrderId="+orderId);
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
                if (log.IsDebugEnabled)
                {
                    log.Debug("Cancel Order Call: apikey=" + apikey);
                }
                if (orderId != string.Empty)
                {
                    TraderId traderId=new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                    return Ok(_orderApplicationService.CancelOrder(
                        new CancelOrderCommand(new OrderId(orderId), traderId)));

                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Cancel Order Call Error", exception);
                }
                return InternalServerError(exception);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Create Order Call: " + order);
            }
            try
            {
                if (order != null)
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
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Createl Order Call: ApiKey=" + apikey);
                    }
                    TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                    return
                        Ok(
                            _orderApplicationService.CreateOrder(new CreateOrderCommand(order.Price, order.Type,
                                // ToDo: Need to perform check on the API key and then provide the corresponding TraderId
                                order.Side, order.Pair,order.Volume, traderId.Id)));
                }
                return BadRequest();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Create Order Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Create Order Call Error", exception);
                }
                return InternalServerError(exception);
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
        public IHttpActionResult QueryOpenOrders([FromBody] string includeTrades)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Query Open Orders Call: IncludeTrades=" + includeTrades);
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
                if (log.IsDebugEnabled)
                {
                    log.Debug("Query Open Orders Call: ApiKey=" + apikey);
                }
                TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                object value = _orderQueryService.GetOpenOrders(traderId,
                    Convert.ToBoolean(includeTrades));

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
                
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Open Orders Call Error", exception);
                }
                return InternalServerError(exception);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Query Closed Orders Call: " + closedOrdersParams);
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
                if (log.IsDebugEnabled)
                {
                    log.Debug("Query Closed Orders Call: ApiKey=" + apikey);
                }
                TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                object orders = _orderQueryService.GetClosedOrders(traderId,
                    closedOrdersParams.IncludeTrades,closedOrdersParams.StartTime,
                    closedOrdersParams.EndTime);

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
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Closed Orders Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        [Route("orders/queryorders")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult QueryOrders([FromBody] string orderId)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Query Orders Call: OrderId=" + orderId);
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
                if (log.IsDebugEnabled)
                {
                    log.Debug("Query Orders Call: ApiKey=" + apikey);
                }
                TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                object orders = _orderQueryService.GetOrderById(traderId,new OrderId(orderId) );
                return Ok(orders);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Orders Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }
    }
}
