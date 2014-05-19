using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
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
        private RatesList _ratesList = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBOMemoryImage()
        {
            _currencyPairs = new List<string>();
            InitializeCurrencyPairs();
            _bboRepresentationList = new BBORepresentationList();
            _ratesList = new RatesList();
            BBOEvent.BBOChanged += OnBBOArrived;
        }

        /// <summary>
        /// Event listener for listening BBO
        /// </summary>
        /// <param name="bbo"></param>
        private void OnBBOArrived(BBO bbo)
        {
            OnBBOArrived(bbo.CurrencyPair, bbo.BestBid, bbo.BestAsk);
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
            _ratesList.AddRate(currencyPair, bestBid.Price.Value, bestAsk.Price.Value);
        }

        /// <summary>
        /// Contains a BBO representation against each currency, each representation contains Volume, Price and order count 
        /// information for the best dask and bid depth of that currency
        /// </summary>
        public BBORepresentationList BBORepresentationList
        {
            get { return _bboRepresentationList; }
        }

        /// <summary>
        /// Contains the Rates for each and every currency pair. Rate is the midpoint between the best bid and the best ask
        /// </summary>
        public RatesList RatesList
        {
            get { return _ratesList; }
        }
    }
}
