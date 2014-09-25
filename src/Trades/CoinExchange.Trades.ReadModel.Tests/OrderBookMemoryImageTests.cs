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
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order buyOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("1"));
            Order buyOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("2"));
            Order buyOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1252), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("3"));
            Order buyOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("4"));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of the first book in the" +
                                       " memory image's list of BidOrderBook representations");
            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");
        }

        [Test]
        [Category(Unit)]
        public void AddAskOrderBookToMemoryImage_ChecksWhetherOrderBooksGetAddedToImageListsPrperly_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order sellOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("1"));
            Order sellOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("2"));
            Order sellOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("3"));
            Order sellOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("4"));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.AskBooks.First().CurrencyPair, "Currency pair of the first book in the" +
                                       " memory image's list of AskOrderBook representations");
            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");
        }

        [Test]
        [Category(Unit)]
        public void CheckBidBookPrices_ChecksWhetherOrderOfBidsPricesIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order buyOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("1"));
            Order buyOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("2"));
            Order buyOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1252), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("3"));
            Order buyOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId("4"));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(1252, orderBookMemory.BidBooks.First().ToList()[0].Price);
            Assert.AreEqual(1251, orderBookMemory.BidBooks.First().ToList()[1].Price);
            Assert.AreEqual(1251, orderBookMemory.BidBooks.First().ToList()[2].Price);
            Assert.AreEqual(1250, orderBookMemory.BidBooks.First().ToList()[3].Price);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckAskBookPrices_ChecksWhetherOrderOfAsksPricesIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order sellOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("1"));
            Order sellOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("2"));
            Order sellOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("3"));
            Order sellOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId("4"));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(1250, orderBookMemory.AskBooks.First().ToList()[0].Price);
            Assert.AreEqual(1251, orderBookMemory.AskBooks.First().ToList()[1].Price);
            Assert.AreEqual(1251, orderBookMemory.AskBooks.First().ToList()[2].Price);
            Assert.AreEqual(1252, orderBookMemory.AskBooks.First().ToList()[3].Price);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckBidBookVolumes_ChecksWhetherOrderOfBidsVolumeIsSortedAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order buyOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId("1"));
            Order buyOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1253), OrderSide.Buy,
                      OrderType.Limit, new Volume(300), new TraderId("2"));
            Order buyOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1257), OrderSide.Buy,
                      OrderType.Limit, new Volume(700), new TraderId("3"));
            Order buyOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1256), OrderSide.Buy,
                      OrderType.Limit, new Volume(600), new TraderId("4"));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(700, orderBookMemory.BidBooks.First().ToList()[0].Volume);
            Assert.AreEqual(600, orderBookMemory.BidBooks.First().ToList()[1].Volume);
            Assert.AreEqual(300, orderBookMemory.BidBooks.First().ToList()[2].Volume);
            Assert.AreEqual(100, orderBookMemory.BidBooks.First().ToList()[3].Volume);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());
        }

        [Test]
        [Category(Unit)]
        public void CheckAskBookVolumes_ChecksWhetherOrderOfAsksPricesIsSameAsTheLimitOrderBook_VerifiesImageListsToConfirm()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);
            Order sellOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId("1"));
            Order sellOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId("2"));
            Order sellOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId("3"));
            Order sellOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(1253), OrderSide.Sell,
                      OrderType.Limit, new Volume(400), new TraderId("4"));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.AskBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(200, orderBookMemory.AskBooks.First().ToList()[0].Volume);
            Assert.AreEqual(100, orderBookMemory.AskBooks.First().ToList()[1].Volume);
            Assert.AreEqual(300, orderBookMemory.AskBooks.First().ToList()[2].Volume);
            Assert.AreEqual(400, orderBookMemory.AskBooks.First().ToList()[3].Volume);

            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());
        }

        [Test]
        [Category("Unit")]
        public void BuyOrdersPartiallyFilledTest_ChecksIfFilledOrdersAreRepresentedCorrectlyInBidBooksVolumes_VerifiesThroughBidBookOutput()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);

            Order buyOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId("1"));
            Order buyOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId("2"));
            Order buyOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(943), OrderSide.Buy,
                      OrderType.Limit, new Volume(600), new TraderId("3"));
            Order buyOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(944), OrderSide.Buy,
                      OrderType.Limit, new Volume(800), new TraderId("4"));

            Order sellOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(941), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId("5"));
            Order sellOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId("6"));
            Order sellOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(943), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId("7"));
            Order sellOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(944), OrderSide.Sell,
                      OrderType.Limit, new Volume(400), new TraderId("8"));

            orderBook.AddOrder(sellOrder1);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder4);
            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);

            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            // Both books contain one currency pair
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            // Each currency pair contains several entries for the orders
            Assert.AreEqual(4, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(0, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Prices elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(400, orderBookMemory.BidBooks.First().ToList()[0].Volume);
            Assert.AreEqual(300, orderBookMemory.BidBooks.First().ToList()[1].Volume);
            Assert.AreEqual(200, orderBookMemory.BidBooks.First().ToList()[2].Volume);
            Assert.AreEqual(100, orderBookMemory.BidBooks.First().ToList()[3].Volume);

            Assert.AreEqual(944, orderBookMemory.BidBooks.First().ToList()[0].Price);
            Assert.AreEqual(943, orderBookMemory.BidBooks.First().ToList()[1].Price);
            Assert.AreEqual(942, orderBookMemory.BidBooks.First().ToList()[2].Price);
            Assert.AreEqual(941, orderBookMemory.BidBooks.First().ToList()[3].Price);
        }

        [Test]
        [Category("Unit")]
        public void SellOrdersPartiallyFilledTest_ChecksIfFilledOrdersAreRepresentedCorrectlyInAskBooksVolumes_VerifiesThroughAskBookOutput()
        {
            LimitOrderBook orderBook = new LimitOrderBook(CurrencyConstants.BtcUsd);

            Order buyOrder1 = new Order(new OrderId("1"), CurrencyConstants.BtcUsd, new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId("1"));
            Order buyOrder2 = new Order(new OrderId("2"), CurrencyConstants.BtcUsd, new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId("2"));
            Order buyOrder3 = new Order(new OrderId("3"), CurrencyConstants.BtcUsd, new Price(943), OrderSide.Buy,
                      OrderType.Limit, new Volume(300), new TraderId("3"));
            Order buyOrder4 = new Order(new OrderId("4"), CurrencyConstants.BtcUsd, new Price(944), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId("4"));

            Order sellOrder1 = new Order(new OrderId("5"), CurrencyConstants.BtcUsd, new Price(941), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId("5"));
            Order sellOrder2 = new Order(new OrderId("6"), CurrencyConstants.BtcUsd, new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(400), new TraderId("6"));
            Order sellOrder3 = new Order(new OrderId("7"), CurrencyConstants.BtcUsd, new Price(943), OrderSide.Sell,
                      OrderType.Limit, new Volume(600), new TraderId("7"));
            Order sellOrder4 = new Order(new OrderId("8"), CurrencyConstants.BtcUsd, new Price(944), OrderSide.Sell,
                      OrderType.Limit, new Volume(800), new TraderId("8"));

            orderBook.AddOrder(buyOrder1);
            orderBook.AddOrder(buyOrder2);
            orderBook.AddOrder(buyOrder3);
            orderBook.AddOrder(buyOrder4);
            orderBook.AddOrder(sellOrder4);
            orderBook.AddOrder(sellOrder3);
            orderBook.AddOrder(sellOrder2);
            orderBook.AddOrder(sellOrder1);
            
            OrderBookMemoryImage orderBookMemory = new OrderBookMemoryImage();
            orderBookMemory.OnOrderBookChanged(orderBook);

            Assert.AreEqual(CurrencyConstants.BtcUsd, orderBookMemory.BidBooks.First().CurrencyPair, "Currency pair of " +
                            "the first book in the memory image's list of BidOrderBook representations");
            // Both books contain one currency pair
            Assert.AreEqual(1, orderBookMemory.BidBooks.Count());
            Assert.AreEqual(1, orderBookMemory.AskBooks.Count());

            // Each currency pair contains several entries for the orders
            Assert.AreEqual(0, orderBookMemory.BidBooks.First().Count(), "Count of the Bids in the first Bid Book present in the memory image");
            Assert.AreEqual(4, orderBookMemory.AskBooks.First().Count(), "Count of the Asks in te first Ask Book present in the memory image");

            // Check the Volume elements of the first order book in the Memory Image's list of Order representations
            Assert.AreEqual(100, orderBookMemory.AskBooks.First().ToList()[0].Volume);
            Assert.AreEqual(200, orderBookMemory.AskBooks.First().ToList()[1].Volume);
            Assert.AreEqual(300, orderBookMemory.AskBooks.First().ToList()[2].Volume);
            Assert.AreEqual(400, orderBookMemory.AskBooks.First().ToList()[3].Volume);

            // Check Prices
            Assert.AreEqual(941, orderBookMemory.AskBooks.First().ToList()[0].Price);
            Assert.AreEqual(942, orderBookMemory.AskBooks.First().ToList()[1].Price);
            Assert.AreEqual(943, orderBookMemory.AskBooks.First().ToList()[2].Price);
            Assert.AreEqual(944, orderBookMemory.AskBooks.First().ToList()[3].Price);
        }

        #endregion Object to simplified representation Conversion Tests
    }
}
