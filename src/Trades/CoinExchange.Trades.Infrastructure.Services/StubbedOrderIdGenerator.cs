using System;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Infrastructure.Services
{
    /// <summary>
    /// Stub implementation of order id generator service
    /// </summary>
    public class StubbedOrderIdGenerator:IOrderIdGenerator
    {
        public OrderId GenerateOrderId()
        {
            return new OrderId((int)DateTime.Now.Ticks);
        }
    }
}
