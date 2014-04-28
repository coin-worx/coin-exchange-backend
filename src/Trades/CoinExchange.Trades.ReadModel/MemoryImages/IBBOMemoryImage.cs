using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Interface for in-memory staorage of the Best Bid and Best Offer on the read side
    /// </summary>
    public interface IBBOMemoryImage
    {
        /// <summary>
        /// handles the BBO fired as an event in case of the top of the order book changes
        /// </summary>
        /// <param name="bestBid"></param>
        /// <param name="bestAsk"></param>
        void OnBBOArrived(DepthLevel bestBid, DepthLevel bestAsk);
    }
}
