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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    public class OrderFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }
        
        /// <summary>
        /// To verify buy side market order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateBuySideMarketOrderTest()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Market);
            Assert.AreEqual(order.OrderSide, OrderSide.Buy);
            Assert.AreEqual(order.Volume.Value, 5);
        }

        /// <summary>
        /// To verify sell side market order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateSellSideMarketOrderTest()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Market);
            Assert.AreEqual(order.OrderSide, OrderSide.Sell);
            Assert.AreEqual(order.Volume.Value, 5);
        }

        /// <summary>
        /// To verify buy side limit order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateBuySideLimitOrderTest()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Limit);
            Assert.AreEqual(order.OrderSide, OrderSide.Buy);
            Assert.AreEqual(order.Volume.Value, 5);
            Assert.AreEqual(order.Price.Value, 10);
        }

        /// <summary>
        /// To verify sell side limit order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateSellSideLimitOrderTest()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 5, 10,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Limit);
            Assert.AreEqual(order.OrderSide, OrderSide.Sell);
            Assert.AreEqual(order.Volume.Value, 5);
            Assert.AreEqual(order.Price.Value, 10);
        }

        /// <summary>
        /// To verify sell side limit order created
        /// </summary>
        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateLimitOrder_IfPriceIs0_InvalidOperationExceptionWillBeThrown()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 5, 0,
                new StubbedOrderIdGenerator());
        }

        /// <summary>
        /// To verify order volume is greater than 0
        /// </summary>
        [Test]
        [Category("Unit")]
        public void InvalidOrderVolumeTest()
        {
            bool orderVolumeException = false;
            decimal volume = 0;
            try
            {
                Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", volume, 10,
                    new StubbedOrderIdGenerator());
            }
            catch (InvalidOperationException exception)
            {
                orderVolumeException = true;
            }
            Assert.AreEqual(orderVolumeException,true);
        }

    }
}
