using System.Collections.Generic;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Application.TradeServices.Representation
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
