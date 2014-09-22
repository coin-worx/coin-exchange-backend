using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.ReadModel.MemoryImages;
using NUnit.Framework;

namespace CoinExchange.Trades.ReadModel.Tests
{
    [TestFixture]
    class DepthMemoryImageTests
    {
        private const string Unit = "Unit";

        #region Depth Memory Image Direct data sending Test

        [Test]
        [Category(Unit)]
        public void BuyDepthMemoryImageInitialization_ChecksWhetherTheDictionariesInitializesAsExpected_ChecksTheCountForDepthDictionaries()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());
        }

        [Test]
        [Category(Unit)]
        public void SellDepthMemoryImageInitialization_ChecksWhetherTheDictionariesInitializesAsExpected_ChecksTheCountForDepthDictionaries()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());
        }

        [Test]
        [Category(Unit)]
        public void BuyDepthLevelsPriceCheck_ChecksWhetherTheLevelshaveTheSamePriceAsInDepthLevels_VerifiesFromTheBidDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());

            Assert.AreEqual(498, depthMemoryImage.BidDepths.First().Value.First().Price);
            Assert.AreEqual(496, depthMemoryImage.BidDepths.First().Value.ToList()[1].Price);
            Assert.AreEqual(493, depthMemoryImage.BidDepths.First().Value.ToList()[2].Price);
            Assert.AreEqual(491, depthMemoryImage.BidDepths.First().Value.ToList()[3].Price);
        }

        [Test]
        [Category(Unit)]
        public void AskDepthLevelsPriceCheck_ChecksWhetherTheLevelshaveTheSamePriceAsInDepthLevels_VerifiesFromTheBidDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());

            Assert.AreEqual(491, depthMemoryImage.AskDepths.First().Value.First().Price);
            Assert.AreEqual(493, depthMemoryImage.AskDepths.First().Value.ToList()[1].Price);
            Assert.AreEqual(496, depthMemoryImage.AskDepths.First().Value.ToList()[2].Price);
            Assert.AreEqual(498, depthMemoryImage.AskDepths.First().Value.ToList()[3].Price);
        }

        [Test]
        [Category(Unit)]
        public void BuyDepthLevelsVolumeCheck_ChecksWhetherTheLevelshaveTheSameVolumeAsInDepthLevels_VerifiesFromTheAskDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());

            Assert.AreEqual(800, depthMemoryImage.BidDepths.First().Value.First().Volume);
            Assert.AreEqual(600, depthMemoryImage.BidDepths.First().Value.ToList()[1].Volume);
            Assert.AreEqual(300, depthMemoryImage.BidDepths.First().Value.ToList()[2].Volume);
            Assert.AreEqual(100, depthMemoryImage.BidDepths.First().Value.ToList()[3].Volume);
        }

        [Test]
        [Category(Unit)]
        public void AskDepthLevelsVolumeCheck_ChecksWhetherTheLevelshaveTheSameVolumeAsInDepthLevels_VerifiesFromTheAskDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());

            Assert.AreEqual(100, depthMemoryImage.AskDepths.First().Value.First().Volume);
            Assert.AreEqual(300, depthMemoryImage.AskDepths.First().Value.ToList()[1].Volume);
            Assert.AreEqual(600, depthMemoryImage.AskDepths.First().Value.ToList()[2].Volume);
            Assert.AreEqual(800, depthMemoryImage.AskDepths.First().Value.ToList()[3].Volume);
        }

        [Test]
        [Category("Integration")]
        public void LastBidRemovedTest_TestsIfTheLastBidIsRemovedFromTheMemorymagesListOfDepths_VerifiesusingMemoryImagesBidBook()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 5);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Buy);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Sell);

            // Checks first and last bid prices
            Assert.AreEqual(498, depth.BidLevels[0].Price.Value);
            Assert.AreEqual(491, depth.BidLevels[3].Price.Value);
            // Checks first and last ask prices
            Assert.AreEqual(491, depth.AskLevels[0].Price.Value);
            Assert.AreEqual(498, depth.AskLevels[3].Price.Value);

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            // Bids in DepthMemoryImage
            Assert.AreEqual(800, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].Volume);
            Assert.AreEqual(498, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].OrderCount);
            Assert.AreEqual(600, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].Volume);
            Assert.AreEqual(496, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].OrderCount);
            Assert.AreEqual(300, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].Volume);
            Assert.AreEqual(493, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].OrderCount);
            Assert.AreEqual(100, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].Volume);
            Assert.AreEqual(491, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].OrderCount);
            // Asks in DepthMemoryImage
            Assert.AreEqual(491, depthMemoryImage.AskDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(498, depthMemoryImage.AskDepths.First().Value.DepthLevels[3].Price);

            DepthLevel depthLevel498 = depth.FindLevel(new Price(498), OrderSide.Buy, depth.BidLevels);
            depth.EraseLevel(depthLevel498, OrderSide.Buy);
            DepthLevel depthLevel496 = depth.FindLevel(new Price(496), OrderSide.Buy, depth.BidLevels);
            depth.EraseLevel(depthLevel496, OrderSide.Buy);
            DepthLevel depthLevel493 = depth.FindLevel(new Price(493), OrderSide.Buy, depth.BidLevels);
            depth.EraseLevel(depthLevel493, OrderSide.Buy);
            DepthLevel depthLevel491 = depth.FindLevel(new Price(491), OrderSide.Buy, depth.BidLevels);
            depth.EraseLevel(depthLevel491, OrderSide.Buy);

            // Checks first and last bid prices
            Assert.AreEqual(null, depth.BidLevels[0].Price);
            Assert.AreEqual(null, depth.BidLevels[3].Price);
            // Checks first and last ask prices
            Assert.AreEqual(491, depth.AskLevels[0].Price.Value);
            Assert.AreEqual(498, depth.AskLevels[3].Price.Value);

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            // Bids in DepthMemoryImage
            Assert.IsNull(depthMemoryImage.BidDepths.First().Value.DepthLevels[0]);
            Assert.IsNull(depthMemoryImage.BidDepths.First().Value.DepthLevels[1]);
            Assert.IsNull(depthMemoryImage.BidDepths.First().Value.DepthLevels[2]);
            Assert.IsNull(depthMemoryImage.BidDepths.First().Value.DepthLevels[3]);
            // Asks in DepthMemoryImage
            Assert.AreEqual(491, depthMemoryImage.AskDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(498, depthMemoryImage.AskDepths.First().Value.DepthLevels[3].Price);
        }

        [Test]
        [Category("Integration")]
        public void LastAskRemovedTest_TestsIfTheLastAskIsRemovedFromTheMemorymagesListOfDepths_VerifiesusingMemoryImagesAskBook()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BtcUsd, 5);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Buy);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Sell);

            // Checks first and last bid prices
            Assert.AreEqual(498, depth.BidLevels[0].Price.Value);
            Assert.AreEqual(491, depth.BidLevels[3].Price.Value);
            // Checks first and last ask prices
            Assert.AreEqual(491, depth.AskLevels[0].Price.Value);
            Assert.AreEqual(498, depth.AskLevels[3].Price.Value);

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            // Bids in DepthMemoryImage
            Assert.AreEqual(800, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].Volume);
            Assert.AreEqual(498, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].OrderCount);
            Assert.AreEqual(600, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].Volume);
            Assert.AreEqual(496, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].OrderCount);
            Assert.AreEqual(300, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].Volume);
            Assert.AreEqual(493, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].OrderCount);
            Assert.AreEqual(100, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].Volume);
            Assert.AreEqual(491, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].Price);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].OrderCount);
            // Asks in DepthMemoryImage
            Assert.AreEqual(100, depthMemoryImage.AskDepths.First().Value.DepthLevels[0].Volume);
            Assert.AreEqual(491, depthMemoryImage.AskDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(1, depthMemoryImage.AskDepths.First().Value.DepthLevels[0].OrderCount);
            Assert.AreEqual(300, depthMemoryImage.AskDepths.First().Value.DepthLevels[1].Volume);
            Assert.AreEqual(493, depthMemoryImage.AskDepths.First().Value.DepthLevels[1].Price);
            Assert.AreEqual(1, depthMemoryImage.AskDepths.First().Value.DepthLevels[1].OrderCount);
            Assert.AreEqual(600, depthMemoryImage.AskDepths.First().Value.DepthLevels[2].Volume);
            Assert.AreEqual(496, depthMemoryImage.AskDepths.First().Value.DepthLevels[2].Price);
            Assert.AreEqual(1, depthMemoryImage.AskDepths.First().Value.DepthLevels[2].OrderCount);
            Assert.AreEqual(800, depthMemoryImage.AskDepths.First().Value.DepthLevels[3].Volume);
            Assert.AreEqual(498, depthMemoryImage.AskDepths.First().Value.DepthLevels[3].Price);
            Assert.AreEqual(1, depthMemoryImage.AskDepths.First().Value.DepthLevels[3].OrderCount);

            DepthLevel depthLevel498 = depth.FindLevel(new Price(498), OrderSide.Sell, depth.AskLevels);
            depth.EraseLevel(depthLevel498, OrderSide.Sell);
            DepthLevel depthLevel496 = depth.FindLevel(new Price(496), OrderSide.Sell, depth.AskLevels);
            depth.EraseLevel(depthLevel496, OrderSide.Sell);
            DepthLevel depthLevel493 = depth.FindLevel(new Price(493), OrderSide.Sell, depth.AskLevels);
            depth.EraseLevel(depthLevel493, OrderSide.Sell);
            DepthLevel depthLevel491 = depth.FindLevel(new Price(491), OrderSide.Sell, depth.AskLevels);
            depth.EraseLevel(depthLevel491, OrderSide.Sell);

            // Checks first and last bid prices
            Assert.AreEqual(498, depth.BidLevels[0].Price.Value);
            Assert.AreEqual(491, depth.BidLevels[3].Price.Value);
            // Checks first and last ask prices
            Assert.AreEqual(null, depth.AskLevels[0].Price);
            Assert.AreEqual(null, depth.AskLevels[3].Price);

            depthMemoryImage.OnDepthArrived(depth);
            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            // Bids in DepthMemoryImage
            Assert.AreEqual(498, depthMemoryImage.BidDepths.First().Value.DepthLevels[0].Price);
            Assert.AreEqual(496, depthMemoryImage.BidDepths.First().Value.DepthLevels[1].Price);
            Assert.AreEqual(493, depthMemoryImage.BidDepths.First().Value.DepthLevels[2].Price);
            Assert.AreEqual(491, depthMemoryImage.BidDepths.First().Value.DepthLevels[3].Price);
            // Asks in DepthMemoryImage
            Assert.IsNull(depthMemoryImage.AskDepths.First().Value.DepthLevels[0]);
            Assert.IsNull(depthMemoryImage.AskDepths.First().Value.DepthLevels[1]);
            Assert.IsNull(depthMemoryImage.AskDepths.First().Value.DepthLevels[2]);
            Assert.IsNull(depthMemoryImage.AskDepths.First().Value.DepthLevels[3]);
        }

        #endregion Depth Memory Image Direct Data Sending Test        
    }
}
