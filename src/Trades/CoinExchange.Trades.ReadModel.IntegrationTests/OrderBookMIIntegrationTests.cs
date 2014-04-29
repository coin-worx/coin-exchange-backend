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
    class OrderBookMIIntegrationTests
    {
        private const string Integration = "Integration";

        #region Disruptor Tests

        [Test]
        [Category(Integration)]
        public void ManualOrderBookSendTest_ManuallySendsTheOrderBook_VerifiesIfTheOrderBookIsReceivedByTheDisruptorsEventHandlerMemoryImage()
        {
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

            IEventStore eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(eventStore);
            LimitOrderBook limitOrderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order buyOrder1 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 491.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 494.34M, new StubbedOrderIdGenerator());
            limitOrderBook.PlaceOrder(buyOrder1);
            limitOrderBook.PlaceOrder(sellOrder1);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            Assert.AreEqual(0, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(0, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            //byte[] array2 = ObjectToByteArray(limitOrderBook);
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            OutputDisruptor.Publish(limitOrderBook);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(200, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(491.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Volume of first ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void NewOrderOnExchangeTest_ChecksWhetherLimitOrderBookGetsReceviedAtTheMemoryImage_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

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

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(100, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(491.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Volume of first ask in the first ask book in the ask books list in memory image");

            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(sellOrder2);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            Assert.AreEqual(2, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(200, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Volume of first ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void SellOrdersMatchTest_ChecksWhetherLimitOrderBookGetsUpdatedAtTheMemoryImageWhenIncomingSellOrderMatches_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

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
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());

            // No matching orders till now
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(2, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Second Bid's volume in first OrderBook
            Assert.AreEqual(100, orderBookMemoryImage.BidBooks.First().ToList()[1].Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // BidsOrderBooks -> First BidOrderBook -> Second Bid's price in first OrderBook
            Assert.AreEqual(491.34M, orderBookMemoryImage.BidBooks.First().ToList()[1].Item2, "Price of first bids in the first bid book in the bids book list in  memory image");

            exchange.PlaceNewOrder(sellOrder2);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            // One order on bid book matched so one the order one lower level comes on top
            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(100, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(491.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void BuyOrdersMatchTest_ChecksWhetherLimitOrderBookGetsUpdatedAtTheMemoryImageWhenIncomingBuyOrderMatches_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });

            // Start exchagne to accept orders
            Exchange exchange = new Exchange();
            Order buyOrder1 = OrderFactory.CreateOrder("1233", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 486.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 496.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());

            // No matching orders till now
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(492.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of second ask in the first ask book in the ask books list in memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of second ask in the first ask book in the ask books list in memory image");

            exchange.PlaceNewOrder(buyOrder2);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            // One order on bid book matched so one the order one lower level comes on top
            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        /// <summary>
        /// Adds 3 Buy and 2 Sell Orders
        /// Cancels the buy with the highest price and sees whether the OrderBook and OrderBook image have been updated
        /// Then again cancels the buy with the highest price and sees everything updated as expected
        /// </summary>
        [Test]
        [Category(Integration)]
        public void CancelBuyOrdersTest_ChecksWhetherLimitOrderBookGetsUpdatedAtTheMemoryImageABuyIsCancelled_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });

            // Start exchagne to accept orders
            Exchange exchange = new Exchange();
            Order buyOrder1 = OrderFactory.CreateOrder("1233", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 600, 486.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 300, 493.34M, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1134", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 494.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 496.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 500, 495.34M, new StubbedOrderIdGenerator());

            OrderId buyOrder2Id = buyOrder2.OrderId;
            OrderId buyOrder3Id = buyOrder3.OrderId;

            // No matching orders till now
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(3, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(400, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(495.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Second Bid's volume in first OrderBook
            Assert.AreEqual(300, orderBookMemoryImage.BidBooks.First().ToList()[1].Item1, "Volume of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of Second ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Second Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().ToList()[1].Item2, "Price of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of Second ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Third Bid's volume in first OrderBook
            Assert.AreEqual(600, orderBookMemoryImage.BidBooks.First().ToList()[2].Item1, "Volume of third bid in the first bid book in the bids book list in  memory image");
            // BidsOrderBooks -> First BidOrderBook -> Third Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().ToList()[2].Item2, "Price of third bid in the first bid book in the bids book list in  memory image");

            exchange.CancelOrder(buyOrder3Id);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            // One order on bid book matched so one the order one lower level comes on top
            Assert.AreEqual(2, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(300, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(495.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Second Bid's volume in first OrderBook
            Assert.AreEqual(600, orderBookMemoryImage.BidBooks.First().ToList()[1].Item1, "Volume of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of Second ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Second Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().ToList()[1].Item2, "Price of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of Second ask in the first ask book in the ask books list in memory image");

            exchange.CancelOrder(buyOrder2Id);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            // One order on bid book matched so one the order one lower level comes on top
            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(600, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(495.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(250, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of Second ask in the first ask book in the ask books list in memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of Second ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category(Integration)]
        public void CancelSellOrdersTest_ChecksWhetherLimitOrderBookGetsUpdatedAtTheMemoryImageWhenASellIsCancelled_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });

            // Start exchagne to accept orders
            Exchange exchange = new Exchange();
            Order buyOrder1 = OrderFactory.CreateOrder("1233", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 600, 486.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 300, 493.34M, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1134", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 494.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 495.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 500, 497.34M, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 300, 496.34M, new StubbedOrderIdGenerator());

            OrderId sellOrder1Id = sellOrder1.OrderId;
            OrderId sellOrder3Id = sellOrder3.OrderId;

            // No matching orders till now
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(buyOrder3);
            exchange.PlaceNewOrder(sellOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder3);

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(3, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(3, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> First Bid's volume in first OrderBook
            Assert.AreEqual(400, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(100, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(495.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Second Bid's volume in first OrderBook
            Assert.AreEqual(300, orderBookMemoryImage.BidBooks.First().ToList()[1].Item1, "Volume of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(300, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of Second ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Second Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().ToList()[1].Item2, "Price of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(496.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of Second ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Third Bid's volume in first OrderBook
            Assert.AreEqual(600, orderBookMemoryImage.BidBooks.First().ToList()[2].Item1, "Volume of Third bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Third Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().ToList()[2].Item1, "Volume of Third ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Third Bid's price in first OrderBook
            Assert.AreEqual(486.34, orderBookMemoryImage.BidBooks.First().ToList()[2].Item2, "Price of Third bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Third Ask's price in first OrderBook
            Assert.AreEqual(497.34, orderBookMemoryImage.AskBooks.First().ToList()[2].Item2, "Price of Third ask in the first ask book in the ask books list in memory image");

            exchange.CancelOrder(sellOrder3Id);

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            Assert.AreEqual(3, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(2, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook ->  Bid's volume in first OrderBook
            Assert.AreEqual(400, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's volume in first OrderBook
            Assert.AreEqual(100, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of first ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> First Bid's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of first bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> First Ask's price in first OrderBook
            Assert.AreEqual(495.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of first ask in the first ask book in the ask books list in memory image");

            // BidsOrderBooks -> First BidOrderBook -> Second Bid's volume in first OrderBook
            Assert.AreEqual(300, orderBookMemoryImage.BidBooks.First().ToList()[1].Item1, "Volume of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().ToList()[1].Item1, "Volume of Second ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Second Bid's price in first OrderBook
            Assert.AreEqual(493.34, orderBookMemoryImage.BidBooks.First().ToList()[1].Item2, "Price of Second bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Second Ask's price in first OrderBook
            Assert.AreEqual(497.34, orderBookMemoryImage.AskBooks.First().ToList()[1].Item2, "Price of Second ask in the first ask book in the ask books list in memory image");

            exchange.CancelOrder(sellOrder1Id);

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            Assert.AreEqual(1, orderBookMemoryImage.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.Count());

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.BidBooks.First().CurrencyPair);
            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemoryImage.AskBooks.First().CurrencyPair);

            Assert.AreEqual(3, orderBookMemoryImage.BidBooks.First().Count(), "Count of the bids in the first bid book in the list of bid books");
            Assert.AreEqual(1, orderBookMemoryImage.AskBooks.First().Count(), "Count of the asks in the first ask book in the list of ask books");

            // BidsOrderBooks -> First BidOrderBook -> Third Bid's volume in first OrderBook
            Assert.AreEqual(400, orderBookMemoryImage.BidBooks.First().First().Item1, "Volume of Third bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Third Ask's volume in first OrderBook
            Assert.AreEqual(500, orderBookMemoryImage.AskBooks.First().First().Item1, "Volume of Third ask in the first ask book in the ask books list in memory image");
            // BidsOrderBooks -> First BidOrderBook -> Third Bid's price in first OrderBook
            Assert.AreEqual(494.34, orderBookMemoryImage.BidBooks.First().First().Item2, "Price of Third bids in the first bid book in the bids book list in  memory image");
            // AsksOrderBooks -> First AskOrderBook -> Third Ask's price in first OrderBook
            Assert.AreEqual(497.34, orderBookMemoryImage.AskBooks.First().First().Item2, "Price of Third ask in the first ask book in the ask books list in memory image");

            OutputDisruptor.ShutDown();
        }

        #endregion Disruptor Tests
    }
}
