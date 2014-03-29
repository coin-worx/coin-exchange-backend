using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// Contains all the info to return against ticker info
    /// </summary>
    public class Pair
    {
        public Pair(string pairName, MarketData ask, MarketData bid, MarketData last, decimal[] volume, decimal[] weightedVolumeAverage, int[] numberOfTrades, decimal[] low, decimal[] high, decimal openingPrice)
        {
            PairName = pairName;
            Ask = ask;
            Bid = bid;
            Last = last;
            Volume = volume;
            WeightedVolumeAverage = weightedVolumeAverage;
            NumberOfTrades = numberOfTrades;
            Low = low;
            High = high;
            OpeningPrice = openingPrice;
        }

        public string PairName { get; private set; }
        public MarketData Ask { get; private set; }
        public MarketData Bid { get; private set; }
        public MarketData Last { get; private set; }
        public decimal[] Volume { get; private set; }
        public decimal[] WeightedVolumeAverage { get; private set; }
        public int[] NumberOfTrades { get; private set; }
        public decimal[] Low { get; private set; }
        public decimal[] High { get; private set; }
        public decimal OpeningPrice { get;private set; }
    }
}
