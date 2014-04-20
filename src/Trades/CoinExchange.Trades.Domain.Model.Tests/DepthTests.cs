using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    class DepthTests
    {
        [Test]
        public void AddSellOrderTest_ChecksWhetherTheDepthLevelGetsInsertedCorrectly_SortsWithAscendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 4);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(490, depthOrderBook.AskLevels.First().Price.Value, "Price at zero index in ascending order");
            Assert.AreEqual(491, depthOrderBook.AskLevels[1].Price.Value, "Price at zero index in ascending order");
            Assert.AreEqual(491.5, depthOrderBook.AskLevels[2].Price.Value, "Price at second index in ascending order");
            Assert.AreEqual(492, depthOrderBook.AskLevels[3].Price.Value, "Price at last index in ascending order");
        }

        [Test]
        public void AddBuyOrderTest_ChecksWhetherTheDepthLevelGetsInsertedCorrectly_SortsWithAscendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 4);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(492, depthOrderBook.BidLevels.First().Price.Value, "Price at zero index in descending order");
            Assert.AreEqual(491.5, depthOrderBook.BidLevels[1].Price.Value, "Price at zero index in descending order");
            Assert.AreEqual(491, depthOrderBook.BidLevels[2].Price.Value, "Price at second index in descending order");
            Assert.AreEqual(490, depthOrderBook.BidLevels[3].Price.Value, "Price at last index in descending order");
        }

        [Test]
        public void FindBuyLevelTest_InsertsTheNewLevelIntoTheDesiredPosition_ChecksLevelsAfterInsertion()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 10);

            depthOrderBook.FindLevel(new Price(490), OrderSide.Buy, depthOrderBook.BidLevels);
            depthOrderBook.FindLevel(new Price(491), OrderSide.Buy, depthOrderBook.BidLevels);
            depthOrderBook.FindLevel(new Price(492), OrderSide.Buy, depthOrderBook.BidLevels);
            depthOrderBook.FindLevel(new Price(494), OrderSide.Buy, depthOrderBook.BidLevels);
            // Price that comes in between the currently present prices
            depthOrderBook.FindLevel(new Price(493), OrderSide.Buy, depthOrderBook.BidLevels);

            Assert.AreEqual(494, depthOrderBook.BidLevels.First().Price.Value, "Price at zero index in descending order");
            Assert.AreEqual(493, depthOrderBook.BidLevels[1].Price.Value, "Price at zero index in descending order");
            Assert.AreEqual(492, depthOrderBook.BidLevels[2].Price.Value, "Price at second index in descending order");
            Assert.AreEqual(491, depthOrderBook.BidLevels[3].Price.Value, "Price at last index in descending order");
            Assert.AreEqual(490, depthOrderBook.BidLevels[4].Price.Value, "Price at last index in descending order");
        }

        [Test]
        public void FindSellLevelTest_InsertsTheNewLevelIntoTheDesiredPosition_ChecksLevelsAfterInsertion()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 10);

            depthOrderBook.FindLevel(new Price(490), OrderSide.Sell, depthOrderBook.AskLevels);
            depthOrderBook.FindLevel(new Price(491), OrderSide.Sell, depthOrderBook.AskLevels);
            depthOrderBook.FindLevel(new Price(492), OrderSide.Sell, depthOrderBook.AskLevels);
            depthOrderBook.FindLevel(new Price(494), OrderSide.Sell, depthOrderBook.AskLevels);
            // Price that comes in between the currently present prices
            depthOrderBook.FindLevel(new Price(493), OrderSide.Sell, depthOrderBook.AskLevels);

            Assert.AreEqual(490, depthOrderBook.AskLevels.First().Price.Value, "Price at zero index in ascending order");
            Assert.AreEqual(491, depthOrderBook.AskLevels[1].Price.Value, "Price at zero index in ascending order");
            Assert.AreEqual(492, depthOrderBook.AskLevels[2].Price.Value, "Price at second index in ascending order");
            Assert.AreEqual(493, depthOrderBook.AskLevels[3].Price.Value, "Price at last index in ascending order");
            Assert.AreEqual(494, depthOrderBook.AskLevels[4].Price.Value, "Price at last index in ascending order");
        }

        [Test]
        public void EraseSellLevelTest_RemovesAndMovesLevels_OrderOfLevelsSpecifiesIfDoneAccuretaly()
        {
            Depth depth = AddSellOrdersToDepth();

            DepthLevel depthLevel = depth.AskLevels[3];
            depth.EraseLevel(depthLevel, OrderSide.Sell);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));
        }

        [Test]
        public void EraseBuyLevelTest_RemovesAndMovesLevels_OrderOfLevelsSpecifiesIfDoneAccuretaly()
        {
            Depth depth = AddBuyOrderToDepth();

            DepthLevel depthLevel = depth.BidLevels[3];
            depth.EraseLevel(depthLevel, OrderSide.Buy);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));
        }

        private Depth AddSellOrdersToDepth()
        {
            Depth depth = new Depth("XBTUSD", 10);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);
            // This price level should be inserted between the last and second last level
            depth.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(494M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(495M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(496M), new Volume(200), OrderSide.Sell);
            return depth;
        }

        private Depth AddBuyOrderToDepth()
        {
            Depth depth = new Depth("XBTUSD", 10);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);
            // This price level should be inserted between the last and second last level
            depth.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(494M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(495M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(496M), new Volume(200), OrderSide.Buy);
            return depth;
        }
    }
}
