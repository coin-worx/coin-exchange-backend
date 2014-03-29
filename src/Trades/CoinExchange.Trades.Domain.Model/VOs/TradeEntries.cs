using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// VO to hold recent trade entries
    /// </summary>
    public class TradeEntries
    {
        public TradeEntries(decimal price, decimal volume, string side, string type, string miscellaneous, string dateTime)
        {
            Price = price;
            this.volume = volume;
            Side = side;
            Type = type;
            Miscellaneous = miscellaneous;
            DateTime = dateTime;
        }

        public decimal Price { get; private set; }
        public decimal volume { get; private set; }
        public string DateTime { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public string Miscellaneous { get; private set; }

    }
}
