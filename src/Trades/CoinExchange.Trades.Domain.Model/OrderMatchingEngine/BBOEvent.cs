using System;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
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
