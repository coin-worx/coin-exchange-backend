using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the change in the state of any order
    /// </summary>
    public class OrderListener
    {
        /// <summary>
        /// Handles the OrderBook''s event in case the state of an Order changes
        /// </summary>
        /// <param name="?"></param>
        public void OnOrderChanged(Order.Order order)
        {
            
        }
    }
}
