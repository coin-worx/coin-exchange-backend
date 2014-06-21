using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Application.MarketDataServices.Representation
{
    /// <summary>
    /// VO to represent Spread of market data
    /// </summary>
    public class Spread
    {
        public decimal Ask { get; private set; }
        public decimal Bid { get; private set; }
        public DateTime DateTime { get; private set; }
        public decimal Difference { get; private set; }

        /// <summary>
        /// paramterized constructor
        /// </summary>
        /// <param name="ask"></param>
        /// <param name="bid"></param>
        /// <param name="dateTime"></param>
        public Spread(decimal ask, decimal bid, DateTime dateTime)
        {
            Ask = ask;
            Bid = bid;
            DateTime = dateTime;
            Difference = ask-bid;
        }
    }
}
