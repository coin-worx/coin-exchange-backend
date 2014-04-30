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
            Depth depth = new Depth(CurrencyConstants.BitCoinUsd, 10);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(498), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(493), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(496), new Volume(100), OrderSide.Buy);

            Assert.AreEqual(10, depth.BidLevels.Count());
            Assert.AreEqual(10, depth.AskLevels.Count());

            depthMemoryImage.OnDepthArrived(depth);

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());
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

            Assert.AreEqual(0, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());
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

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());

            Assert.AreEqual(498, depthMemoryImage.BidDepths.First().Value.First().Item2);
            Assert.AreEqual(496, depthMemoryImage.BidDepths.First().Value.ToList()[1].Item2);
            Assert.AreEqual(493, depthMemoryImage.BidDepths.First().Value.ToList()[2].Item2);
            Assert.AreEqual(491, depthMemoryImage.BidDepths.First().Value.ToList()[3].Item2);
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

            Assert.AreEqual(0, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());

            Assert.AreEqual(491, depthMemoryImage.AskDepths.First().Value.First().Item2);
            Assert.AreEqual(493, depthMemoryImage.AskDepths.First().Value.ToList()[1].Item2);
            Assert.AreEqual(496, depthMemoryImage.AskDepths.First().Value.ToList()[2].Item2);
            Assert.AreEqual(498, depthMemoryImage.AskDepths.First().Value.ToList()[3].Item2);
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

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(0, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.BidDepths.First().Value.Count());

            Assert.AreEqual(800, depthMemoryImage.BidDepths.First().Value.First().Item1);
            Assert.AreEqual(600, depthMemoryImage.BidDepths.First().Value.ToList()[1].Item1);
            Assert.AreEqual(300, depthMemoryImage.BidDepths.First().Value.ToList()[2].Item1);
            Assert.AreEqual(100, depthMemoryImage.BidDepths.First().Value.ToList()[3].Item1);
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

            Assert.AreEqual(0, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            Assert.AreEqual(4, depthMemoryImage.AskDepths.First().Value.Count());

            Assert.AreEqual(100, depthMemoryImage.AskDepths.First().Value.First().Item1);
            Assert.AreEqual(300, depthMemoryImage.AskDepths.First().Value.ToList()[1].Item1);
            Assert.AreEqual(600, depthMemoryImage.AskDepths.First().Value.ToList()[2].Item1);
            Assert.AreEqual(800, depthMemoryImage.AskDepths.First().Value.ToList()[3].Item1);
        }

        #endregion Depth Memory Image Direct data sending Test        
    }
}
