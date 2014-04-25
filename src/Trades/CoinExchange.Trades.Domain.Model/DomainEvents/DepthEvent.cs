using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.Domain.Model.DomainEvents
{
    public static class DepthEvent
    {
        public static event Action<Depth> DepthChanged;
        public static void Raise(Depth depth)
        {
            if (DepthChanged != null)
            {
                DepthChanged(depth);
            }
        }
    }
}
