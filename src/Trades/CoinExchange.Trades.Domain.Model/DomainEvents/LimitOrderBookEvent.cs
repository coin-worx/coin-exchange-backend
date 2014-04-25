using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.Domain.Model.DomainEvents
{
    public static class LimitOrderBookEvent
    {
        public static event Action<LimitOrderBook> LimitOrderBookChanged;

        public static void Raise(LimitOrderBook limitOrderBook)
        {
            if (LimitOrderBookChanged != null)
            {
                LimitOrderBookChanged(limitOrderBook);
            }
        }
    }
}
