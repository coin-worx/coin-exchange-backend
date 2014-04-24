using CoinExchange.Trades.Domain.Model.MarketDataAggregate;

namespace CoinExchange.Trades.Application.MarketDataServices.Representation
{
    /// <summary>
    /// Represenstation for ticker for query ticker info
    /// </summary>
    public class TickerRepresentation
    {
        public TickerRepresentation(string pairName, MarketData ask, MarketData bid, MarketData last, decimal[] volume, decimal[] weightedVolumeAverage, int[] numberOfTrades, decimal[] low, decimal[] high, decimal openingPrice)
        {
            PairName = pairName;
            Ask = ask;
            Bid = bid;
            Last = last;
            Volume = volume;
            WeightedVolumeAverage = weightedVolumeAverage;
            NumberOfTrades = numberOfTrades;
            Low = low;
            High = high;
            OpeningPrice = openingPrice;
        }

        public string PairName { get; private set; }
        public MarketData Ask { get; private set; }
        public MarketData Bid { get; private set; }
        public MarketData Last { get; private set; }
        public decimal[] Volume { get; private set; }
        public decimal[] WeightedVolumeAverage { get; private set; }
        public int[] NumberOfTrades { get; private set; }
        public decimal[] Low { get; private set; }
        public decimal[] High { get; private set; }
        public decimal OpeningPrice { get;private set; }
    }
}
