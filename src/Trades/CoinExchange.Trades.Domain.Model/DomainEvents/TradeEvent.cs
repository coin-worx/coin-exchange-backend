using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.DomainEvents
{
    public static class TradeEvent
    {
        public static event Action<Trade> TradeOccured;
        public static void Raise(Trade trade)
        {
            if (TradeOccured != null)
            {
                TradeOccured(trade);
            }
        }
    }
}
