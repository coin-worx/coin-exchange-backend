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
using CoinExchange.Trades.Application.MarketDataServices;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.MemoryImages;
using NUnit.Framework;

namespace CoinExchange.Trades.Application.Tests
{
    [TestFixture]
    class MarketDataQueryServiceTests
    {
        [Test]
        [Category("Unit")]
        public void GetBboTest_ChecksIfTheBboIsRetreivedSuccessfully_ReturnsBboForCurrencypairIfPresent()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();
            MarketDataQueryService queryService = new MarketDataQueryService(null, null, bboMemoryImage, null, null);

            DepthLevel bestBid = new DepthLevel(new Price(491));
            bestBid.IncreaseVolume(new Volume(100));
            DepthLevel bestAsk = new DepthLevel(new Price(492));
            bestAsk.IncreaseVolume(new Volume(200));
            bboMemoryImage.OnBBOArrived("BTCUSD", bestBid, bestAsk);

            BBORepresentation bboRepresentation = queryService.GetBBO("BTCUSD");

            Assert.IsNotNull(bboRepresentation);
            Assert.AreEqual(bestBid.Price.Value, bboRepresentation.BestBidPrice);
            Assert.AreEqual(bestBid.AggregatedVolume.Value, bboRepresentation.BestBidVolume);
            Assert.AreEqual(bestBid.OrderCount, bboRepresentation.BestBidOrderCount);
            Assert.AreEqual(bestAsk.Price.Value, bboRepresentation.BestAskPrice);
            Assert.AreEqual(bestAsk.AggregatedVolume.Value, bboRepresentation.BestAskVolume);
            Assert.AreEqual(bestAsk.OrderCount, bboRepresentation.BestAskOrderCount);
        }

        [Test]
        [Category("Unit")]
        public void GetBidOrderBookTest_ChecksIfOrderBookIsretreivedSuccessfully_VerifiesOrderBookToSeeItContainsValuesAsExpected()
        {
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(orderBookMemoryImage, null, null, null, null);
            LimitOrderBook limitOrderBook = new LimitOrderBook("BTCUSD");

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 946, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12347", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 940, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 947, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("127633", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 950, new StubbedOrderIdGenerator());

            limitOrderBook.AddOrder(buyOrder1);
            limitOrderBook.AddOrder(buyOrder2);
            limitOrderBook.AddOrder(buyOrder3);
            limitOrderBook.AddOrder(buyOrder4);
            limitOrderBook.AddOrder(sellOrder1);
            limitOrderBook.AddOrder(sellOrder2);
            limitOrderBook.AddOrder(sellOrder3);
            limitOrderBook.AddOrder(sellOrder4);

            Assert.AreEqual(4, limitOrderBook.Bids.Count());
            Assert.AreEqual(4, limitOrderBook.Asks.Count());
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);

            OrderBookRepresentation orderBooksTuple = (OrderBookRepresentation)memoryImageQueryService.GetOrderBook("BTCUSD", 3);
            Assert.AreEqual(3, orderBooksTuple.Bids.Count());
            Assert.AreEqual("BTCUSD", orderBooksTuple.Bids.CurrencyPair);
            Assert.AreEqual(100, orderBooksTuple.Bids.ToList()[0].Volume); // Volume
            Assert.AreEqual(946, orderBooksTuple.Bids.ToList()[0].Price); // Price

            Assert.AreEqual(100, orderBooksTuple.Bids.ToList()[1].Volume); // Volume
            Assert.AreEqual(945, orderBooksTuple.Bids.ToList()[1].Price); // Price

