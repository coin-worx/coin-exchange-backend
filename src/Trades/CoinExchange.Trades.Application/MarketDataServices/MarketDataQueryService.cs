using System;
using System.Collections.Generic;
using CoinExchange.Trades.Application.MarketDataServices.Representation;
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.Application.MarketDataServices
{
    /// <summary>
    /// Gets the data from te MemoryImages upon request from the user
    /// </summary>
    public class MarketDataQueryService : IMarketDataQueryService
    {
        private OrderBookMemoryImage _orderBookMemoryImage = null;
        private DepthMemoryImage _depthMemoryImage = null;
        private BBOMemoryImage _bboMemoryImage = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketDataQueryService(OrderBookMemoryImage orderBookMemoryImage, DepthMemoryImage depthMemoryImage,
            BBOMemoryImage bboMemoryImage)
        {
            _orderBookMemoryImage = orderBookMemoryImage;
            _depthMemoryImage = depthMemoryImage;
            _bboMemoryImage = bboMemoryImage;
        }

        /// <summary>
        /// Retreives the BBO from the BBO memory image
        /// </summary>
        /// <returns></returns>
        public BBORepresentation GetBBO(string currencyPair)
        {
            foreach (BBORepresentation bboRepresentation in _bboMemoryImage.BBORepresentationList)
            {
                if (bboRepresentation.CurrencyPair == currencyPair)
                {
                    return bboRepresentation;
                }
            }
            return null;
        }

        /// <summary>
        /// Retreives the LimitOrderBook for bids specified by the Currency pair
        /// </summary>
        /// <param name="currencyPair"></param>
        private OrderRepresentationList GetBidBook(string currencyPair)
        {
            foreach (OrderRepresentationList bidBook in _orderBookMemoryImage.BidBooks)
            {
                if (bidBook.CurrencyPair == currencyPair)
                {
                    return bidBook;
                }
            }
            return null;
        }

        /// <summary>
        /// Retreives the LimitOrderBook for asks specified by the Currency pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        private OrderRepresentationList GetAskBook(string currencyPair)
        {
            foreach (OrderRepresentationList askBook in _orderBookMemoryImage.AskBooks)
            {
                if (askBook.CurrencyPair == currencyPair)
                {
                    return askBook;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the bid depth for the specified Currency Pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        private Tuple<decimal, decimal, int>[] GetBidDepth(string currencyPair)
        {
            // Traverse within each key value pair and find the currency pair stored
            foreach (KeyValuePair<string, DepthLevelRepresentationList> keyValuePair in _depthMemoryImage.BidDepths)
            {
                if (keyValuePair.Key == currencyPair)
                {
                    DepthLevelRepresentationList depthLevels = keyValuePair.Value;
                    return depthLevels.DepthLevels;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the ask depth for the specified Currency Pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        private Tuple<decimal, decimal, int>[] GetAskDepth(string currencyPair)
        {
            // Traverse within each key value pair and find the currency pair stored
            foreach (KeyValuePair<string, DepthLevelRepresentationList> keyValuePair in _depthMemoryImage.AskDepths)
            {
                if (keyValuePair.Key == currencyPair)
                {
                    DepthLevelRepresentationList depthLevels = keyValuePair.Value;
                    return depthLevels.DepthLevels;
                }
            }
            return null;
        }

        #region Implementation of IMarketDataApplicationService

        public TickerRepresentation[] GetTickerInfo(string pairs)
        {
            throw new NotImplementedException();
        }

        public OhlcRepresentation GetOhlcInfo(string pair, int interval, string since)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns "Bid OrderBook : Ask OrderBook" where each element contains a tuple of Value:
        /// Item1 = Volume, Item2 = Price
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Tuple<OrderRepresentationList, OrderRepresentationList> GetOrderBook(string currencyPair, int count)
        {
            OrderRepresentationList bidBook = this.GetBidBook(currencyPair);
            OrderRepresentationList askBook = this.GetAskBook(currencyPair);
            return new Tuple<OrderRepresentationList, OrderRepresentationList>(bidBook, askBook);
        }

        /// <summary>
        /// Returns the Depth as a Tuple where 
        /// Item1 = BidDepth,
        /// Item2 = AskDepth
        /// Each is an array of a Tuple of <decimal, decimal, int>, representing Volume, Price and OrderCount respectively
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> GetDepth(string currencyPair)
        {
            Tuple<decimal, decimal, int>[] bidDepth = GetBidDepth(currencyPair);
            Tuple<decimal, decimal, int>[] askDepth = GetAskDepth(currencyPair);

            return new Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]>(bidDepth, askDepth);
        }

        #endregion
    }
}
