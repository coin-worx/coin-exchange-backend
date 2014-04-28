using System;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
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
