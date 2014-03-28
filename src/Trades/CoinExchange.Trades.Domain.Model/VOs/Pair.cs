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
        public string PairName { get; set; }
        public MarketData Ask { get; set; }
        public MarketData Bid { get; set; }
        public MarketData Last { get; set; }
        public decimal[] Volume=new decimal[2];
        public decimal[] WeightedVolumeAverage = new decimal[2];
        public int[] NumberOfTrades = new int[2];
        public decimal[] Low = new decimal[2];
        public decimal[] High = new decimal[2];
        public decimal OpeningPrice { get; set; }
    }
}
