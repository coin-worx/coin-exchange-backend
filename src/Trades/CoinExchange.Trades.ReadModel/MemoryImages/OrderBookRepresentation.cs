using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// VO for OrderBook Representation
    /// </summary>
    public class OrderBookRepresentation
    {
        public OrderRepresentationList Bids { get; private set; }
        public OrderRepresentationList Asks { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="asks"></param>
        /// <param name="bids"></param>
        public OrderBookRepresentation(OrderRepresentationList bids, OrderRepresentationList asks)
        {
            Asks = asks;
            Bids = bids;
        }
    }
}
