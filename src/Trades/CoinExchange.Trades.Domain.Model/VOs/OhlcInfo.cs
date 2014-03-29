using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// Return ohlc info to user
    /// </summary>
    public class OhlcInfo
    {
        public List<OhlcValues> Ohlc { get; private set; }
        public string PairName { get; private set; }
        public long Last { get; private set; }

        public OhlcInfo(List<OhlcValues> ohlc, string pairName, long last)
        {
            Ohlc = ohlc;
            PairName = pairName;
            Last = last;
        }
    }
}
