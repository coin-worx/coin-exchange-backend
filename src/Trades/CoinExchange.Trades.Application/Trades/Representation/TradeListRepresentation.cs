using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.Trades.Representation
{
    /// <summary>
    /// representation for retunring list of trades
    /// </summary>
    public class TradeListRepresentation
    {
        public string PairName { get;private set; }
        public List<TradeRecord> Entries { get; private set; }
        public string Last { get; private set; }

        public TradeListRepresentation(string last, List<TradeRecord> entries, string pairName)
        {
            Last = last;
            Entries = entries;
            PairName = pairName;
        }
    }
}
