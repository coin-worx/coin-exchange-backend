using System.Collections.Generic;
using CoinExchange.Trades.Domain.Model.MarketDataAggregate;

namespace CoinExchange.Trades.Application.MarketDataServices.Representation
{
    /// <summary>
    /// serves as representation for ticker info query
    /// </summary>
    public class OhlcRepresentation
    {
        //public List<Ohlc> Ohlc { get; set; }
        public string PairName { get; set; }
        public long Last { get; set; }
        public IList<object> OhlcInfo { get; set; }

        public OhlcRepresentation(List<Ohlc> ohlc, string pairName, long last)
        {
            //Ohlc = ohlc;
            PairName = pairName;
            Last = last;
        }

        public OhlcRepresentation(IList<object> ohlcInfo, long last, string pairName)
        {
            OhlcInfo = ohlcInfo;
            Last = last;
            PairName = pairName;
        }

    }
}
