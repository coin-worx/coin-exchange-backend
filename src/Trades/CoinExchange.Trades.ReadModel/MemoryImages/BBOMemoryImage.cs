using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.DomainEvents;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing best Bid and best Ask
    /// </summary>
    public class BBOMemoryImage
    {
        private List<string> _currencyPairs = new List<string>(); 

        private BBORepresentationList _bboRepresentationList = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBOMemoryImage()
        {
            _currencyPairs = new List<string>();
            InitializeCurrencyPairs();
            _bboRepresentationList = new BBORepresentationList();
            BBOEvent.BBOChanged += OnBBOArrived;
        }

        /// <summary>
        /// Event listener for listening BBO
        /// </summary>
        /// <param name="bbo"></param>
        private void OnBBOArrived(BBO bbo)
        {
            OnBBOArrived("", bbo.BestBid, bbo.BestAsk);

        }

        /// <summary>
        /// Initialize the set of currency pairs that the application is to support.
        /// </summary>
        private void InitializeCurrencyPairs()
        {
            _currencyPairs.Add(CurrencyConstants.BitCoinUsd);
        }

        /// <summary>
        /// Handles the Best bid and best offer
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="bestBid"></param>
        /// <param name="bestAsk"></param>
        public void OnBBOArrived(string currencyPair, DepthLevel bestBid, DepthLevel bestAsk)
        {
            _bboRepresentationList.AddBBO(currencyPair, bestBid, bestAsk);
        }

        /// <summary>
        /// BBORepresentationList
        /// </summary>
        public BBORepresentationList BBORepresentationList
        {
            get { return _bboRepresentationList; }
        }
    }
}
