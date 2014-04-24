using System;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Serves the operations for resources related to Order
    /// </summary>
    public class StubbedOrderApplicationService:IOrderApplicationService
    {
        /// <summary>
        /// Cancel the order of user
        /// </summary>
        /// <param name="cancelOrderCommand"></param>
        /// <returns></returns>
        public CancelOrderResponse CancelOrder(CancelOrderCommand cancelOrderCommand)
        {
            return new CancelOrderResponse(true, 2);
        }

        /// <summary>
        /// Cancel the order of user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            return new NewOrderRepresentation(orderCommand.Price, orderCommand.Type, orderCommand.Side, orderCommand.Pair,
                DateTime.Now.ToString(),orderCommand.Volume);
        }
    }
}
