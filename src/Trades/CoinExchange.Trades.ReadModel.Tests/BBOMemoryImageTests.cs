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
    class BBOMemoryImageTests
    {
        [Test]
        [Category("Unit")]
        public void BboAddTest_ChecksWhetherTheBestBidAndOfferAreAddedSuccessfully_VerifiesBBoListFromMEmoryImage()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();

            DepthLevel bestBid = new DepthLevel(new Price(491));
            bestBid.IncreaseVolume(new Volume(600));
            bestBid.UpdateOrderCount(1);

            DepthLevel bestAsk = new DepthLevel(new Price(496));
            bestAsk.IncreaseVolume(new Volume(500));
            bestAsk.UpdateOrderCount(1);

            bboMemoryImage.OnBBOArrived("XTCUSD", bestBid, bestAsk);

            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.Count());
            Assert.AreEqual(491, bboMemoryImage.BBORepresentationList.First().BestBidPrice);
            Assert.AreEqual(496, bboMemoryImage.BBORepresentationList.First().BestAskPrice);
            Assert.AreEqual(600, bboMemoryImage.BBORepresentationList.First().BestBidVolume);
            Assert.AreEqual(500, bboMemoryImage.BBORepresentationList.First().BestAskVolume);
            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.First().BestBidOrderCount);
            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.First().BestAskOrderCount);
        }
    }
}
