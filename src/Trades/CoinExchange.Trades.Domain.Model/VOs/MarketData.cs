using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class MarketData
    {
        public MarketData(long volume, decimal price)
        {
            Volume = volume;
            Price = price;
        }

        public decimal Price { get; private set; }
        public long Volume { get; private set; }
    }
}
