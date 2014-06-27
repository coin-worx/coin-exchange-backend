using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Exchange snapshort event
    /// </summary>
    public static class ExchangeEssentialsSnapshortEvent
    {
        public static event Action<ExchangeEssentialsList> ExchangeSnapshot;

        /// <summary>
        /// Raise snaphsot event
        /// </summary>
        /// <param name="exchangeEssentialsList"></param>
        public static void Raise(ExchangeEssentialsList exchangeEssentialsList)
        {
            if (ExchangeSnapshot != null)
            {
                ExchangeSnapshot(exchangeEssentialsList);
            }
        }
    }
}
