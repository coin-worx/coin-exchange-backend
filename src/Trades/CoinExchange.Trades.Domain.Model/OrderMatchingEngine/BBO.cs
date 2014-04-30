using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// VO for transmitting two depth levels(bids and asks)
    /// </summary>
    [Serializable]
    public class BBO
    {
        private string _currencyPair;
        private DepthLevel _bestBid;
        private DepthLevel _bestAsk;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="currencypair"></param>
        /// <param name="bestBid"></param>
        /// <param name="bestAsk"></param>
        public BBO(string currencypair, DepthLevel bestBid, DepthLevel bestAsk)
        {
            _currencyPair = currencypair;
            _bestBid = bestBid;
            _bestAsk = bestAsk;
        }

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
            private set { _currencyPair = value; }
        }

        /// <summary>
        /// BestBid
        /// </summary>
        public DepthLevel BestBid
        {
            get { return _bestBid; }
            private set { _bestBid = value; }
        }

        /// <summary>
        /// BestAsk
        /// </summary>
        public DepthLevel BestAsk
        {
            get { return _bestAsk; }
            private set { _bestAsk = value; }
        }
    }
}
