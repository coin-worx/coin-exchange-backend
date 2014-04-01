using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.MarketData
{
    /// <summary>
    /// Contains ohlc data
    /// </summary>
    public class Ohlc
    {
        public Ohlc(DateTime time, decimal open, decimal low, decimal high, decimal close, decimal vwap, decimal volume, int count)
        {
            Time = time;
            Open = open;
            Low = low;
            High = high;
            Close = close;
            this.vwap = vwap;
            this.volume = volume;
            this.count = count;
        }

        public DateTime Time { get; private set; }
        public decimal Open { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Close { get; private set; }
        public decimal vwap { get; private set; }
        public decimal volume { get; private set; }
        public int count { get; private set; }
    }
}
