using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// VO for transmitting two depth levels(bids and asks)
    /// </summary>
    [Serializable]
    public class BBO
    {
        public DepthLevel BestBid;
        public DepthLevel BestAsk;
    }
}
