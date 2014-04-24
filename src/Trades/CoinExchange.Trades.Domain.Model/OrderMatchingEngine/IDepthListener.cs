using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the depth changes for a particular currency
    /// </summary>
    public interface IDepthListener
    {
        /// <summary>
        /// Handles the change in the Depth of and OrderBook
        /// </summary>
        void OnDepthChanged(Depth depth);
    }
}
