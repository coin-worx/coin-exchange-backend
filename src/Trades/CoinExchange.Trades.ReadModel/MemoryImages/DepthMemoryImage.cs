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

        /// <summary>
        /// Contains tuple to contain Volume, Price and Order Count for bid depth levels
        /// </summary>
        private DepthLevelRepresentationList _bidDepth = null;

        /// <summary>
        /// Contains tuple to contain Volume, Price and Order Count for ask depth levels
        /// </summary>
        private DepthLevelRepresentationList _askDepth = null;

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
            var depthlevelrepresentations = GetDepthDictionary(OrderSide.Buy);
            UpdateDepthlevels(currencyPair,OrderSide.Buy, bidLevels, depthlevelrepresentations);
        }

        /// <summary>
        /// Update teh Ask depth levels
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="askLevels"></param>
        private void UpdateAsks(string currencyPair, DepthLevel[] askLevels)
        {
            var depthLevelRepresentations = GetDepthDictionary(OrderSide.Sell);
            UpdateDepthlevels(currencyPair, OrderSide.Sell, askLevels, depthLevelRepresentations);
        }

        /// <summary>
        /// Update the depth levels in the dictionaries
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="orderSide"> </param>
        /// <param name="depthLevels"></param>
        /// <param name="depthLevelRepresentations"></param>
        private void UpdateDepthlevels(string currencyPair, OrderSide orderSide, DepthLevel[] depthLevels,
                                      DepthLevelRepresentationList depthLevelRepresentations)
        {
            for (int i = 0; i < depthLevels.Length; i++)
            {
                if (depthLevelRepresentations == null)
                {
                    if (_currencyPairs.Contains(currencyPair))
                    {
                        depthLevelRepresentations = new DepthLevelRepresentationList(currencyPair, orderSide,
                                                                                     depthLevels.Length);
                        if (orderSide == OrderSide.Buy)
                        {
                            _bidDepth = depthLevelRepresentations;
                        }
                        else
                        {
                            _askDepth = depthLevelRepresentations;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Currency pair not allowed to be traded in CoinExchange application.");
                    }
                }
                depthLevelRepresentations.AddDepthLevel(i, depthLevels[i]);
            }
        }

        /// <summary>
        /// Returns the corresponding depth dictionary as per the OrderSide
        /// </summary>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        private DepthLevelRepresentationList GetDepthDictionary(OrderSide orderSide)
        {
            switch (orderSide)
            {
                    case OrderSide.Buy:
                    return _bidDepth;

                    case OrderSide.Sell:
                    return _askDepth;
            }
            return null;
        }

        /// <summary>
        /// Bid Depth containing all bid depth levels
        /// </summary>
        public DepthLevelRepresentationList BidDepth
        {
            get { return _bidDepth; }
        }

        /// <summary>
        /// Ask Depth containing all ask depth levels
        /// </summary>
        public DepthLevelRepresentationList AskDepth
        {
            get { return _askDepth; }
        }
    }
}
