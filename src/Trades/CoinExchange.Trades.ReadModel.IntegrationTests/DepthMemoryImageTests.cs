using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.MemoryImages;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.ReadModel.IntegrationTests
{
    [TestFixture]
    class DepthMemoryImageTests
    {
        private const string Integration = "Integration";

        #region Disruptor Linkage Tests

        [Test]
        [Category(Integration)]
        public void NewOrderDepthUpdatetest_ChecksWhetherDepthMemeoryImageGetsUpdatedWhenOrdersAreInserted_VerifiesThroughMemoryImagesLists()
        {
            DepthMemoryImage depthMemoryImage = new DepthMemoryImage();
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });

            // Start exchagne to accept orders
            Exchange exchange = new Exchange();
            Order buyOrder1 = OrderFactory.CreateOrder("1233", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 491.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 496.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 494.34M, new StubbedOrderIdGenerator());

            // No matching orders till now
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, depthMemoryImage.BidDepths.Count());
            Assert.AreEqual(1, depthMemoryImage.AskDepths.Count());

            /*DepthRepresentation depth = new DepthRepresentation();
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, depthMemoryImage.BidDepths);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, depthMemoryImage.AskDepths.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, depthMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, depthMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");
*/
        }

        #endregion Disruptor Linkage Tests
    }
}
