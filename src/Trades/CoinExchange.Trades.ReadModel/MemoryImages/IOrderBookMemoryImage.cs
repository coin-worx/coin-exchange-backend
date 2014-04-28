using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using Disruptor;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Interface for in-memory OrderBook image
    /// </summary>
    public interface IOrderBookMemoryImage : IMemoryImage
    {
        /// <summary>
        /// handles the processing in case the LimitOrderBook changes and is fired as an event
        /// </summary>
        /// <param name="orderBook"></param>
        void OnOrderBookChanged(LimitOrderBook orderBook);
    }
}
