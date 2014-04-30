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
        private const string Integration = "Integration";

        [Test]
        [Category(Integration)]
        public void AddNewOrdersAndRestore_AddOrdersToLimitOrderBookAndThenRestoresThemAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

            exchange.OrderBook.AddOrder(buyOrder1);
            exchange.OrderBook.AddOrder(buyOrder2);
            exchange.OrderBook.AddOrder(sellOrder1);
            exchange.OrderBook.AddOrder(sellOrder2);

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            exchange = new Exchange();

            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            exchange.OrderBook.RestoreLimitOrderBook(eventStore);

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void MatchBuyOrdersAndRestore_MatchesBuyOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

            // Only one open buy and one open sell orders remain
            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Asks.First().Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            exchange.OrderBook.RestoreLimitOrderBook(eventStore);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void MatchSellOrdersAndRestore_MatchesSellOrdersAndThenRestoresOpenOrdersAfterOrderBookCrash_VerifiesFromTheBidAndAskBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Asks.First().Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            exchange.OrderBook.RestoreLimitOrderBook(eventStore);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(941, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.First().Volume.Value);

            Assert.AreEqual(949, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Asks.First().Volume.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void OneMatchMultipleOpen_MatchesOnePairOfOrdersLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Bids.First().Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.OrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.OrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.OrderBook.Asks.ToList()[1].Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            exchange.OrderBook.RestoreLimitOrderBook(eventStore);

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(944, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Bids.First().Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(943, exchange.OrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(100, exchange.OrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.OrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.OrderBook.Asks.ToList()[1].Volume.Value);

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void OnePartialBuyMatchMultipleOpen_MatchesBuyOrderPartiallyLeavesTheRestAndthenRestoresThemAfterExchangeCrash_VerifiesThroughBidsAndAsksBooks()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
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

            Assert.AreEqual(4, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.OrderBook.Bids.First().Volume.Value);
            // Check open quantity after partial fill
            Assert.AreEqual(400, exchange.OrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(944, exchange.OrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.OrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.OrderBook.Asks.ToList()[1].Volume.Value);

            // Crash Exchange and order book
            exchange = new Exchange();
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            exchange.OrderBook.RestoreLimitOrderBook(eventStore);

            Assert.AreEqual(4, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(945, exchange.OrderBook.Bids.First().Price.Value);
            Assert.AreEqual(500, exchange.OrderBook.Bids.First().Volume.Value);
            // Check open quantity after partial fill
            Assert.AreEqual(400, exchange.OrderBook.Bids.First().OpenQuantity.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.First().CurrencyPair);
            Assert.AreEqual(954, exchange.OrderBook.Asks.First().Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Asks.First().Volume.Value);

            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(944, exchange.OrderBook.Bids.ToList()[1].Price.Value);
            Assert.AreEqual(400, exchange.OrderBook.Bids.ToList()[1].Volume.Value);
            Assert.AreEqual("BTCUSD", exchange.OrderBook.Bids.ToList()[1].CurrencyPair);
            Assert.AreEqual(956, exchange.OrderBook.Asks.ToList()[1].Price.Value);
            Assert.AreEqual(600, exchange.OrderBook.Asks.ToList()[1].Volume.Value);

            OutputDisruptor.ShutDown();
        }

        // Remaining tests
        // ToDo: Partial Sells/Buys and restore
        // ToDo: Cancelled orders and restore
        // ToDo: Combine test of New, filled, partially filled and cancel test
    }
}
