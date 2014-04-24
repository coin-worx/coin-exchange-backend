using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to any changes in the OrderBook
    /// </summary>
    public interface IOrderBookListener
    {
        /// <summary>
        /// Handles events raised due to the change in the OrderBook
        /// </summary>
        void OnOrderBookChanged(LimitOrderBook orderBook);
    }
}
