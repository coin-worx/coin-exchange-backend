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
        public List<OhlcData> Ohlc { get; set; }
        public string PairName { get; set; }
        public long Last { get; set; }
        
    }
}
