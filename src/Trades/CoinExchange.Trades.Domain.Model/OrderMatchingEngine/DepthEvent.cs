using System;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// DepthEvent
    /// </summary>
    [Serializable]
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
