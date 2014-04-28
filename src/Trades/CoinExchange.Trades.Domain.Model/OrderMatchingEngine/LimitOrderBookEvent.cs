using System;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
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
