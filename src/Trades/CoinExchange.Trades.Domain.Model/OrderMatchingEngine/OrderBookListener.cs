using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the changes in the OrderBook. Will publish the OrderBook as a snapshot to the Messaging Queue when a 
    /// change occurs in the OrderBook
    /// </summary>
    [Serializable]
    public class OrderBookListener : IOrderBookListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
        /// <summary>
        /// Handles the event that signifies that the OrderBook has changed
        /// </summary>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            OutputDisruptor.Publish(orderBook);
            Log.Debug("OrderBook changed for Currency pair: " + orderBook.CurrencyPair);
        }
    }
}
