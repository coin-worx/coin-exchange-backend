using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.MatchingEngineServices;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
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
            MemoryImageQueryService queryService = new MemoryImageQueryService(bboMemoryImage);

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
    }
}
