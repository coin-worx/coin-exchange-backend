using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Test]
        [Category(Unit)]
        public void BuyDepthMemoryImageInitialization_ChecksWhetherTheDictionariesInitializesAsExpected_ChecksTheCountForDepthDictionaries()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(4, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepth.Count());
        }

        [Test]
        [Category(Unit)]
        public void SellDepthMemoryImageInitialization_ChecksWhetherTheDictionariesInitializesAsExpected_ChecksTheCountForDepthDictionaries()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(0, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(4, depthMemoryImage.AskDepth.Count());
        }

        [Test]
        [Category(Unit)]
        public void BuyDepthLevelsPriceCheck_ChecksWhetherTheLevelshaveTheSamePriceAsInDepthLevels_VerifiesFromTheBidDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(4, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepth.Count());

            Assert.AreEqual(498, depthMemoryImage.BidDepth.ToList()[0].Item2);
            Assert.AreEqual(496, depthMemoryImage.BidDepth.ToList()[1].Item2);
            Assert.AreEqual(493, depthMemoryImage.BidDepth.ToList()[2].Item2);
            Assert.AreEqual(491, depthMemoryImage.BidDepth.ToList()[3].Item2);
        }

        [Test]
        [Category(Unit)]
        public void AskDepthLevelsPriceCheck_ChecksWhetherTheLevelshaveTheSamePriceAsInDepthLevels_VerifiesFromTheBidDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(0, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(4, depthMemoryImage.AskDepth.Count());

            Assert.AreEqual(491, depthMemoryImage.AskDepth.ToList()[0].Item2);
            Assert.AreEqual(493, depthMemoryImage.AskDepth.ToList()[1].Item2);
            Assert.AreEqual(496, depthMemoryImage.AskDepth.ToList()[2].Item2);
            Assert.AreEqual(498, depthMemoryImage.AskDepth.ToList()[3].Item2);
        }

        [Test]
        [Category(Unit)]
        public void BuyDepthLevelsVolumeCheck_ChecksWhetherTheLevelshaveTheSameVolumeAsInDepthLevels_VerifiesFromTheAskDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(4, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepth.Count());

            Assert.AreEqual(800, depthMemoryImage.BidDepth.ToList()[0].Item1);
            Assert.AreEqual(600, depthMemoryImage.BidDepth.ToList()[1].Item1);
            Assert.AreEqual(300, depthMemoryImage.BidDepth.ToList()[2].Item1);
            Assert.AreEqual(100, depthMemoryImage.BidDepth.ToList()[3].Item1);
        }

        [Test]
        [Category(Unit)]
        public void AskDepthLevelsVolumeCheck_ChecksWhetherTheLevelshaveTheSameVolumeAsInDepthLevels_VerifiesFromTheAskDepthRepresentations()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(498), new Volume(800), OrderSide.Sell);
            depth.AddOrder(new Price(493), new Volume(300), OrderSide.Sell);
            depth.AddOrder(new Price(496), new Volume(600), OrderSide.Sell);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(0, depthMemoryImage.BidDepth.Count());
            Assert.AreEqual(4, depthMemoryImage.AskDepth.Count());

            Assert.AreEqual(100, depthMemoryImage.AskDepth.ToList()[0].Item1);
            Assert.AreEqual(300, depthMemoryImage.AskDepth.ToList()[1].Item1);
            Assert.AreEqual(600, depthMemoryImage.AskDepth.ToList()[2].Item1);
            Assert.AreEqual(800, depthMemoryImage.AskDepth.ToList()[3].Item1);
        }
    }
}
