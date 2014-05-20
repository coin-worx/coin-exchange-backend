using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// VO to represent depth tuple for ask and bids depth
    /// </summary>
    public class DepthTupleRepresentation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="bidDepth"></param>
        /// <param name="askDepth"></param>
        public DepthTupleRepresentation(DepthTuple[] bidDepth, DepthTuple[] askDepth)
        {
            BidDepth = bidDepth;
            AskDepth = askDepth;
        }

        public DepthTuple[] AskDepth { get; private set; }
        public DepthTuple[] BidDepth { get; private set; }
    }
}
