using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.MatchingEngineServices;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.MemoryImages;
using NUnit.Framework;

namespace CoinExchange.Trades.Application.Tests
{
    [TestFixture]
    class MemoryImageQueryServiceTests
    {
        [Test]
        [Category("Unit")]
        public void GetBboTest_ChecksIfTheBboIsRetreivedSuccessfully_ReturnsBboForCurrencypairIfPresent()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();
            MemoryImageQueryService queryService = new MemoryImageQueryService(null, bboMemoryImage);

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
            MemoryImageQueryService memoryImageQueryService = new MemoryImageQueryService(orderBookMemoryImage, null);
            LimitOrderBook limitOrderBook = new LimitOrderBook("BTCUSD");

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 946, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 947, new StubbedOrderIdGenerator());

            limitOrderBook.AddOrder(buyOrder1);
            limitOrderBook.AddOrder(buyOrder2);
            limitOrderBook.AddOrder(buyOrder3);
            limitOrderBook.AddOrder(sellOrder1);
            limitOrderBook.AddOrder(sellOrder2);
            limitOrderBook.AddOrder(sellOrder3);

            Assert.AreEqual(3, limitOrderBook.Bids.Count());
            Assert.AreEqual(3, limitOrderBook.Asks.Count());
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);

            OrderRepresentationList bidList = memoryImageQueryService.GetBidBook("BTCUSD");
            Assert.AreEqual("BTCUSD", bidList.CurrencyPair);
            Assert.AreEqual(946, bidList.ToList()[0].Item2); // Volume
            Assert.AreEqual(100, bidList.ToList()[0].Item1); // Price

            Assert.AreEqual(945, bidList.ToList()[1].Item2); // Volume
            Assert.AreEqual(100, bidList.ToList()[1].Item1); // Price

            Assert.AreEqual(941, bidList.ToList()[2].Item2); // Volume
            Assert.AreEqual(100, bidList.ToList()[2].Item1); // Price
        }

        [Test]
        [Category("Unit")]
        public void GetAskOrderBookTest_ChecksIfOrderBookIsretreivedSuccessfully_VerifiesOrderBookToSeeItContainsValuesAsExpected()
        {
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();
            MemoryImageQueryService memoryImageQueryService = new MemoryImageQueryService(orderBookMemoryImage, null);
            LimitOrderBook limitOrderBook = new LimitOrderBook("BTCUSD");

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 946, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 947, new StubbedOrderIdGenerator());

            limitOrderBook.AddOrder(buyOrder1);
            limitOrderBook.AddOrder(buyOrder2);
            limitOrderBook.AddOrder(buyOrder3);
            limitOrderBook.AddOrder(sellOrder1);
            limitOrderBook.AddOrder(sellOrder2);
            limitOrderBook.AddOrder(sellOrder3);

            Assert.AreEqual(3, limitOrderBook.Bids.Count());
            Assert.AreEqual(3, limitOrderBook.Asks.Count());
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);

            OrderRepresentationList askList = memoryImageQueryService.GetAskBook("BTCUSD");
            Assert.AreEqual("BTCUSD", askList.CurrencyPair);
            Assert.AreEqual(947, askList.ToList()[0].Item2); // Volume
            Assert.AreEqual(100, askList.ToList()[0].Item1); // Price

            Assert.AreEqual(948, askList.ToList()[1].Item2); // Volume
            Assert.AreEqual(100, askList.ToList()[1].Item1); // Price

            Assert.AreEqual(949, askList.ToList()[2].Item2); // Volume
            Assert.AreEqual(100, askList.ToList()[2].Item1); // Price
        }
    }
}
