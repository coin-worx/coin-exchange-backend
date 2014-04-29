using System;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    [Serializable]
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
