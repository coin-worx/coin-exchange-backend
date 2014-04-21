using System;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Services;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Real implementation of order application service
    /// </summary>
    public class OrderApplicationService:IOrderApplicationService
    {
        public CancelOrderResponse CancelOrder(string txid)
        {
            //TODO: to be implemented
            throw new NotImplementedException();
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            IOrderIdGenerator orderIdGenerator = ContextRegistry.GetContext()["OrderIdGenerator"] as IOrderIdGenerator;
            Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, orderIdGenerator);
            //TODO:Publish the order to disruptor
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order));
            return new NewOrderRepresentation(order);
        }
    }
}
