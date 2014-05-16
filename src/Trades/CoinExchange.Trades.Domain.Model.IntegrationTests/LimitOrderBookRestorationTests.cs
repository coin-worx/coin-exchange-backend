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
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.IntegrationTests
{
    // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
    // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS

    /// <summary>
    /// Tests the prevention of raise of events after replaying them for order book and verifies the rebuilt order book's 
    /// state after the replay
    /// </summary>
    [TestFixture]
    internal class LimitOrderBookRestorationTests
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IEventStore eventStore;
        private Journaler journaler;
        [SetUp]
        public void Setup()
        {
            log4net.Config.XmlConfigurator.Configure();
            eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
        }

        [TearDown]
        public void TearDown()
        {
            OutputDisruptor.ShutDown();
            eventStore.RemoveAllEvents();
        }

        private const string Integration = "Integration";

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void AddNewOrdersAndRestore_AddOrdersToLimitOrderBookAndThenRestoresThemAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 949,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 948,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            Thread.Sleep(1500);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(3000);
            var preCrashOrders = journaler.GetAllOrders();

            Assert.AreEqual(4, preCrashOrders.Count);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[0].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[1].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[2].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[3].OrderState);

            exchange = new Exchange();

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(1500);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(OrderState.Accepted, orders[0].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[1].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[2].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[3].OrderState);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void
            MatchBuyOrdersAndRestore_MatchesBuyOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks
            ()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 100, 949,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 943,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 945,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 944,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(sellOrder4);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            Thread.Sleep(3000);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(4000);

            var preCrashOrders = journaler.GetAllOrders();
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);
            // Only one open buy and one open sell orders remain
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);
            Log.Debug("Replay service started for Exchange.");
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
            }

            Assert.AreEqual(14, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void
            MatchSellOrdersAndRestore_MatchesSellOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks
            ()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
           // IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 100, 949,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 945,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 944,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 943,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            Thread.Sleep(3000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            var preCrashOrders = journaler.GetAllOrders();
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);
            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
            }

            Assert.AreEqual(14, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void
            OneMatchMultipleOpen_MatchesOnePairOfOrdersLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks
            ()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
           // IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            //OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 600, 956,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 400, 954,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 958,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 500, 945,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            Thread.Sleep(3000);

            var preCrashOrders = journaler.GetAllOrders();
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(2, completeOrdersCount);
            Assert.AreEqual(10, preCrashOrders.Count);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
            }

            Assert.AreEqual(10, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(2, completeOrdersCount);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void
            OnePartialBuyMatchMultipleOpen_MatchesBuyOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks
            ()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
           // IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
            //OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 600, 956,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 400, 954,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 958,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 945,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            Thread.Sleep(3000);

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(1, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);
            Assert.AreEqual(10, preCrashOrders.Count);

            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            // Check open quantity after partial fill
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            // Third slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);

            // Fourth slot in bid book only
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].CurrencyPair);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            partialFillOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
                if (order.OrderState == OrderState.PartiallyFilled)
                {
                    ++partialFillOrdersCount;
                }
            }

            Assert.AreEqual(10, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(1, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);

            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            // Check open quantity after partial fill
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            // Third slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);

            // Fourth slot in bid book only
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].CurrencyPair);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[3].Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void
            OnePartialSellMatchMultipleOpen_MatchesSellOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks
            ()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           // Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 600, 956,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 400, 954,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 958,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 500, 945,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder4);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(buyOrder4);
            Thread.Sleep(3000);

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(1, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);
            Assert.AreEqual(10, preCrashOrders.Count);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            // Third slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);

            // Fourth slot in Ask only, as Bids book count is 3
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetAllOrders();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            partialFillOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
                if (order.OrderState == OrderState.PartiallyFilled)
                {
                    ++partialFillOrdersCount;
                }
            }

            Assert.AreEqual(10, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(1, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(954, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);

            // Third slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].CurrencyPair);
            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[2].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);

            // Fourth slot in Ask only, as Bids book count is 3
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[3].Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // IMP NOTE: NEED TO RUN THIS TEST AS SEPARATE STANDALONE, AS EVENTSTORE FETCHES EVENTS FOR THE ENTIRE SESSION, AND
        // OLDER EVENTS ARE FETCHED TOO AND SO THE TEST WILL FAIL IF RUN IN CONJUNCTION WITH OTHER TESTS
        [Test]
        [Category(Integration)]
        public void CancelOrdersTest_ChecksIfOrdersAreRemovedFromTheOrderBookAfterRestore_VerifiesThroughBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           // Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
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
                                                        Constants.ORDER_SIDE_SELL, 600, 956,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 400, 954,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12225", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 800, 958,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12226", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 500, 946,
                                                        new StubbedOrderIdGenerator());

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

            exchange.CancelOrder(new OrderCancellation(buyOrder3.OrderId,buyOrder3.TraderId, buyOrder3.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(sellOrder2.OrderId, sellOrder2.TraderId, sellOrder2.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(buyOrder1.OrderId, buyOrder1.TraderId, buyOrder1.CurrencyPair));
            exchange.CancelOrder(new OrderCancellation(sellOrder4.OrderId, sellOrder4.TraderId, sellOrder4.CurrencyPair));

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(5000);

            var preCrashOrders = journaler.GetAllOrders();
            int cancelledOrdersCount = 0;
            int acceptedOrdersCount = 0;
            foreach (Order order in preCrashOrders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Cancelled)
                {
                    ++cancelledOrdersCount;
                }
            }

            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(4, cancelledOrdersCount);
            Assert.AreEqual(12, preCrashOrders.Count);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(2000);
            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = journaler.GetOrdersForReplay(exchange.ExchangeEssentials.First().LimitOrderBook);

            acceptedOrdersCount = 0;
            cancelledOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                }
                else if (order.OrderState == OrderState.Cancelled)
                {
                    ++cancelledOrdersCount;
                }
            }

            Assert.AreEqual(12, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(4, cancelledOrdersCount);

            manualReset.Reset();
            manualReset.WaitOne(5000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(956, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(958, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(800, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void BidMultipleMatchesTest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesBidsAnsAsksIfTheyAreAsExpected()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           // Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event store
            Exchange exchange = new Exchange();

            Order sellOrder1 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 250, 1252,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 300, 1251,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 200, 1251,
                                                        new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 500, 1251,
                                                       new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("123457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 250, 1250,
                                                       new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(buyOrder1);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(5, acceptedOrdersCount);
            Assert.AreEqual(3, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);
            Assert.AreEqual(9, preCrashOrders.Count);

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(1250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].CurrencyPair);
            Assert.AreEqual(1252, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Volume.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].OpenQuantity.Value);

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);

            manualReset.Reset();
            manualReset.WaitOne(3000);

            // First slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(1250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            // Second slot for bids and asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].CurrencyPair);
            Assert.AreEqual(1252, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Price.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Volume.Value);
            Assert.AreEqual(250, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].OpenQuantity.Value);

            var postCrashorder = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            foreach (Order order in postCrashorder)
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
            }
            Assert.AreEqual(5, acceptedOrdersCount);
            Assert.AreEqual(3, completeOrdersCount);
            Assert.AreEqual(1, partialFillOrdersCount);
            Assert.AreEqual(9, preCrashOrders.Count);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void AskMultipleMatchesTest_EntersAskOrdertoMatchAndUpdatesBook_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
           // IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 200, 1254,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 300, 1253,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 200, 1253,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12355", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 700, 1251,
                                                        new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 500, 1251,
                                                       new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 100, 1252,
                                                       new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 100, 1251,
                                                       new StubbedOrderIdGenerator());
            Order buyOrder4 = OrderFactory.CreateOrder("523457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 100, 1249,
                                                       new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder4);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(2, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Bid
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(1249, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            // Asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].CurrencyPair);
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].CurrencyPair);
            Assert.AreEqual(1254, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].OpenQuantity.Value);

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);

            var postCrashOrders = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            foreach (Order order in postCrashOrders)
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
            }
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(2, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Bid
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(1249, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);

            // Asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].CurrencyPair);
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[0].OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(1253, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(300, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].CurrencyPair);
            Assert.AreEqual(1254, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[2].OpenQuantity.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void MultipleRepeatMatchBidTest_MatchesManyAsksToBidButBidVolumeRemains_VerifiesbyOrderBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
           // IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
            // Intialize the exchange so that the order changes can be fired to listernes which will then log them to event 
            // store
            Exchange exchange = new Exchange();

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 200, 930, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                       Constants.ORDER_SIDE_BUY, 900, 942, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 942,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 942,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 942,
                                                        new StubbedOrderIdGenerator());
            Order sellOrder4 = OrderFactory.CreateOrder("12224", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                                                        Constants.ORDER_SIDE_SELL, 100, 942,
                                                        new StubbedOrderIdGenerator());

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder3);
            exchange.PlaceNewOrder(sellOrder4);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(6, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(4, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Bids
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(900, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(930, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].OpenQuantity.Value);

            exchange = new Exchange();
            replayService.ReplayOrderBooks(exchange, journaler);
            Thread.Sleep(3000);

            var postCrashOrders = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            foreach (Order order in postCrashOrders)
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
            }
            Assert.AreEqual(6, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(4, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Bids
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(900, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);
            Assert.AreEqual(500, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(930, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual(200, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.ToList()[1].OpenQuantity.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void MultipleRepeatMatchAskTest_MatchesManyBidsToAskButAskVolumeRemains_VerifiesUsingOrderBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           // Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
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

            var preCrashOrders = journaler.GetAllOrders();
            int partialFillOrdersCount = 0;
            int completeOrdersCount = 0;
            int acceptedOrdersCount = 0;
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
            }
            Assert.AreEqual(6, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(4, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(1000, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(951, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            exchange = new Exchange();
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
            replayService.ReplayOrderBooks(exchange, journaler);

            manualReset.Reset();
            manualReset.WaitOne(2000);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var postCrashOrders = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            foreach (Order order in postCrashOrders)
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
            }
            Assert.AreEqual(6, acceptedOrdersCount);
            Assert.AreEqual(4, completeOrdersCount);
            Assert.AreEqual(4, partialFillOrdersCount);
            Assert.AreEqual(14, preCrashOrders.Count);

            // Asks
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().CurrencyPair);
            Assert.AreEqual(942, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(1000, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);
            Assert.AreEqual(600, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].CurrencyPair);
            Assert.AreEqual(951, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].Volume.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.ToList()[1].OpenQuantity.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void AskCancelThenBidMatchTest_TestsWhetherTheOrderIsCancelledAndThenBidOrderisMatched_VerifiesThroughBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
           // Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
           // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
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
            Thread.Sleep(2000);
            
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(sellOrder1.OrderId);
            Assert.IsTrue(cancelOrderResponse);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            exchange.PlaceNewOrder(buyOrder3);
            Thread.Sleep(800);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

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
            Thread.Sleep(3000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var postCrashOrders = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            cancelledOrdersCount = 0;
            foreach (Order order in postCrashOrders)
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

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void BidCancelThenAskMatchTest_TestsWhetherTheOrderIsCancelledAndThenAskOrderisMatched_VerifiesThroughBooksLists()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            //IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler journaler = new Journaler(eventStore);
            LimitOrderBookReplayService replayService = new LimitOrderBookReplayService();
          //  OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
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
            Thread.Sleep(4000);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(buyOrder1.OrderId);
            Assert.IsTrue(cancelOrderResponse);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(2000);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            exchange.PlaceNewOrder(sellOrder3);
            Thread.Sleep(800);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

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
            Thread.Sleep(3000);

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            var postCrashOrders = journaler.GetAllOrders();
            partialFillOrdersCount = 0;
            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            cancelledOrdersCount = 0;
            foreach (Order order in postCrashOrders)
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

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            OutputDisruptor.ShutDown();
        }
    }
}
