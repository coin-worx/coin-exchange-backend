using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// BBORepresentation
    /// </summary>
    public class BBORepresentation
    {
        private string _currencyPair = string.Empty;
        private decimal _bestBidPrice = 0;
        private decimal _bestBidVolume = 0;
        private int _bestBidOrderCount = 0;
        private decimal _bestAskPrice = 0;
        private decimal _bestAskVolume = 0;
        private int _bestAskOrderCount = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBORepresentation(string currencyPair, decimal bestBidPrice, decimal bestBidVolume, int bestBidOrderCount,
            decimal bestAskPrice, decimal bestAskVolume, int bestAskOrderCount)
        {
            _currencyPair = currencyPair;
            _bestBidPrice = bestBidPrice;
            _bestBidVolume = bestBidVolume;
            _bestBidOrderCount = bestBidOrderCount;
            _bestAskPrice = bestAskPrice;
            _bestAskVolume = bestAskVolume;
            _bestAskOrderCount = bestAskOrderCount;
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Best Bid Price
        /// </summary>
        public decimal BestBidPrice
        {
            get { return _bestBidPrice; }
        }

        /// <summary>
        /// Best Bid Volume
        /// </summary>
        public decimal BestBidVolume
        {
            get { return _bestBidVolume; }
        }

        /// <summary>
        /// Best Bid's Ordercount
        /// </summary>
        public decimal BestBidOrderCount
        {
            get { return _bestBidOrderCount; }
        }

        /// <summary>
        /// best Ask's Price
        /// </summary>
        public decimal BestAskPrice
        {
            get { return _bestAskPrice; }
        }

        /// <summary>
        /// Best Ask's Volume
        /// </summary>
        public decimal BestAskVolume
        {
            get { return _bestAskVolume; }
        }

        /// <summary>
        /// Best Ask's OrderCount
        /// </summary>
        public decimal BestAskOrderCount
        {
            get { return _bestAskOrderCount; }
        }
    }
}
