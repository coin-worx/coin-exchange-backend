using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing depth levels
    /// </summary>
    public class DepthMemoryImage
    {
        private List<string> _currencyPairs = new List<string>(); 

        private DepthRepresentation _bidDepth = new DepthRepresentation();
        private DepthRepresentation _askDepth = new DepthRepresentation();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DepthMemoryImage()
        {
            _currencyPairs = new List<string>();
            InitializeCurrencyPairs();
            DepthEvent.DepthChanged += OnDepthArrived;
        }

        /// <summary>
        /// Initialize the set of currency pairs that the application is to support.
        /// </summary>
        private void InitializeCurrencyPairs()
        {
            _currencyPairs.Add(CurrencyConstants.BitCoinUsd);
        }

        /// <summary>
        /// Handles the event for the Depth
        /// </summary>
        public void OnDepthArrived(Depth depth)
        {
            UpdateBids(depth.CurrencyPair, depth.BidLevels);
            UpdateAsks(depth.CurrencyPair, depth.AskLevels);
        }

        /// <summary>
        /// Update the Bid depth levels
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="bidLevels"></param>
        private void UpdateBids(string currencyPair, DepthLevel[] bidLevels)
        {
            var depthLevelsRepresentations = CreateDepthLevels(bidLevels);
            if (depthLevelsRepresentations != null)
            {
                if (_bidDepth.ContainsKey(currencyPair))
                {
                    _bidDepth.SetValue(currencyPair, CreateDepthLevels(bidLevels));
                }
                _bidDepth.AddDepth(currencyPair, CreateDepthLevels(bidLevels));
            }
        }

        /// <summary>
        /// Update teh Ask depth levels
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="askLevels"></param>
        private void UpdateAsks(string currencyPair, DepthLevel[] askLevels)
        {
            var depthLevelsRepresentations = CreateDepthLevels(askLevels);
            if (depthLevelsRepresentations != null)
            {
                if (_askDepth.ContainsKey(currencyPair))
                {
                    _askDepth.SetValue(currencyPair, CreateDepthLevels(askLevels));
                }
                _askDepth.AddDepth(currencyPair, CreateDepthLevels(askLevels));
            }
        }

        private DepthLevelRepresentationList CreateDepthLevels(DepthLevel[] depthLevels)
        {
            DepthLevelRepresentationList depthLevelsRepresentations = new DepthLevelRepresentationList(depthLevels.Length);
            for (int i = 0; i < depthLevels.Length; i++)
            {
                if (depthLevels[i].Price != null)
                {
                    depthLevelsRepresentations.AddDepthLevel(i, depthLevels[i]);
                }
            }
            if (depthLevelsRepresentations.Any())
            {
                return depthLevelsRepresentations;
            }
            return null;
        }

        /// <summary>
        /// Contains Depth levels for bids for each currency in the order book, and each depth level contains
        /// Item1 = Aggregated Volume
        /// Item2 = Price
        /// Item3 = Number of orders present
        /// </summary>
        public DepthRepresentation BidDepths
        {
            get { return _bidDepth; }
        }

        /// <summary>
        /// Contains Depth levels for asks for each currency in the order book, and each depth level contains:
        /// Item1 = Aggregated Volume
        /// Item2 = Price
        /// Item3 = Number of orders present
        /// </summary>
        public DepthRepresentation AskDepths
        {
            get { return _askDepth; }
        }
    }
}
