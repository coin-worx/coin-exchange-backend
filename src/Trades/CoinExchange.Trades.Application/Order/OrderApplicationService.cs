using System;
using CoinExchange.Trades.Application.Order.Commands;
using CoinExchange.Trades.Application.Order.Representation;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.Order
{
    public class OrderApplicationService:IOrderApplicationService
    {
        public CancelOrderResponse CancelOrder(string txid)
        {
            //TODO: to be implemented
            throw new NotImplementedException();
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            Domain.Model.Order.Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, new OrderSpecification());
            //TODO:Publish the order to disruptor
            return new NewOrderRepresentation(order.Price, order.OrderType.ToString(), order.OrderSide.ToString(), order.Pair,
                order.OrderId.Id.ToString());
        }
    }
}
