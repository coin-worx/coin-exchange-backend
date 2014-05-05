using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.MatchingEngineServices;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Application.IntegrationTests
{
    [TestFixture]
    class LimitOrderBookRestoreTests
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
            IEventStore eventStore = new RavenNEventStore();
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
        public void MatchBuyOrdersAndRestore_MatchesBuyOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

    }
}
