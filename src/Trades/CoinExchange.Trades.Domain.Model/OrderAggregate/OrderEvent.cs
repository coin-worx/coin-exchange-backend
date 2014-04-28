using System;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    public static class OrderEvent
    {
        public static event Action<Order> OrderChanged;
        public static void Raise(Order order)
        {
            if (OrderChanged != null)
            {
                OrderChanged(order);
            }
        }
    }
}
