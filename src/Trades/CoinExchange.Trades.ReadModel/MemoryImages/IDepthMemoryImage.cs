using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Interface to have an n-memeory image to store depth for all the currency pairs
    /// </summary>
    public interface IDepthMemoryImage : IMemoryImage
    {
        /// <summary>
        /// Handles the events in the case of changed depth for a currencyPair
        /// </summary>
        /// <param name="depth"></param>
        void OnDepthArrived(Depth depth);
    }
}
