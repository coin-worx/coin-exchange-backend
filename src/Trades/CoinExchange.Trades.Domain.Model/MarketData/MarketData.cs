using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace CoinExchange.Trades.Domain.Model.MarketData
{
    /// <summary>
    /// VO to represent market data
    /// </summary>
    public class MarketData
    {
        public MarketData(long volume, decimal price,string type)
        {
            Volume = volume;
            Price = price;
            Type = type;
        }

        public decimal Price { get; private set; }
        public long Volume { get; private set; }
        public string Type { get; private set; }
    }
}
