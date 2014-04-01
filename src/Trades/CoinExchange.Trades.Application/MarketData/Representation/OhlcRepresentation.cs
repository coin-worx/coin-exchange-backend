using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.MarketData;

namespace CoinExchange.Trades.Application.MarketData.Representation
{
    /// <summary>
    /// serves as representation for ticker info query
    /// </summary>
    public class OhlcRepresentation
    {
        public List<Ohlc> Ohlc { get; set; }
        public string PairName { get; set; }
        public long Last { get; set; }

        public OhlcRepresentation(List<Ohlc> ohlc, string pairName, long last)
        {
            Ohlc = ohlc;
            PairName = pairName;
            Last = last;
        }
    }
}
