using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the best bid and best ask on the Order Book
    /// </summary>
    public class BBOListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// handles the event when the BestBid/best Ask on an OrderBook changes
        /// </summary>
        public void OnBBOChange(LimitOrderBook orderBook)
        {
            // ToDo: Need to figure whether to send the complete book here and extract the best bid and ask or send it from
            // the OrderBook
            Log.Debug("OrderBook received for currency pair: " + orderBook.CurrencyPair);
        }
    }
}
