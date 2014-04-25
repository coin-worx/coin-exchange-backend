using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.Domain.Model.DomainEvents
{
    public static class BBOEvent
    {
        public static event Action<BBO> BBOChanged;
        public static void Raise(BBO bbo)
        {
            if (BBOChanged != null)
            {
                BBOChanged(bbo);
            }
        }
    }
}
