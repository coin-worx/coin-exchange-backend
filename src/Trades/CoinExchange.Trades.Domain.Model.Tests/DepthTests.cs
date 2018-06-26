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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    class DepthTests
    {
        #region Add Method Tests

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

        #endregion Add Method Tests

        #region Level Finding Tests

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

        #endregion Level Finding Tests

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

        #region Close Order and Erase Level Tests

        [Test]
        [Category("Unit")]
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

        [Test]
        public void EarseBidOrdersTwiceTest_RemovesOrderThenAddsThenRemovesAgain_ValidatesDepthLevelToConfirm()
        {
            Depth depth = new Depth("XBTUSD", 7);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(494M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(495M), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(496M), new Volume(200), OrderSide.Buy);

            DepthLevel depthLevel = depth.BidLevels[1]; // Price = 495;
            depth.EraseLevel(depthLevel, OrderSide.Buy);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));

            Assert.AreEqual(496, depth.BidLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(494, depth.BidLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(492, depth.BidLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(491.5, depth.BidLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(491, depth.BidLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(490, depth.BidLevels[5].Price.Value, "Price at sixth index");
            Assert.IsNull(depth.BidLevels[6].Price);

            depth.AddOrder(new Price(480), new Volume(200), OrderSide.Buy);
            Assert.AreEqual(496, depth.BidLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(494, depth.BidLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(492, depth.BidLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(491.5, depth.BidLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(491, depth.BidLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(490, depth.BidLevels[5].Price.Value, "Price at sixth index");
            Assert.AreEqual(480, depth.BidLevels[6].Price.Value, "Price at seventh index");

            DepthLevel depthLevel2 = depth.BidLevels[2];
            depth.EraseLevel(depthLevel2, OrderSide.Buy);

            Assert.AreEqual(496, depth.BidLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(494, depth.BidLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(491.5, depth.BidLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(491, depth.BidLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(490, depth.BidLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(480, depth.BidLevels[5].Price.Value, "Price at sixth index");
            Assert.IsNull(depth.BidLevels[6].Price);
        }

        [Test]
        public void EarseAskOrdersTwiceTest_RemovesOrderThenAddsThenRemovesAgain_ValidatesDepthLevelToConfirm()
        {
            Depth depth = new Depth("XBTUSD", 7);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(491.5M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(494M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(495M), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(496M), new Volume(200), OrderSide.Sell);

            DepthLevel depthLevel = depth.AskLevels[1]; // Price = 491;
            depth.EraseLevel(depthLevel, OrderSide.Sell);

            Assert.IsFalse(depth.AskLevels.Contains(depthLevel));

            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(491.5, depth.AskLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(492, depth.AskLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(494, depth.AskLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(495, depth.AskLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(496, depth.AskLevels[5].Price.Value, "Price at sixth index");
            Assert.IsNull(depth.AskLevels[6].Price);

            depth.AddOrder(new Price(499), new Volume(200), OrderSide.Sell);
            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(491.5, depth.AskLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(492, depth.AskLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(494, depth.AskLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(495, depth.AskLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(496, depth.AskLevels[5].Price.Value, "Price at sixth index");
            Assert.AreEqual(499, depth.AskLevels[6].Price.Value, "Price at seventh index");

            DepthLevel depthLevel2 = depth.AskLevels[2];
            depth.EraseLevel(depthLevel2, OrderSide.Sell);

            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "Price at first index");
            Assert.AreEqual(491.5, depth.AskLevels[1].Price.Value, "Price at second index");
            Assert.AreEqual(494, depth.AskLevels[2].Price.Value, "Price at third index");
            Assert.AreEqual(495, depth.AskLevels[3].Price.Value, "Price at fourth index");
            Assert.AreEqual(496, depth.AskLevels[4].Price.Value, "Price at fifth index");
            Assert.AreEqual(499, depth.AskLevels[5].Price.Value, "Price at sixth index");
            Assert.IsNull(depth.AskLevels[6].Price);
        }

        #endregion Close Order and Erase Level Tests

        #region Order Fill Tests

        [Test]
        public void BuyFillOrderTest_DecreasesVolumeFromThePriceLevelWhenItsOrderFills_ChecksTheVolumeOnThePriceLevelToConfirm()
        {
            Depth depth = new Depth("XBT/USD", 10);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            Assert.IsNotNull(depth.BidLevels[0].Price, "Depth level's price after expected removal of level");
            depth.FillOrder(new Price(492), new Price(490), new Volume(100), true, OrderSide.Buy);
            Assert.IsNotNull(depth.BidLevels[0].Price, "Depth level's price after expected removal of level");
            Assert.AreEqual(100, depth.BidLevels[0].AggregatedVolume.Value, "Depth level's volume after expected removal of level");

            Assert.IsNotNull(depth.BidLevels[1].Price, "Depth level's price after expected removal of level");
            depth.FillOrder(new Price(490), new Price(490), new Volume(100), true, OrderSide.Buy);
            Assert.IsNull(depth.BidLevels[1].Price, "Depth level's price after expected removal of level");
        }

        [Test]
        public void SellFillOrderTest_DecreasesVolumeFromThePriceLevelWhenItsOrderFills_ChecksTheVolumeOnThePriceLevelToConfirm()
        {
            Depth depth = new Depth("XBT/USD", 10);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);

            Assert.IsNotNull(depth.AskLevels[0].Price, "Depth level's price after expected removal of level");
            depth.FillOrder(new Price(492), new Price(490), new Volume(100), true, OrderSide.Sell);
            Assert.IsNotNull(depth.AskLevels[0].Price, "Depth level's price after expected removal of level");
            Assert.AreEqual(100, depth.AskLevels[0].AggregatedVolume.Value, "Depth level's volume after expected removal of level");

            Assert.IsNotNull(depth.AskLevels[1].Price, "Depth level's price after expected removal of level");
            depth.FillOrder(new Price(490), new Price(490), new Volume(100), true, OrderSide.Sell);
            Assert.IsNull(depth.AskLevels[1].Price, "Depth level's price after expected removal of level");
        }

        #endregion Order Fill Tests

        #region Bid Excess Levels Tests

        [Test]
        public void AddBuyOrdersInExcessLevels_IfBidIsFoundOrMovedBetweenExcessAndVisibleLevels_AssertsChecksTrueAndVerifiesUsingExcessLevel()
        {
            Depth depth = new Depth("XBT/USD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            depth.AddOrder(new Price(494), new Volume(200), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(494, depth.BidLevels.First().Price.Value, "Price of first level in Bid Depth Level Book");
            Assert.AreEqual(492, depth.BidLevels.Last().Price.Value, "Price of last level in Bid Depth Level Book");

            Assert.AreEqual(2, depth.BidExcessLevels.Count(), "2 levels must be present in the BidExcess levels");
            Assert.AreEqual(491, depth.BidExcessLevels.First().Value.Price.Value, "First index price in excess level");
            Assert.AreEqual(490, depth.BidExcessLevels.Last().Value.Price.Value, "Second index price in excess level");
        }

        [Test]
        public void AddSellOrdersInExcessLevels_IfAskIsIsFoundMovedBetweenExcessAndVisibleLevels_AssertsChecksTrueAndVerifiesUsingExcessLevel()
        {
            Depth depth = new Depth("XBT/USD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);

            depth.AddOrder(new Price(494), new Volume(200), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "Price of first level in Ask Depth Level Book");
            Assert.AreEqual(492, depth.AskLevels.Last().Price.Value, "Price of last level in Ask Depth Level Book");

            Assert.AreEqual(2, depth.AskExcessLevels.Count(), "2 levels must be present in the AskExcess levels");
            Assert.AreEqual(493, depth.AskExcessLevels.First().Value.Price.Value, "First index price in excess level");
            Assert.AreEqual(494, depth.AskExcessLevels.Last().Value.Price.Value, "Second index price in excess level");
        }

        [Test]
        public void EraseBuyLevelOrder_RemovesTheOrderAndShiftsTheLevelsOneLevelBack_ChecksTheLastElementInTheBiddepthLevels()
        {
            Depth depth = new Depth("XBT/USD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            depth.AddOrder(new Price(493), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(493, depth.BidLevels.First().Price.Value, "The price at the first level");
            Assert.AreEqual(491, depth.BidLevels.Last().Price.Value, "The price at the last level");
            Assert.AreEqual(1, depth.BidExcessLevels.Count(), "Count in the bid excess levels.");

            depth.CloseOrder(new Price(493), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(492, depth.BidLevels.First().Price.Value, "The price at the first level");
            Assert.AreEqual(490, depth.BidLevels.Last().Price.Value, "The price at the last level");
            Assert.AreEqual(0, depth.BidExcessLevels.Count(), "Count in the bid excess levels.");
        }

        [Test]
        public void EraseSellLevelOrder_RemovesTheOrderAndShiftsTheLevelsOneLevelBack_ChecksTheLastElementInTheAskDpthLevels()
        {
            Depth depth = new Depth("XBT/USD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);

            depth.AddOrder(new Price(493), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "The price at the first level");
            Assert.AreEqual(492, depth.AskLevels.Last().Price.Value, "The price at the last level");
            Assert.AreEqual(1, depth.AskExcessLevels.Count(), "Count in the bid excess levels.");

            depth.CloseOrder(new Price(492), new Volume(100), OrderSide.Sell);

            Assert.AreEqual(490, depth.AskLevels.First().Price.Value, "The price at the first level");
            Assert.AreEqual(493, depth.AskLevels.Last().Price.Value, "The price at the last level");
            Assert.AreEqual(0, depth.AskExcessLevels.Count(), "Count in the bid excess levels.");
        }

        #endregion Bid Excess Levels Tests

        [Test]
        public void BidRestorationTest_IfBidRestorationIsRequiredReturnsSecondLastDepthLevelPrice_ChecksTheBidLevelsToAssure()
        {
            Depth depth = new Depth("XBTUSD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            Assert.AreEqual(490, depth.BidLevels.Last().Price.Value);
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Buy);

            Price price1 = null;
            bool response1 = depth.NeedsBidRestoration(out price1);

            Assert.IsTrue(response1);
            Assert.AreEqual(490, price1.Value);

            depth.CloseOrder(new Price(490), new Volume(100), OrderSide.Buy);

            Price price2 = null;
            bool response2 = depth.NeedsBidRestoration(out price2);

            Assert.False(response2);
            Assert.IsNull(price2);
        }

        [Test]
        public void AskRestorationTest_IfAskRestorationIsRequiredReturnsSecondLastDepthLevelPrice_ChecksTheAskLevelsToAssure()
        {
            Depth depth = new Depth("XBTUSD", 3);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Sell);

            Assert.AreEqual(492, depth.AskLevels.Last().Price.Value);
            depth.CloseOrder(new Price(491), new Volume(100), OrderSide.Sell);

            Price price1 = null;
            bool response1 = depth.NeedsAskRestoration(out price1);

            Assert.IsTrue(response1);
            Assert.AreEqual(492, price1.Value);

            depth.CloseOrder(new Price(492), new Volume(200), OrderSide.Sell);

            Price price2 = null;
            bool response2 = depth.NeedsAskRestoration(out price2);

            Assert.False(response2);
            Assert.IsNull(price2);
        }

        #region Helper Methods

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

        #endregion Helper Methods
    }
}
