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
using CoinExchange.Trades.ReadModel.MemoryImages;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.ReadModel.Tests
{
    [TestFixture]
    public class OrderBookMemoryImageTests
    {
        private const string Unit= "Unit";

        #region Object to simplified representation Conversion Tests

        [Test]
        [Category(Unit)]
        public void InitializeOrderBookRepresentationForACurrency_ChecksWhetherTheOrderRepresentationBooksInitializeForCurrencies_VerifiesUsingMemoryImage()
        {
            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            // At initialization, one OrderRepresentationBook gets initialized for bid and ask each
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count(), "Count of the Bid Books present in the memory image");
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count(), "Count of the Ask Books present in the memory image");
        }

        [Test]
        [Category(Unit)]
        public void AddBuyOrderBookToMemoryImage_ChecksWhetherOrderBooksGetAddedToImageListsPrperly_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order buyOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1252), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of the first book in the" +
                                       " memory image's list of BidOrderBook representations");
            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");
        }

        [Test]
        [Category(Unit)]
        public void AddAskOrderBookToMemoryImage_ChecksWhetherOrderBooksGetAddedToImageListsPrperly_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order sellOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.AskBooks.First().CurrencyPair, "Currency pair of the first book in the" +
                                       " memory image's list of AskOrderBook representations");
            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");
        }

        [Test]
        [Category(Unit)]
        public void CheckBidBookPrices_ChecksWhetherOrderOfBidsPricesIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order buyOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1252), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(1252, orderBookMemory.BidBooks.First().ToList()[0].Item2);
            Assert.AreEqual(1251, orderBookMemory.BidBooks.First().ToList()[1].Item2);
            Assert.AreEqual(1251, orderBookMemory.BidBooks.First().ToList()[2].Item2);
            Assert.AreEqual(1250, orderBookMemory.BidBooks.First().ToList()[3].Item2);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckAskBookPrices_ChecksWhetherOrderOfAsksPricesIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order sellOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());

            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(1250, orderBookMemory.AskBooks.First().ToList()[0].Item2);
            Assert.AreEqual(1251, orderBookMemory.AskBooks.First().ToList()[1].Item2);
            Assert.AreEqual(1251, orderBookMemory.AskBooks.First().ToList()[2].Item2);
            Assert.AreEqual(1252, orderBookMemory.AskBooks.First().ToList()[3].Item2);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckBidBookVolumes_ChecksWhetherOrderOfBidsVolumeIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order buyOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1253), OrderSide.Buy,
                      OrderType.Limit, new Volume(300), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1257), OrderSide.Buy,
                      OrderType.Limit, new Volume(700), new TraderId(1));
            Order buyOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1256), OrderSide.Buy,
                      OrderType.Limit, new Volume(600), new TraderId(1));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());

            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(700, orderBookMemory.BidBooks.First().ToList()[0].Item1);
            Assert.AreEqual(600, orderBookMemory.BidBooks.First().ToList()[1].Item1);
            Assert.AreEqual(300, orderBookMemory.BidBooks.First().ToList()[2].Item1);
            Assert.AreEqual(100, orderBookMemory.BidBooks.First().ToList()[3].Item1);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckAskBookVolumes_ChecksWhetherOrderOfAsksPricesIsSameAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BitCoinUsd);
            Order sellOrder1 = new Order(new OrderId(1), CurrencyConstants.BitCoinUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(2), CurrencyConstants.BitCoinUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(3), CurrencyConstants.BitCoinUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId(1));
            Order sellOrder4 = new Order(new OrderId(4), CurrencyConstants.BitCoinUsd, new Price(1253), OrderSide.Sell,
                      OrderType.Limit, new Volume(400), new TraderId(1));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BitCoinUsd, orderBookMemory.AskBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());

            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(200, orderBookMemory.AskBooks.First().ToList()[0].Item1);
            Assert.AreEqual(100, orderBookMemory.AskBooks.First().ToList()[1].Item1);
            Assert.AreEqual(300, orderBookMemory.AskBooks.First().ToList()[2].Item1);
            Assert.AreEqual(400, orderBookMemory.AskBooks.First().ToList()[3].Item1);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
        }

        #endregion Object to simplified representation Conversion Tests

        #region Disruptor Tests

        [Test]
        [Category(Unit)]
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
        }

        [Test]
        public void NewOrderOnExchangeTest_ChecksWhetherLimitOrderBookGetsReceviedAtTheMemoryImage_VerifiesThroughTheListsInOrderBookMemoryImage()
        {
            // Initialize memory image
            OrderBookMemoryImage orderBookMemoryImage = new OrderBookMemoryImage();

            // Start excahgne to accept orders
            Exchange exchange = new Exchange();
            Order buyOrder1 = OrderFactory.CreateOrder("1233", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 491.34M, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 494.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", CurrencyConstants.BitCoinUsd, Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());
            exchange.PlaceNewOrder(buyOrder1);
            exchange.PlaceNewOrder(buyOrder2);
            exchange.PlaceNewOrder(sellOrder1);
            exchange.PlaceNewOrder(sellOrder2);

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.FilledPartiallyOrComplete(exchange.OrderBook, sellOrder1, false));
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder1, new Volume(100), new Price(0), new Price(0)));
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { orderBookMemoryImage });
            manualResetEvent.WaitOne(4000);
        }

        #endregion Disruptor Tests
    }
}
