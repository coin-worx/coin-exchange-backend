using System;
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
            MarketDataQueryService queryService = new MarketDataQueryService(null, null, bboMemoryImage);

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
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(orderBookMemoryImage, null, null);
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

            Tuple<OrderRepresentationList, OrderRepresentationList> bidList = memoryImageQueryService.GetOrderBook("BTCUSD", 10);
            Assert.AreEqual("BTCUSD", bidList.Item1.CurrencyPair);
            Assert.AreEqual(946, bidList.Item1.ToList()[0].Item2); // Volume
            Assert.AreEqual(100, bidList.Item1.ToList()[0].Item1); // Price

            Assert.AreEqual(945, bidList.Item1.ToList()[1].Item2); // Volume
            Assert.AreEqual(100, bidList.Item1.ToList()[1].Item1); // Price

            Assert.AreEqual(941, bidList.Item1.ToList()[2].Item2); // Volume
            Assert.AreEqual(100, bidList.Item1.ToList()[2].Item1); // Price
        }

        [Test]
        [Category("Unit")]
        public void GetAskOrderBookTest_ChecksIfOrderBookIsretreivedSuccessfully_VerifiesOrderBookToSeeItContainsValuesAsExpected()
        {
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(orderBookMemoryImage,null, null);
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

            Tuple<OrderRepresentationList, OrderRepresentationList> askList = memoryImageQueryService.GetOrderBook("BTCUSD", 10);
            Assert.AreEqual("BTCUSD", askList.Item2.CurrencyPair);
            Assert.AreEqual(947, askList.Item2.ToList()[0].Item2); // Volume
            Assert.AreEqual(100, askList.Item2.ToList()[0].Item1); // Price

            Assert.AreEqual(948, askList.Item2.ToList()[1].Item2); // Volume
            Assert.AreEqual(100, askList.Item2.ToList()[1].Item1); // Price

            Assert.AreEqual(949, askList.Item2.ToList()[2].Item2); // Volume
            Assert.AreEqual(100, askList.Item2.ToList()[2].Item1); // Price
        }

        [Test]
        [Category("Unit")]
        public void GetBidDepthForACurrencyTest_TestsTheBidDepthRetreivalForAParticularCurrencyPair_ReturnsDepthLevelinformationForEachLevel()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(null, depthMemoryImage, null);

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

            Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> returnedDepth = memoryImageQueryService.GetDepth("BTCUSD");

            Assert.AreEqual(500, returnedDepth.Item1[0].Item1); // Aggregated Volume
            Assert.AreEqual(943, returnedDepth.Item1[0].Item2); // Price
            Assert.AreEqual(1, returnedDepth.Item1[0].Item3); // OrderCount
            Assert.AreEqual(200, returnedDepth.Item1[1].Item1); // Aggregated Volume
            Assert.AreEqual(942, returnedDepth.Item1[1].Item2); // Price
            Assert.AreEqual(1, returnedDepth.Item1[1].Item3); // OrderCount
            Assert.AreEqual(500, returnedDepth.Item1[2].Item1); // Aggregated Volume
            Assert.AreEqual(941, returnedDepth.Item1[2].Item2); // Price
            Assert.AreEqual(2, returnedDepth.Item1[2].Item3); // OrderCount
            Assert.IsNull(returnedDepth.Item1[3]); // Index three is null as there is no level present for it
        }

        [Test]
        [Category("Unit")]
        public void GetAskDepthForACurrencyTest_TestsTheAskDepthRetreivalForAParticularCurrencyPair_ReturnsDepthLevelinformationForEachLevel()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            MarketDataQueryService memoryImageQueryService = new MarketDataQueryService(null, depthMemoryImage, null);

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

            Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> returnedDepth = memoryImageQueryService.GetDepth("BTCUSD");

            Assert.AreEqual(300, returnedDepth.Item2[0].Item1); // Aggregated Volume
            Assert.AreEqual(948, returnedDepth.Item2[0].Item2); // Price
            Assert.AreEqual(1, returnedDepth.Item2[0].Item3); // OrderCount
            Assert.AreEqual(900, returnedDepth.Item2[1].Item1); // Aggregated Volume
            Assert.AreEqual(949, returnedDepth.Item2[1].Item2); // Price
            Assert.AreEqual(3, returnedDepth.Item2[1].Item3); // OrderCount
            Assert.IsNull(returnedDepth.Item2[2]); // Second index is null as there is no depth level for it
        }
    }
}
