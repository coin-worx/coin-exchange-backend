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
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.IntegrationTests
{
    /// <summary>
    /// Tests the restoration of the depth after the replay adn verifies that it is as expected
    /// </summary>
    [TestFixture]
    class DepthRestorationTests
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [SetUp]
        public void Setup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private const string Integration = "Integration";

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void AddNewOrdersAndRestore_AddOrdersToLimitOrderBookAndThenRestoresThemAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(3000);
            
            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            exchange = new Exchange();

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void MatchBuyOrdersAndRestore_MatchesBuyOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 943, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 945, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 944, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(sellOrder4);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(4000);

            // Only one open buy and one open sell orders remain
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            
            // Ask levels
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);            
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Log.Debug("Replay service started for Exchange.");
            
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Ask levels
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void MatchSellOrdersAndRestore_MatchesSellOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 945, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 944, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 943, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Ask levels
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Ask levels
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }


        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void OneMatchMultipleOpen_MatchesOnePairOfOrdersLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 500, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 600, 956, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 400, 954, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 958, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 500, 945, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            // Ask levels
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            // Ask levels
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void OnePartialBuyMatchMultipleOpen_MatchesBuyOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 500, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 600, 956, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 400, 954, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 958, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 945, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);

            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].OrderCount);

            // Ask levels
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[3].OrderCount);

            // Ask levels
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void OnePartialSellMatchMultipleOpen_MatchesSellOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 600, 956, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 400, 954, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 958, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 500, 945, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(buyOrder4);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            // Ask levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            // Ask levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[3].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void CancelOrdersTest_ChecksIfOrdersAreRemovedFromTheOrderBookAfterRestore_VerifiesThroughBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 300, 943, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 944, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 500, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 600, 956, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 400, 954, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 800, 958, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 500, 946, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(buyOrder4);

            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            exchange.CancelOrder(new OrderCancellation(buyOrder3.OrderId, buyOrder3.TraderId, buyOrder3.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(sellOrder2.OrderId, sellOrder2.TraderId, sellOrder2.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(buyOrder1.OrderId, buyOrder1.TraderId, buyOrder1.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(sellOrder4.OrderId, sellOrder4.TraderId, sellOrder4.CurrencyPair));

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(5000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Ask levels
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetOrdersForReplay(exchange.ExchangeEssentials.First().LimitOrderBook);

            manualReset.Reset();
            manualReset.WaitOne(5000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Ask levels
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Unit")]
        public void BidMultipleMatchesTest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order sellOrder1 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 1252, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 300, 1251, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 1251, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 500, 1251, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("123457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 1250, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(buyOrder1);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(1250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            
            // Ask levels
            Assert.AreEqual(1252, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);

            manualReset.Reset();
            manualReset.WaitOne(2000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bids levels
            Assert.AreEqual(1250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Ask levels
            Assert.AreEqual(1252, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Unit")]
        public void AskMultipleMatchesTest_EntersAskOrdertoMatchAndUpdatesBook_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 1254, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 300, 1253, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 1253, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12355", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 700, 1251, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 500, 1251, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 1252, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 1251, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("523457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 1249, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(1249, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            // Ask levels
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(1254, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);

            // Bid levels
            Assert.AreEqual(1249, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            // Ask levels
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(1254, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void MultipleRepeatMatchBidTest_MatchesManyAsksToBidButBidVolumeRemains_VerifiesByCheckingDepthLevels()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 200, 930, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 900, 942, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12224", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(930, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);
            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(930, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void MultipleRepeatMatchAskTest_MatchesManyBidsToAskButAskVolumeRemains_VerifiesUsingOrderBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 942, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 942, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 942, new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 942, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 951, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 1000, 942, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            // Ask levels
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(951, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bid levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            // Ask levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            replayService.ReplayOrderBooks(exchange, journaler);

            manualReset.Reset();
            manualReset.WaitOne(2000);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            // Ask levels
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(951, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void AskCancelThenBidMatchTest_TestsWhetherTheOrderIsCancelledAndThenBidOrderisMatched_VerifiesThroughDepthLevels()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 948, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            // Cancel order
            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(sellOrder1.OrderId);
            Assert.IsTrue(cancelOrderResponse);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            exchange.PlaceNewOrder(buyOrder3);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            // Crash and reinitialize Exchange
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
            replayService.ReplayOrderBooks(exchange, journaler);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void BidCancelThenAskMatchTest_TestsWhetherTheOrderIsCancelledAndThenAskOrderisMatched_VerifiesThroughBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 945, new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            // Cancel Orders
            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(buyOrder1.OrderId);
            Assert.IsTrue(cancelOrderResponse);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bid levels
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            exchange.PlaceNewOrder(sellOrder3);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            // Events before crash
            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
            int cancelledOrdersCount = 0;
            foreach (Order order in preCrashOrders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
                else if (order.OrderState == OrderState.PartiallyFilled)
                {
                    ++partialFillOrdersCount;
                }
                else if (order.OrderState == OrderState.Cancelled)
                {
                    ++cancelledOrdersCount;
                }
            }
            Assert.AreEqual(5, acceptedOrdersCount);
            Assert.AreEqual(2, completeOrdersCount);
            Assert.AreEqual(0, partialFillOrdersCount);
            Assert.AreEqual(1, cancelledOrdersCount);
            Assert.AreEqual(8, preCrashOrders.Count);

            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
            replayService.ReplayOrderBooks(exchange, journaler);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            //------------------------Depth Checks------------------------------
            // Bid levels
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
            // Ask levels
            Assert.AreEqual(948, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(949, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume.Value);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
            //------------------------Depth Checks------------------------------

            OutputDisruptor.ShutDown();
        }
    }
}
