/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.MatchingEngine;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    /// <summary>
    /// OrderBook Tests
    /// </summary>
    [TestFixture]
    class OrderBookTests
    {
        #region Sell Orders Tests

        /// <summary>
        /// Tests if there are no orders on the buy side, will the Orderbook add Sell orders properly and also sort them
        /// in ascending order
        /// </summary>
        [Test]
        public void AddSellOrdersAndSortTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 493.34M, OrderSide.Sell, OrderType.Limit, 250,
                                                      new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 250,
                                                      new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 492.34M, OrderSide.Sell, OrderType.Limit, 250,
                                                      new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Asks.Count, "Count of Sell Orders");
            Assert.AreEqual(491.34M, limitOrderBook.Asks.First().Price, "First element of Sell Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Asks.Last().Price, "Last element of Sell Orders list");
        }

        /// <summary>
        /// Tests if sell orders abundant quantity can fill multiple buy orders
        /// </summary>
        [Test]
        public void PlaceSellOrderAbundantTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 150, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 50, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 50, new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 250, new TraderId(1)));
            
            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order is of greater quantity than the buy order on that price level
        /// </summary>
        [Test]
        public void PlaceSellOrderGreaterQtyTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 150, new TraderId(1)));
            
            Assert.AreEqual(1, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 200, new TraderId(1)));

            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(1, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");
            Assert.AreEqual(50, limitOrderBook.Asks.First().Volume, "Volume of the Buy Order after updating");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order that has lesser price than the buy order matches the and trade is executed
        /// </summary>
        [Test]
        public void PlaceSellOrderPriceLessThanEqualTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 150, new TraderId(1)));

            Assert.AreEqual(1, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 489.34M, OrderSide.Sell, OrderType.Limit, 150, new TraderId(1)));

            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order is of limited quantity than the buy orders combined
        /// </summary>
        [Test]
        public void PlaceSellOrderLimitedTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 150, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 50, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 50, new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 200, new TraderId(1)));

            Assert.AreEqual(1, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        #endregion Sell Order Tests

        #region Buy Order Tests

        /// <summary>
        /// Tests if there are no orders on the sell side, will the Orderbook add buy orders properly and also sort them
        /// in descending order
        /// </summary>
        [Test]
        public void AddBuyOrdersAndSortTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 493.34M, OrderSide.Buy, OrderType.Limit, 250,
                                                      new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 250,
                                                      new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 492.34M, OrderSide.Buy, OrderType.Limit, 250,
                                                      new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Bids.Count, "Count of Buy Orders");
            Assert.AreEqual(493.34M, limitOrderBook.Bids.First().Price, "First element of Buy Orders list");
            Assert.AreEqual(491.34M, limitOrderBook.Bids.Last().Price, "Last element of Buy Orders list");
        }

        /// <summary>
        /// Tests if buy orders abundant quantity can fill multiple sell orders
        /// </summary>
        [Test]
        public void PlaceBuyOrderAbundantTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 150, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 50, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 50, new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 250, new TraderId(1)));

            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Sell Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Buy Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order is of greater quantity than the sell order on that price level
        /// </summary>
        [Test]
        public void PlaceBuyOrderGreaterQuantityTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 150, new TraderId(1)));

            Assert.AreEqual(1, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 200, new TraderId(1)));

            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(1, limitOrderBook.Bids.Count, "Count of Sell Orders after trade execution");
            Assert.AreEqual(50, limitOrderBook.Bids.First().Volume, "Volume of the Buy Order after updating");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order that has greater price than the sell order in Ask Order book matches and trade is executed
        /// </summary>
        [Test]
        public void PlaceBuyOrderPriceGreaterThanEqualTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 480.34M, OrderSide.Sell, OrderType.Limit, 150, new TraderId(1)));

            Assert.AreEqual(1, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 490.34M, OrderSide.Buy, OrderType.Limit, 150, new TraderId(1)));

            Assert.AreEqual(0, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order is of limited quantity than the sell orders on the order book combined
        /// </summary>
        [Test]
        public void PlaceBuyOrderLimitedTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 150, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 50, new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Sell, OrderType.Limit, 50, new TraderId(1)));

            Assert.AreEqual(3, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order.Order("XBTUSD", 491.34M, OrderSide.Buy, OrderType.Limit, 200, new TraderId(1)));

            Assert.AreEqual(1, limitOrderBook.Asks.Count, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.Bids.Count, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        #endregion Buy Order Tests
    }
}
*/