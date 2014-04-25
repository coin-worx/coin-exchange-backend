using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.DomainEvents
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
