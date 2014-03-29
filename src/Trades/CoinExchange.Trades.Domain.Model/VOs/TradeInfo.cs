using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// VO for response of recent trades call
    /// </summary>
    public class TradeInfo
    {
        public string PairName { get;private set; }
        public List<TradeEntries> Entries { get;private set; }
        public string Last { get; private set; }

        public TradeInfo(string last, List<TradeEntries> entries, string pairName)
        {
            Last = last;
            Entries = entries;
            PairName = pairName;
        }
    }
}
