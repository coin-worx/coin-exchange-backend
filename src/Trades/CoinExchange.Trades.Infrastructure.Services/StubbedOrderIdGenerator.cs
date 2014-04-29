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
        private static int _safeInstanceCount = 0;

        public OrderId GenerateOrderId()
        {
            var increment = Interlocked.Increment(ref _safeInstanceCount);
            return new OrderId(increment);
        }


    }
}
