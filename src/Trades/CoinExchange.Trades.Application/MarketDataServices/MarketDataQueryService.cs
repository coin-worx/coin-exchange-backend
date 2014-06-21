using System;
using System.Collections.Generic;
using System.Globalization;
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
                        bidBook.AddRecord(originalBidBook.ToList()[i].Volume, originalBidBook.ToList()[i].Price, originalBidBook.ToList()[i].DateTime);
                    }
                    if (i < originalAskBook.Count())
                    {
                        askBook.AddRecord(originalAskBook.ToList()[i].Volume, originalAskBook.ToList()[i].Price, originalAskBook.ToList()[i].DateTime);
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

        /// <summary>
        /// Get spread of bid and ask
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public object GetSpread(string currencyPair)
        {
            List<OrderRecord> bidsRecord;
            List<OrderRecord> asksRecord;
            OrderRepresentationList originalBidBook = this.GetBidBook(currencyPair);
            OrderRepresentationList originalAskBook = this.GetAskBook(currencyPair);
            asksRecord = originalAskBook.OrderBy(x => x.DateTime).ToList();
            bidsRecord = originalBidBook.OrderBy(x => x.DateTime).ToList();
            List<Spread> spread;
            OrderRecord lastBid;
            OrderRecord lastAsk;
            if (asksRecord.Count > 0 && bidsRecord.Count > 0)
            {
                spread=new List<Spread>();
                if (asksRecord[0].DateTime >= bidsRecord[0].DateTime)
                {
                    spread.Add(new Spread(asksRecord[0].Price,bidsRecord[0].Price,asksRecord[0].DateTime));
                }
                if (asksRecord[0].DateTime < bidsRecord[0].DateTime)
                {
                    spread.Add(new Spread(asksRecord[0].Price, bidsRecord[0].Price, bidsRecord[0].DateTime));
                }
                lastAsk = asksRecord[0];
                lastBid = bidsRecord[0];
                
                if (bidsRecord.Count >= asksRecord.Count)
                {
                    //traverse according to bids
                    for (int i = 1; i < bidsRecord.Count; i++)
                    {
                        for (int j = 1; j < asksRecord.Count; j++)
                        {
                            if (bidsRecord[i].DateTime <= asksRecord[j].DateTime)
                            {
                               //lastAsk = asksRecord[j];
                               spread.Add(new Spread(lastAsk.Price, bidsRecord[i].Price, bidsRecord[i].DateTime));
                            }
                            else
                            {
                                lastAsk = asksRecord[j];
                                spread.Add(new Spread(lastAsk.Price, bidsRecord[i].Price, lastAsk.DateTime));
                            }
                        }
                        //spread.Add(new Spread(lastAsk.Price,bidsRecord[i].Price,bidsRecord[i].DateTime));
                    }
                }
                else
                {
                    //traverse according to asks
                    for (int i = 1; i < asksRecord.Count; i++)
                    {
                        for (int j = 1; j < bidsRecord.Count; j++)
                        {
                            if (asksRecord[i].DateTime <= bidsRecord[j].DateTime)
                            {
                                lastBid = bidsRecord[j];
                            }
                        }
                        spread.Add(new Spread(asksRecord[i].Price, lastBid.Price, asksRecord[i].DateTime));
                    }
                }
                
                return spread;
            }
           return null;
        }
    }
}
