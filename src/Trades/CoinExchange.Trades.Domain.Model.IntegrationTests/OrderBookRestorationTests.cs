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
    [TestFixture]
    class OrderBookRestorationTests
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

        [Test]
        [Category(Integration)]
        public void AddNewOrdersAndRestore_AddOrdersToLimitOrderBookAndThenRestoresThemAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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
            var preCrashOrders = eventStore.GetOrders();

            Assert.AreEqual(4, preCrashOrders.Count);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[0].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[1].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[2].OrderState);
            Assert.AreEqual(OrderState.Accepted, preCrashOrders[3].OrderState);

            exchange = new Exchange();

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();
            foreach (Order order in orders)
            {
                // Send the orders to the first OrderBook in the Exchange
                exchange.ExchangeEssentials.First().LimitOrderBook.AddOrder(order);
            }

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(OrderState.Accepted, orders[0].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[1].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[2].OrderState);
            Assert.AreEqual(OrderState.Accepted, orders[3].OrderState);

            exchange.TurnReplayModeOff();

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void MatchBuyOrdersAndRestore_MatchesBuyOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            
            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
            }

            Assert.AreEqual(14, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);
            
            exchange.TurnReplayModeOff();

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void MatchSellOrdersAndRestore_MatchesSellOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
                }
                else if (order.OrderState == OrderState.Complete)
                {
                    ++completeOrdersCount;
                }
            }

            Assert.AreEqual(14, orders.Count);
            Assert.AreEqual(8, acceptedOrdersCount);
            Assert.AreEqual(6, completeOrdersCount);

            exchange.TurnReplayModeOff();
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void OneMatchMultipleOpen_MatchesOnePairOfOrdersLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
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

        [Test]
        [Category(Integration)]
        public void OnePartialBuyMatchMultipleOpen_MatchesBuyOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            partialFillOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
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

        [Test]
        [Category(Integration)]
        public void OnePartialSellMatchMultipleOpen_MatchesSellOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            completeOrdersCount = 0;
            acceptedOrdersCount = 0;
            partialFillOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
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

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void CancelOrdersTest_ChecksIfOrdersAreRemovedFromTheOrderBookAfterRestore_VerifiesThroughBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
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

            exchange.CancelOrder(buyOrder3.OrderId);
            exchange.CancelOrder(sellOrder2.OrderId);
            exchange.CancelOrder(buyOrder1.OrderId);
            exchange.CancelOrder(sellOrder4.OrderId);

            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(5000);

            var preCrashOrders = eventStore.GetOrders();
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

            // Get all orders from the EventStore and send to the LimitOrderBook to restore its state
            var orders = eventStore.GetOrders();
            // Tell the exchange that replay mode is on
            exchange.TurnReplayModeOn();

            acceptedOrdersCount = 0;
            cancelledOrdersCount = 0;

            foreach (Order order in orders)
            {
                if (order.OrderState == OrderState.Accepted)
                {
                    ++acceptedOrdersCount;
                    // Send the orders to the first OrderBook in the Exchange
                    exchange.PlaceNewOrder(order);
                }
                else if (order.OrderState == OrderState.Cancelled)
                {
                    ++cancelledOrdersCount;
                    exchange.CancelOrder(order.OrderId);
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
        // ToDo: Combine test of New, filled, partially filled and cancel test
    }
}
