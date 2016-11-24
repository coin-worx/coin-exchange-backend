/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Services;
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
        [Category("Unit")]
        public void AddSellOrdersAndSortTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 491.34M, new StubbedOrderIdGenerator());

            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.AskCount, "Count of Sell Orders");
            Assert.AreEqual(491.34M, limitOrderBook.Asks.First().Price.Value, "First element of Sell Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Asks.Last().Price.Value, "Last element of Sell Orders list");
        }

        /// <summary>
        /// Tests if sell orders abundant quantity can fill multiple buy orders
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceSellOrderAbundantTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 150, 491.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 491.34M, new StubbedOrderIdGenerator());
            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M), 
                OrderSide.Sell, OrderType.Limit, new Volume(250), new TraderId("1")));
            
            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order is of greater quantity than the buy order on that price level
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceSellOrderGreaterQtyTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 150, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            
            Assert.AreEqual(1, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M), OrderSide.Sell,
                OrderType.Limit, new Volume(200), new TraderId("1")));

            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(1, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");
            Assert.AreEqual(50, limitOrderBook.Asks.First().OpenQuantity.Value, "Volume of the Buy Order after updating");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order that has lesser price than the buy order matches the and trade is executed
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceSellOrderPriceLessThanEqualTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 150, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);

            Assert.AreEqual(1, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(489.34M), 
                OrderSide.Sell, OrderType.Limit, new Volume(150), new TraderId("1")));

            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if sell order is of limited quantity than the buy orders combined
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceSellOrderLimitedTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 150, 491.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 491.34M, new StubbedOrderIdGenerator());
            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M), 
                OrderSide.Sell, OrderType.Limit, new Volume(200), new TraderId("1")));

            Assert.AreEqual(1, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        #endregion Sell Order Tests

        #region Buy Order Tests

        /// <summary>
        /// Tests if there are no orders on the sell side, will the Orderbook add buy orders properly and also sort them
        /// in descending order
        /// </summary>
        [Test]
        [Category("Unit")]
        public void AddBuyOrdersAndSortTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 491.34M, new StubbedOrderIdGenerator());
            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.BidCount, "Count of Buy Orders");
            Assert.AreEqual(493.34M, limitOrderBook.Bids.First().Price.Value, "First element of Buy Orders list");
            Assert.AreEqual(491.34M, limitOrderBook.Bids.Last().Price.Value, "Last element of Buy Orders list");
        }

        /// <summary>
        /// Tests if buy orders abundant quantity can fill multiple sell orders
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceBuyOrderAbundantTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 150, 491.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 491.34M, new StubbedOrderIdGenerator());
            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M),
                OrderSide.Buy, OrderType.Limit, new Volume(250), new TraderId("1")));

            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Sell Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Buy Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order is of greater quantity than the sell order on that price level
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceBuyOrderGreaterQuantityTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 150, 491.34M, new StubbedOrderIdGenerator());
            limitOrderBook.PlaceOrder(order1);

            Assert.AreEqual(1, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M),
                OrderSide.Buy, OrderType.Limit, new Volume(200), new TraderId("1")));

            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(1, limitOrderBook.BidCount, "Count of Sell Orders after trade execution");
            Assert.AreEqual(50, limitOrderBook.Bids.First().OpenQuantity.Value, "Volume of the Buy Order after updating");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order that has greater price than the sell order in Ask Order book matches and trade is executed
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceBuyOrderPriceGreaterThanEqualTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 150, 480.34M, new StubbedOrderIdGenerator());
            limitOrderBook.PlaceOrder(order1);

            Assert.AreEqual(1, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(490.34M),
                OrderSide.Buy, OrderType.Limit, new Volume(150), new TraderId("1")));

            Assert.AreEqual(0, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        /// <summary>
        /// Tests if buy order is of limited quantity than the sell orders on the order book combined
        /// </summary>
        [Test]
        [Category("Unit")]
        public void PlaceBuyOrderLimitedTest()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 150, 491.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 491.34M, new StubbedOrderIdGenerator());
            Order order3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 491.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);

            Assert.AreEqual(3, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");

            bool placeOrder = limitOrderBook.PlaceOrder(new Order(new OrderId("1"), "XBTUSD", new Price(491.34M), 
                OrderSide.Buy, OrderType.Limit, new Volume(200), new TraderId("1")));

            Assert.AreEqual(1, limitOrderBook.AskCount, "Count of Buy Orders after trade execution");
            Assert.AreEqual(0, limitOrderBook.BidCount, "Count of Sell Orders after trade execution");
            Assert.IsTrue(placeOrder);
        }

        #endregion Buy Order Tests
    }
}
