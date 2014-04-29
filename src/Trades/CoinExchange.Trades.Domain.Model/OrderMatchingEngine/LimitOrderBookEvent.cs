using System;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Limit Order Book Event
    /// </summary>
    [Serializable]
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
