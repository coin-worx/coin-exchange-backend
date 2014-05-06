using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.Application.MatchingEngineServices
{
    /// <summary>
    /// Gets the data from te MemoryImages upon request from the user
    /// </summary>
    public class MemoryImageQueryService
    {
        private OrderBookMemoryImage _orderBookMemoryImage = null;
        private BBOMemoryImage _bboMemoryImage = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MemoryImageQueryService(OrderBookMemoryImage orderBookMemoryImage, BBOMemoryImage bboMemoryImage)
        {
            _orderBookMemoryImage = orderBookMemoryImage;
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
        public OrderRepresentationList GetBidBook(string currencyPair)
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
        public OrderRepresentationList GetAskBook(string currencyPair)
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
    }
}
