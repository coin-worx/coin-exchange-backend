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
        public void AddBuyOrderTest_ChecksIfVolumeIsMantainedDuringSorting_SortsWithDescendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 5);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(494), new Volume(200), OrderSide.Buy);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(493), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(200, depthOrderBook.BidLevels.First().AggregatedVolume.Value, "Volume at zero index in descending order");
            Assert.AreEqual(200, depthOrderBook.BidLevels[1].AggregatedVolume.Value, "Volume at first index in descending order");
            Assert.AreEqual(200, depthOrderBook.BidLevels[2].AggregatedVolume.Value, "Volume at second index in descending order");
            Assert.AreEqual(300, depthOrderBook.BidLevels[3].AggregatedVolume.Value, "Volume at third index in descending order");
            Assert.AreEqual(100, depthOrderBook.BidLevels.Last().AggregatedVolume.Value, "Volume at last index in descending order");
        }

        [Test]
        public void AddSellOrderTest_ChecksIfVolumeIsMantainedDuringSorting_SortsWithAscendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 5);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(494), new Volume(200), OrderSide.Sell);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(493), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(100, depthOrderBook.AskLevels.First().AggregatedVolume.Value, "Volume at zero index in ascending order");
            Assert.AreEqual(300, depthOrderBook.AskLevels[1].AggregatedVolume.Value, "Volume at first index in ascending order");
            Assert.AreEqual(200, depthOrderBook.AskLevels[2].AggregatedVolume.Value, "Volume at second index in ascending order");
            Assert.AreEqual(200, depthOrderBook.AskLevels[3].AggregatedVolume.Value, "Volume at third index in ascending order");
            Assert.AreEqual(200, depthOrderBook.AskLevels.Last().AggregatedVolume.Value, "Volume at last index in ascending order");
        }

        [Test]
        public void AddBuyOrderTest_ChecksIfOrderCountIsMantainedDuringSorting_SortsWithDescendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 5);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(494), new Volume(200), OrderSide.Buy);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);
            depthOrderBook.AddOrder(new Price(493), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(1, depthOrderBook.BidLevels.First().OrderCount, "OrderCount at zero index in descending order");
            Assert.AreEqual(1, depthOrderBook.BidLevels[1].OrderCount, "OrderCount at first index in descending order");
            Assert.AreEqual(1, depthOrderBook.BidLevels[2].OrderCount, "OrderCount at second index in descending order");
            Assert.AreEqual(3, depthOrderBook.BidLevels[3].OrderCount, "OrderCount at third index in descending order");
            Assert.AreEqual(2, depthOrderBook.BidLevels.Last().OrderCount, "orderCount at last index in descending order");
        }

        [Test]
        public void AddSellOrderTest_ChecksIfOrderCountIsMantainedDuringSorting_SortsWithAscendingOrder()
        {
            Depth depthOrderBook = new Depth("XBTUSD", 5);

            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(494), new Volume(200), OrderSide.Sell);
            // This price level should be inserted between the last and second last level
            depthOrderBook.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);
            depthOrderBook.AddOrder(new Price(493), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(2, depthOrderBook.AskLevels.First().OrderCount, "OrderCount at zero index in ascending order");
            Assert.AreEqual(3, depthOrderBook.AskLevels[1].OrderCount, "OrderCount at first index in ascending order");
            Assert.AreEqual(1, depthOrderBook.AskLevels[2].OrderCount, "OrderCount at second index in ascending order");
            Assert.AreEqual(1, depthOrderBook.AskLevels[3].OrderCount, "OrderCount at third index in ascending order");
            Assert.AreEqual(1, depthOrderBook.AskLevels.Last().OrderCount, "orderCount at last index in ascending order");
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
        public void FindSellLevelTest_InsertsTheNewLevelIntoTheDesiredPosition_ChecksLevelsPricesAfterInsertion()
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

        #region Change Order Quantity

        [Test]
        public void ChangeBuyOrderQuantityTest_ChecksIfMethodRemovesQuantityAsSpecified_AssertsVolumeForTheExpectedChangedLevel()
        {
            Depth depth = new Depth("XBT/USD", 10);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            DepthLevel resultingDepthLevel = (from depthLevel in depth.BidLevels
                                              where depthLevel.Price != null && depthLevel.Price.Value == 492
                                              select depthLevel).ToList().Single();
            // By only removing 100 volume from 200, we still have the depth level with a volume of 100
            depth.ChangeOrderQuantity(new Price(492), -100, OrderSide.Buy);
            Assert.IsTrue(depth.BidLevels.Contains(resultingDepthLevel));
            Assert.AreEqual(100, resultingDepthLevel.AggregatedVolume.Value, "Remaining volume after first decrease");
            Assert.AreEqual(1, resultingDepthLevel.OrderCount, "Remaining volume after first decrease");

            // By removing the remaining 100 volume, we wont have any volume left so the level needs to be removed
            depth.ChangeOrderQuantity(new Price(492), -100, OrderSide.Buy);
            Assert.IsFalse(depth.BidLevels.Contains(resultingDepthLevel));
        }

        [Test]
        public void ChangeSellOrderQuantityTest_ChecksIfMethodRemovesQuantityAsSpecified_AssertsVolumeForTheExpectedChangedLevel()
        {
            Depth depth = new Depth("XBT/USD", 10);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);

            DepthLevel resultingDepthLevel = (from depthLevel in depth.AskLevels
                                              where depthLevel.Price != null && depthLevel.Price.Value == 492
                                              select depthLevel).ToList().Single();
            // By only removing 100 volume from 200, we still have the depth level with a volume of 100
            depth.ChangeOrderQuantity(new Price(492), -100, OrderSide.Sell);
            Assert.IsTrue(depth.AskLevels.Contains(resultingDepthLevel));
            Assert.AreEqual(100, resultingDepthLevel.AggregatedVolume.Value, "Remaining volume after first decrease");
            Assert.AreEqual(1, resultingDepthLevel.OrderCount, "Remaining volume after first decrease");

            // By removing the remaining 100 volume, we wont have any volume left so the level needs to be removed
            depth.ChangeOrderQuantity(new Price(492), -100, OrderSide.Sell);
            Assert.IsFalse(depth.AskLevels.Contains(resultingDepthLevel));
        }

        #endregion Change Order Quantity

        [Test]
        public void CloseBuyOrderTest_RemovesQuantityFromThatLevel_RemovesDepthAsWellIfNoOrdersRemain()
        {
            Depth depth = AddBuyOrderToDepth();
            DepthLevel foundDepthLevel = null;

            foreach (var depthLevel in depth.BidLevels)
            {
                if (depthLevel.Price != null)
                {
                    if (depthLevel.Price.Value == 491)
                    {
                        foundDepthLevel = depthLevel;
                        break;
                    }
                }
            }

            Assert.IsTrue(depth.BidLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Buy);
            // After removing 100 volume, the depth level should still be there
            Assert.IsTrue(depth.BidLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Buy);
            // After removing 100 volume, the depth level should still be there
            Assert.IsTrue(depth.BidLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Buy);
            // After removing 100 more volume, there is no quantity left in that level so that level should be removed 
            Assert.IsFalse(depth.BidLevels.Contains(foundDepthLevel));
        }

        [Test]
        public void CloseSellOrderTest_RemovesQuantityFromThatLevel_RemovesDepthAsWellIfNoOrdersRemain()
        {
            Depth depth = AddSellOrdersToDepth();
            DepthLevel foundDepthLevel = null;

            foreach (var depthLevel in depth.AskLevels)
            {
                if (depthLevel.Price != null)
                {
                    if (depthLevel.Price.Value == 491)
                    {
                        foundDepthLevel = depthLevel;
                        break;
                    }
                }
            }

            Assert.IsTrue(depth.AskLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Sell);
            // After removing 100 volume, the depth level should still be there
            Assert.IsTrue(depth.AskLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Sell);
            // After removing 100 volume, the depth level should still be there
            Assert.IsTrue(depth.AskLevels.Contains(foundDepthLevel));
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Sell);
            // After removing 100 more volume, there is no quantity left in that level so that level should be removed 
            Assert.IsFalse(depth.AskLevels.Contains(foundDepthLevel));
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

        [Test]
        public void EraseSellLevelTest_ChecksIfOnlySpecifiedLevelsIsRemoved_VerifiesOtherLevelsVolumesStayIntact()
        {
            Depth depth = AddSellOrdersToDepth();
            // Get index 3 for testing purposes
            DepthLevel depthLevel = depth.AskLevels[1];
            depth.EraseLevel(depthLevel, OrderSide.Sell);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));

            Assert.AreEqual(100, depth.AskLevels.First().AggregatedVolume.Value,"Volume at first index");
            Assert.AreEqual(200, depth.AskLevels[1].AggregatedVolume.Value, "Volume at second index");
            Assert.AreEqual(200, depth.AskLevels[2].AggregatedVolume.Value, "Volume at third index");
            Assert.AreEqual(200, depth.AskLevels[3].AggregatedVolume.Value, "Volume at fourth index");
            Assert.AreEqual(200, depth.AskLevels[4].AggregatedVolume.Value, "Volume at fifth index");
            Assert.AreEqual(200, depth.AskLevels[5].AggregatedVolume.Value, "Volume at sixth index");
        }

        [Test]
        public void EraseBuyLevelTest_ChecksIfOnlySpecifiedLevelsIsRemoved_VerifiesOtherLevelsVolumesStayIntact()
        {
            Depth depth = AddBuyOrderToDepth();
            // Get index 3 for testing purposes
            DepthLevel depthLevel = depth.BidLevels[1];
            depth.EraseLevel(depthLevel, OrderSide.Buy);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));

            Assert.AreEqual(200, depth.BidLevels.First().AggregatedVolume.Value, "Volume at first index");
            Assert.AreEqual(200, depth.BidLevels[1].AggregatedVolume.Value, "Volume at second index");
            Assert.AreEqual(200, depth.BidLevels[2].AggregatedVolume.Value, "Volume at third index");
            Assert.AreEqual(200, depth.BidLevels[3].AggregatedVolume.Value, "Volume at fourth index");
            Assert.AreEqual(300, depth.BidLevels[4].AggregatedVolume.Value, "Volume at fifth index");
            Assert.AreEqual(100, depth.BidLevels[5].AggregatedVolume.Value, "Volume at sixth index");
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
