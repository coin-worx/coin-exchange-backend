using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to changes in the best Bid and Offer
    /// </summary>
    public interface IBBOListener
    {
        /// <summary>
        /// Handles the depth for bids and asks at the top of the book
        /// </summary>
        void OnBBOChange(DepthLevel bestBid, DepthLevel askDepthLevel);
    }
}
