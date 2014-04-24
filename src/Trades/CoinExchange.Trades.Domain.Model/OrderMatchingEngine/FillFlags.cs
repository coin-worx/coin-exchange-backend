using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Signifies which of the two orders got matched and filled
    /// </summary>
    public enum FillFlags
    {
        /// <summary>
        /// Niether of the two orders got filled
        /// </summary>
        NetitherFilled,
        /// <summary>
        /// The order in the order book being matched got filled
        /// </summary>
        MatchedFilled,
        /// <summary>
        /// The incoming order from the trader got filled
        /// </summary>
        InboundFilled,
        /// <summary>
        /// Both mathing and incoming orders got filled
        /// </summary>
        BothFilled
    }
}
