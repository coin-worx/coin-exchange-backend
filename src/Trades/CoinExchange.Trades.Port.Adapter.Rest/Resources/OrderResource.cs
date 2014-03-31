using System;
using System.Collections.Generic;
using System.Web.Http;
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
    }
}
