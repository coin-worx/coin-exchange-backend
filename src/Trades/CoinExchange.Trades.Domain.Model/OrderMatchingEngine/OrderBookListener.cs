using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the changes in the OrderBook. Will publish the OrderBook as a snapshot to the Messaging Queue when a 
    /// change occurs in the OrderBook
    /// </summary>
    public class OrderBookListener
    {
        /// <summary>
        /// Handles the event that signifies that the OrderBook has changed
        /// </summary>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            
        }
    }
}
