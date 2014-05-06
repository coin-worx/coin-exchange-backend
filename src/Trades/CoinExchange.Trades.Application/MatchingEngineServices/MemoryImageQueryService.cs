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
        private BBOMemoryImage _bboMemoryImage;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MemoryImageQueryService(BBOMemoryImage bboMemoryImage)
        {
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
    }
}
