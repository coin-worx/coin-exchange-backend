using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class MarketData
    {
        public decimal Price { get; set; }
        public long Volume { get; set; }
    }
}
