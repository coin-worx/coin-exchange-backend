using System;
using System.Threading;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Infrastructure.Services
{
    /// <summary>
    /// Stub implementation of order id generator service
    /// </summary>
    [Serializable]
    public class StubbedOrderIdGenerator:IOrderIdGenerator
    {
        public OrderId GenerateOrderId()
        {
            var orderid = Guid.NewGuid().ToString();
            return new OrderId(orderid);
        }
    }
}
