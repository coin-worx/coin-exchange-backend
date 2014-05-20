using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Trades.Application.MarketDataServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.ReadModel.MemoryImages;
using CoinExchange.Trades.ReadModel.Repositories;
using CoinExchange.Trades.ReadModel.Services;

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
        private IOhlcRepository _ohlcRepository;
        private TickerInfoService _tickerInfoService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketDataQueryService(OrderBookMemoryImage orderBookMemoryImage, DepthMemoryImage depthMemoryImage,
            BBOMemoryImage bboMemoryImage,IOhlcRepository ohlcRepository,TickerInfoService tickerInfoService)
        {
            _orderBookMemoryImage = orderBookMemoryImage;
            _depthMemoryImage = depthMemoryImage;
            _bboMemoryImage = bboMemoryImage;
            _ohlcRepository = ohlcRepository;
            _tickerInfoService = tickerInfoService;
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
        /// Gets the Rate(midpoint of the best bid and the best ask) for the given currency Pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public Rate GetRate(string currencyPair)
        {
            foreach (Rate rate in _bboMemoryImage.RatesList)
            {
                if (rate.CurrencyPair.Equals(currencyPair, StringComparison.InvariantCultureIgnoreCase))
                {
                    return rate;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the Rates for every CurrencyPair maintaining an OrderBook
        /// </summary>
        public RatesList GetAllRates()
        {
            return _bboMemoryImage.RatesList;
        }

        /// <summary>
        /// Retreives the LimitOrderBook for bids specified by the Currency pair
        /// </summary>
        /// <param name="currencyPair"></param>
        private OrderRepresentationList GetBidBook(string currencyPair)
        {
            foreach (OrderRepresentationList bidBook in _orderBookMemoryImage.BidBooks)
            {
                if (bidBook.CurrencyPair.Equals(currencyPair, StringComparison.InvariantCultureIgnoreCase))
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
                if (askBook.CurrencyPair.Equals(currencyPair, StringComparison.InvariantCultureIgnoreCase))
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
        private DepthTuple[] GetBidDepth(string currencyPair)
        {
            // Traverse within each key value pair and find the currency pair stored
            foreach (KeyValuePair<string, DepthLevelRepresentationList> keyValuePair in _depthMemoryImage.BidDepths)
            {
                if (keyValuePair.Key.Equals(currencyPair, StringComparison.InvariantCultureIgnoreCase))
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
        private DepthTuple[] GetAskDepth(string currencyPair)
        {
            // Traverse within each key value pair and find the currency pair stored
            foreach (KeyValuePair<string, DepthLevelRepresentationList> keyValuePair in _depthMemoryImage.AskDepths)
            {
                if (keyValuePair.Key.Equals(currencyPair, StringComparison.InvariantCultureIgnoreCase))
                {
                    DepthLevelRepresentationList depthLevels = keyValuePair.Value;
                    return depthLevels.DepthLevels;
                }
            }
            return null;
        }

        #region Implementation of IMarketDataApplicationService

        public object GetTickerInfo(string pairs)
        {
            return _tickerInfoService.GetTickerInfo(pairs);
        }

        public OhlcRepresentation GetOhlcInfo(string currencyPair, int interval, string since)
        {
            IList<object> ohlcValues=_ohlcRepository.GetOhlcByCurrencyPair(currencyPair);
            return new OhlcRepresentation(ohlcValues,0,currencyPair);
        }

        /// <summary>
        /// Returns "Bid OrderBook : Ask OrderBook" where each element contains a tuple of Value:
        /// Item1 = Volume, Item2 = Price
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public object GetOrderBook(string currencyPair, int count)
        {
            OrderRepresentationList originalBidBook = this.GetBidBook(currencyPair);
            OrderRepresentationList originalAskBook = this.GetAskBook(currencyPair);

            if (count > 0)
            {
                OrderRepresentationList bidBook = new OrderRepresentationList(currencyPair, OrderSide.Buy);
                OrderRepresentationList askBook = new OrderRepresentationList(currencyPair, OrderSide.Sell);
                for (int i = 0; i < count; i++)
                {
                    if (i < originalBidBook.Count())
                    {
                        bidBook.AddRecord(originalBidBook.ToList()[i].Item1, originalBidBook.ToList()[i].Item2);
                    }
                    if (i < originalAskBook.Count())
                    {
                        askBook.AddRecord(originalAskBook.ToList()[i].Item1, originalAskBook.ToList()[i].Item2);
                    }
                }
                return new OrderBookRepresentation(bidBook,askBook);
            }
            else
            {
                return new OrderBookRepresentation(originalBidBook, originalAskBook);
            }
        }

        /// <summary>
        /// Returns the Depth as a Tuple where 
        /// Item1 = BidDepth,
        /// Item2 = AskDepth
        /// Each is an array of a Tuple of <decimal, decimal, int>, representing Volume, Price and OrderCount respectively
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public object GetDepth(string currencyPair)
        {
            //Tuple<decimal, decimal, int>[] bidDepth = GetBidDepth(currencyPair);
            //Tuple<decimal, decimal, int>[] askDepth = GetAskDepth(currencyPair);

            return new DepthTupleRepresentation(GetBidDepth(currencyPair), GetAskDepth(currencyPair));
        }

        #endregion
    }
}