            Assert.AreEqual(100, orderBooksTuple.Bids.ToList()[2].Volume); // Volume
            Assert.AreEqual(941, orderBooksTuple.Bids.ToList()[2].Price); // Price
        }

        [Test]
        [Category("Unit")]
        public void GetAskOrderBookTest_ChecksIfOrderBookIsretreivedSuccessfully_VerifiesOrderBookToSeeItContainsValuesAsExpected()
        {
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(orderBookMemoryImage, null, null,null, null);
            LimitOrderBook limitOrderBook = new LimitOrderBook("BTCUSD");

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 946, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12347", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 940, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 700, 947, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12643", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 950, new StubbedOrderIdGenerator());

            limitOrderBook.AddOrder(buyOrder1);
            limitOrderBook.AddOrder(buyOrder2);
            limitOrderBook.AddOrder(buyOrder3);
            limitOrderBook.AddOrder(buyOrder4);
            limitOrderBook.AddOrder(sellOrder1);
            limitOrderBook.AddOrder(sellOrder2);
            limitOrderBook.AddOrder(sellOrder3);
            limitOrderBook.AddOrder(sellOrder4);

            Assert.AreEqual(4, limitOrderBook.Bids.Count());
            Assert.AreEqual(4, limitOrderBook.Asks.Count());
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);

            OrderBookRepresentation orderBooksTuple = (OrderBookRepresentation)memoryImageQueryService.GetOrderBook("BTCUSD", 3);
            Assert.AreEqual(3, orderBooksTuple.Asks.Count());
            Assert.AreEqual("BTCUSD", orderBooksTuple.Asks.CurrencyPair);
            Assert.AreEqual(700, orderBooksTuple.Asks.ToList()[0].Volume); // Volume
            Assert.AreEqual(947, orderBooksTuple.Asks.ToList()[0].Price); // Price

            Assert.AreEqual(100, orderBooksTuple.Asks.ToList()[1].Volume); // Volume
            Assert.AreEqual(948, orderBooksTuple.Asks.ToList()[1].Price); // Price

            Assert.AreEqual(100, orderBooksTuple.Asks.ToList()[2].Volume); // Volume
            Assert.AreEqual(949, orderBooksTuple.Asks.ToList()[2].Price); // Price
        }

        [Test]
        [Category("Unit")]
        public void GetBidDepthForACurrencyTest_TestsTheBidDepthRetreivalForAParticularCurrencyPair_ReturnsDepthLevelinformationForEachLevel()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(null, depthMemoryImage, null,null,null);

            Depth depth = new Depth("BTCUSD", 10);

            depth.AddOrder(new Price(941), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(941), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(942), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(943), new Volume(500), OrderSide.Buy);
            depth.AddOrder(new Price(948), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(500), OrderSide.Sell);
            depthMemoryImage.OnDepthArrived(depth);

            var returnedDepth = memoryImageQueryService.GetDepth("BTCUSD") as DepthTupleRepresentation;

            Assert.AreEqual(500, returnedDepth.BidDepth[0].Volume); // Aggregated Volume
            Assert.AreEqual(943, returnedDepth.BidDepth[0].Price); // Price
            Assert.AreEqual(1, returnedDepth.BidDepth[0].OrderCount); // OrderCount
            Assert.AreEqual(200, returnedDepth.BidDepth[1].Volume); // Aggregated Volume
            Assert.AreEqual(942, returnedDepth.BidDepth[1].Price); // Price
            Assert.AreEqual(1, returnedDepth.BidDepth[1].OrderCount); // OrderCount
            Assert.AreEqual(500, returnedDepth.BidDepth[2].Volume); // Aggregated Volume
            Assert.AreEqual(941, returnedDepth.BidDepth[2].Price); // Price
            Assert.AreEqual(2, returnedDepth.BidDepth[2].OrderCount); // OrderCount
            Assert.IsNull(returnedDepth.BidDepth[3]); // Index three is null as there is no level present for it
        }

        [Test]
        [Category("Unit")]
        public void GetAskDepthForACurrencyTest_TestsTheAskDepthRetreivalForAParticularCurrencyPair_ReturnsDepthLevelinformationForEachLevel()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(null, depthMemoryImage, null,null,null);

            Depth depth = new Depth("BTCUSD", 10);

            depth.AddOrder(new Price(941), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(941), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(942), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(943), new Volume(500), OrderSide.Buy);
            depth.AddOrder(new Price(948), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(949), new Volume(500), OrderSide.Sell);
            depthMemoryImage.OnDepthArrived(depth);

            var returnedDepth = memoryImageQueryService.GetDepth("BTCUSD") as DepthTupleRepresentation;

            Assert.AreEqual(300, returnedDepth.AskDepth[0].Volume); // Aggregated Volume
            Assert.AreEqual(948, returnedDepth.AskDepth[0].Price); // Price
            Assert.AreEqual(1, returnedDepth.AskDepth[0].OrderCount); // OrderCount
            Assert.AreEqual(900, returnedDepth.AskDepth[1].Volume); // Aggregated Volume
            Assert.AreEqual(949, returnedDepth.AskDepth[1].Price); // Price
            Assert.AreEqual(3, returnedDepth.AskDepth[1].OrderCount); // OrderCount
            Assert.IsNull(returnedDepth.AskDepth[2]); // Second index is null as there is no depth level for it
        }
    }
}
