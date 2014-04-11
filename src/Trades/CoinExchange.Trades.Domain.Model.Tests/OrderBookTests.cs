using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.MatchingEngine;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    /// <summary>
    /// OrderBook Tests
    /// </summary>
    class OrderBookTests
    {
        public void PlaceSellOrderTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.Bids.Add(491.55M, new Order.Order(){Price = 491, });
        }
    }
}
