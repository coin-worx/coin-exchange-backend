using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    [Serializable]
    public class LimitOrderBookTests
    {
        [SetUp]
        public void Setup()
        {
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
        }

        [TearDown]
        public void TearDown()
        {
            OutputDisruptor.ShutDown();
        }

        #region Sort Tests

        [Test]
        public void BidsOrderListSortTest_ChecksIfTheBidsListIsSortedProperly_ValidatesByComparingEntriesInTheBidsListUsingLoop()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 491.34M, new StubbedOrderIdGenerator());

            Order order3 = OrderFactory.CreateOrder("1244", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 494.34M, new StubbedOrderIdGenerator());
            Order order4 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);
            limitOrderBook.PlaceOrder(order4);

            Assert.AreEqual(4, limitOrderBook.BidCount, "Count of Buy Orders");

            Order[] orders = new Order[4] { order3, order1, order4, order2 };

            for (int i = 0; i < orders.Length; i++)
            {
                Assert.AreEqual(orders[i].Price, limitOrderBook.Bids.ToList()[i].Price, "Order " + i + " of this side's Order Book");
            }

            Assert.AreEqual(494.34M, limitOrderBook.Bids.First().Price.Value, "First element of OrderBook");
            Assert.AreEqual(491.34M, limitOrderBook.Bids.Last().Price.Value, "Last element of OrderBook");
        }

        [Test]
        public void AsksOrderListSortTest_ChecksIfTheAsksListIsSortedProperly_ValidatesByComparingEntriesInTheAsksListUsingLoop()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 491.34M, new StubbedOrderIdGenerator());

            Order order3 = OrderFactory.CreateOrder("1244", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 494.34M, new StubbedOrderIdGenerator());
            Order order4 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);
            limitOrderBook.PlaceOrder(order4);

            Assert.AreEqual(4, limitOrderBook.AskCount, "Count of Sell Orders");

            Order[] orders = new Order[4] { order2, order4, order1, order3 };

            for (int i = 0; i < orders.Length; i++)
            {
                Assert.AreEqual(orders[i].Price, limitOrderBook.Asks.ToList()[i].Price, "Order " + i + " of this side's Order Book");
            }

            Assert.AreEqual(491.34M, limitOrderBook.Asks.First().Price.Value, "First element of OrderBook");
            Assert.AreEqual(494.34M, limitOrderBook.Asks.Last().Price.Value, "Last element of OrderBook");
        }


        [Test]
        public void BidsOrderListSortTest_ChecksIfTheBidsListIsSortedProperly_ValidatesByComparingEntriesInTheBidsList()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 491.34M, new StubbedOrderIdGenerator());

            Order order3 = OrderFactory.CreateOrder("1244", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 494.34M, new StubbedOrderIdGenerator());
            Order order4 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);
            limitOrderBook.PlaceOrder(order4);

            Assert.AreEqual(4, limitOrderBook.BidCount, "Count of Buy Orders");
            Assert.AreEqual(494.34M, limitOrderBook.Bids.First().Price.Value, "First element of Buy Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Bids.ToList()[1].Price.Value, "Second element of Buy Orders list");
            Assert.AreEqual(492.34M, limitOrderBook.Bids.ToList()[2].Price.Value, "Third element of Buy Orders list");
            Assert.AreEqual(491.34M, limitOrderBook.Bids.Last().Price.Value, "Last element of Buy Orders list");
        }

        [Test]
        [Category("Unit")]
        public void AsksOrderListSortTest_ChecksIfTheAsksListIsSortedProperly_ValidatesByComparingEntriesInTheAsksList()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 493.34M, new StubbedOrderIdGenerator());
            Order order2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 491.34M, new StubbedOrderIdGenerator());

            Order order3 = OrderFactory.CreateOrder("1244", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 494.34M, new StubbedOrderIdGenerator());
            Order order4 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 492.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(order1);
            limitOrderBook.PlaceOrder(order2);
            limitOrderBook.PlaceOrder(order3);
            limitOrderBook.PlaceOrder(order4);

            Assert.AreEqual(4, limitOrderBook.AskCount, "Count of Sell Orders");
            Assert.AreEqual(491.34M, limitOrderBook.Asks.First().Price.Value, "First element of Sell Orders list");
            Assert.AreEqual(492.34M, limitOrderBook.Asks.ToList()[1].Price.Value, "Second element of Sell Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Asks.ToList()[2].Price.Value, "Third element of Sell Orders list");
            Assert.AreEqual(494.34M, limitOrderBook.Asks.Last().Price.Value, "Last element of Sell Orders list");
        }

        #endregion Sort Tests

        #region Addition Tests

        [Test]
        [Category("Unit")]
        public void BidsAcceptedTest_VerifiesIftheBidsAreBeingAcceptedAndEventsRaisedProperly_ValidatesTheStateOfTheReceivedOrderToVerify()
        {
            Order buyOrder1 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT, 
                Constants.ORDER_SIDE_BUY, 100, 1251, new StubbedOrderIdGenerator());

            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedOrder = null;
            orderBook.OrderAccepted += delegate(Order order, Price matchedPrice, Volume matchedVolume)
                                           {
                                               returnedOrder = order;
                                               Assert.AreEqual(0, matchedPrice.Value);
                                               Assert.AreEqual(0, matchedVolume.Value);
                                               manualResetEvent.Set();
                                           };

            orderBook.AddOrder(buyOrder1);
            //Thread.Sleep(2000);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(buyOrder1, returnedOrder);
            Assert.AreEqual(OrderState.Accepted, buyOrder1.OrderState);
        }

        [Test]
        [Category("Unit")]
        public void AsksAcceptedTest_VerifiesIftheAsksAreBeingAcceptedAndEventsRaisedProperly_ValidatesTheStateOfTheReceivedOrderToVerify()
        {
            Order sellOrder1 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 1251, new StubbedOrderIdGenerator());

            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedOrder = null;
            orderBook.OrderAccepted += delegate(Order order, Price matchedPrice, Volume matchedVolume)
            {
                returnedOrder = order;
                Assert.AreEqual(0, matchedPrice.Value);
                Assert.AreEqual(0, matchedVolume.Value);
                manualResetEvent.Set();
            };

            orderBook.AddOrder(sellOrder1);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(sellOrder1, returnedOrder);
            Assert.AreEqual(OrderState.Accepted, sellOrder1.OrderState);
        }

        [Test]
        [Category("Unit")]
        public void AddBidAndFillTest_TestsIfTheBidMatchesSuccessfully_ChecksOrderStateAfterEventRaiseToConfirm()
        {
            Order sellOrder1 = OrderFactory.CreateOrder("1234", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 491, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("12345", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 491.5M, new StubbedOrderIdGenerator());

            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");
            orderBook.AddOrder(sellOrder1);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedInboundOrder = null;
            Order returnedMatchedOrder = null;
            FillFlags returnedFillFlags = FillFlags.NetitherFilled;
            Price returnedMatchedPrice = null;
            Volume returnedMatchedVolume = null;
            orderBook.OrderFilled += delegate(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price matchedPrice, Volume matchedVolume)
                                         {
                                             returnedInboundOrder = inboundOrder;
                                             returnedMatchedOrder = matchedOrder;
                                             returnedFillFlags = fillFlags;
                                             returnedMatchedPrice = matchedPrice;
                                             returnedMatchedVolume = matchedVolume;
                                             manualResetEvent.Set();
                                         };

            orderBook.AddOrder(buyOrder1);
            manualResetEvent.WaitOne(4000);
            Assert.AreEqual(returnedInboundOrder, buyOrder1);
            Assert.IsTrue(returnedInboundOrder.OrderState == OrderState.Complete);
            Assert.AreEqual(returnedMatchedOrder, sellOrder1);
            Assert.IsTrue(returnedMatchedOrder.OrderState == OrderState.Complete);
            Assert.AreEqual(FillFlags.BothFilled, returnedFillFlags);
            Assert.AreEqual(sellOrder1.Price, returnedMatchedPrice);
            Assert.AreEqual(sellOrder1.Volume, returnedMatchedVolume);

            Assert.AreEqual(0, returnedInboundOrder.OpenQuantity.Value);
            Assert.AreEqual(0, returnedMatchedOrder.OpenQuantity.Value);
        }

        [Test]
        public void AddAskAndFillTest_TestsIfTheAskMatchesSuccessfully_ChecksOrderStateAfterEventRaiseToConfirm()
        {
            Order sellOrder1 = OrderFactory.CreateOrder("1234", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 491M, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("12345", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 491.5M, new StubbedOrderIdGenerator());

            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");
            orderBook.AddOrder(buyOrder1);
            Assert.AreEqual(1, orderBook.Bids.Count());
            Assert.AreEqual(0, orderBook.Asks.Count());

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedInboundOrder = null;
            Order returnedMatchedOrder = null;
            FillFlags returnedFillFlags = FillFlags.NetitherFilled;
            Price returnedMatchedPrice = null;
            Volume returnedMatchedVolume = null;
            orderBook.OrderFilled += delegate(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price matchedPrice, Volume matchedVolume)
            {
                returnedInboundOrder = inboundOrder;
                returnedMatchedOrder = matchedOrder;
                returnedFillFlags = fillFlags;
                returnedMatchedPrice = matchedPrice;
                returnedMatchedVolume = matchedVolume;
                manualResetEvent.Set();
            };

            orderBook.AddOrder(sellOrder1);
            manualResetEvent.WaitOne(4000);
            Assert.AreEqual(returnedInboundOrder, sellOrder1);
            Assert.IsTrue(returnedInboundOrder.OrderState == OrderState.Complete);
            Assert.AreEqual(returnedMatchedOrder, buyOrder1);
            Assert.IsTrue(returnedMatchedOrder.OrderState == OrderState.Complete);
            Assert.AreEqual(FillFlags.BothFilled, returnedFillFlags);
            Assert.AreEqual(buyOrder1.Price, returnedMatchedPrice);
            Assert.AreEqual(buyOrder1.Volume, returnedMatchedVolume);

            Assert.AreEqual(0, returnedInboundOrder.OpenQuantity.Value);
            Assert.AreEqual(0, returnedMatchedOrder.OpenQuantity.Value);

            Assert.AreEqual(0, orderBook.Bids.Count());
            Assert.AreEqual(0, orderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void MatchBidAndDepthUpdatetest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order sellOrder1 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 1252, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("12345", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 1251, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 1251M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("123457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 1250, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.ExchangeEssentials.First().DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder2.Price, 1, sellOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(250), sellOrder2.Price, new Price(312750M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void MatchAskAndDepthUpdatetest_EntersAskOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 250, 1250, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("123", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 1251, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("12", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 250, 1250, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.ExchangeEssentials.First().DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder2, new Volume(250), buyOrder2.Price, new Price(312500M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(null, 0, null));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void BidMultipleMatchesTest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true));

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.ExchangeEssentials.First().DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder2.Price, 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(500), sellOrder2.Price, new Price(1251 * 500)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void AskMultipleMatchesTest_EntersAskOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder4, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));

            Assert.AreEqual(4, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.ExchangeEssentials.First().DepthOrderBook.Depth);

            // Place bids in descending order and asks in ascending order
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1252), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1251), 2, new Volume(600)));
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1249), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1253), 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1254), 1, new Volume(200)));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder4, true, true));
            depthCheck.Reset();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder4, new Volume(700), buyOrder1.Price, new Price(875800M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1249), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1253), 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1254), 1, new Volume(200)));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        public void BidPartialFillTest_TestsThatABidMatchesAndBecomesPartiallyFilled_ValidatesDepthAndOrderStateAfterPartialFill()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 947, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 942, new StubbedOrderIdGenerator());
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 945, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 200, 940, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 941, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(buyOrder3.Price, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(buyOrder3.Volume, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsFalse(fillCheck.VerifyFilled(buyOrder1, new Volume(500), sellOrder2.Price, new Price(1251 * 500)));
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, buyOrder1.OrderState);
            Assert.AreEqual(100, buyOrder1.OpenQuantity.Value);
            Assert.AreEqual(300, buyOrder1.FilledQuantity.Value);

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(940), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);
        }

        [Test]
        public void AskPartialFillTest_TestsThatAAskMatchesAndBecomesPartiallyFilled_ValidatesDepthAndOrderStateAfterPartialFill()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);
            
            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 945, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 200, 940, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 941, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 947, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 947, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 600, 942, new StubbedOrderIdGenerator());
            
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(940), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(400), buyOrder1.Price, new Price(1251 * 500)));
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, sellOrder3.OrderState);
            Assert.AreEqual(200, sellOrder3.OpenQuantity.Value);
            Assert.AreEqual(400, sellOrder3.FilledQuantity.Value);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        [Test]
        public void MultiplePartialBidMatch_ChecksIfBidMatchesWithMultipleAsksButFillsPartially_ValidatesThroughDepthLevelsAndFillChecks()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 700, 945, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 200, 940, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 941, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 947, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 300, 942, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 943, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(buyOrder3.Price, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(buyOrder3.Volume, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(940), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(943), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[2].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(buyOrder1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.First());
            Assert.AreEqual(sellOrder1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.First());
        }

        [Test]
        public void MultipleMatchedPartialAskFill_ChecksIfAskMatchesWithMultipleAsksButFillsPartially_ValidatesThroughDepthLevelsAndFillChecks()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 945, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 200, 942, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("623456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 400, 941, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 200, 947, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 947, new StubbedOrderIdGenerator());
            Order sellOrder3 = OrderFactory.CreateOrder("12344", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 700, 942, new StubbedOrderIdGenerator());


            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[2].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(600), buyOrder1.Price, new Price(1251 * 500)));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, sellOrder3.OrderState);
            Assert.AreEqual(100, sellOrder3.OpenQuantity.Value);
            Assert.AreEqual(600, sellOrder3.FilledQuantity.Value);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(300), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);
        }

        [Test]
        [Category("Unit")]
        public void MultipleRepeatMatchBidTest_MatchesManyAsksToBidButBidVolumeRemains_VerifiesUsingFillCheckAndDepthLevel()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(900), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(930), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder1, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder2, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder4, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder4, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(500), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(930), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
        }

        [Test]
        [Category("Unit")]
        public void MultipleRepeatMatchAskTest_MatchesManyBidsToAskButAskVolumeRemains_VerifiesUsingFillCheckAndDepthLevel()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(1000), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(951), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder2, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder3, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder4, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder4, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(600), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(951), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        [Test]
        [Category("Unit")]
        public void BidMarketOrderFill_TestsWhetherTheMarketOrderGetFilledCorrectly_ChecksOrderStateAfterFill()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_MARKET,
                Constants.ORDER_SIDE_BUY, 100, 0, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 951, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 942, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(942), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(951), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder2, new Volume(100), new Price(0), new Price(0)));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(OrderState.Complete, buyOrder2.OrderState);
            Assert.AreEqual(0, buyOrder2.OpenQuantity.Value);
            Assert.AreEqual(100, buyOrder2.FilledQuantity.Value);
            Assert.AreEqual(0, buyOrder2.Price.Value);
        }

        [Test]
        public void AskMarketOrderFill_TestsWhetherTheMarketOrderGetFilledCorrectly_ChecksOrderStateAfterFill()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("723457", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_MARKET,
                Constants.ORDER_SIDE_SELL, 100, 0, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder1, new Volume(100), new Price(0), new Price(0)));
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(OrderState.Complete, sellOrder1.OrderState);
            Assert.AreEqual(0, sellOrder1.OpenQuantity.Value);
            Assert.AreEqual(100, sellOrder1.FilledQuantity.Value);
            Assert.AreEqual(0, sellOrder1.Price.Value);
        }

        #endregion Addition Tests

        #region Order Cancel Tests

        [Test]
        public void OrderCancelAskTest_CancelsTheAskAndWaitsForEventToBeRaised_HandlesEventsAndValidatesOrderState()
        {
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 1251, new StubbedOrderIdGenerator());
            OrderId orderId = sellOrder1.OrderId;
            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedOrder = null;
            orderBook.OrderAccepted += delegate(Order order, Price matchedPrice, Volume matchedVolume)
            {
                returnedOrder = order;
                Assert.AreEqual(0, matchedPrice.Value);
                Assert.AreEqual(0, matchedVolume.Value);
                manualResetEvent.Set();
            };

            orderBook.AddOrder(sellOrder1);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(sellOrder1, returnedOrder);
            Assert.AreEqual(OrderState.Accepted, sellOrder1.OrderState);
            Assert.AreEqual(1, orderBook.Asks.Count());

            manualResetEvent.Reset();
            returnedOrder = null;
            orderBook.OrderCancelled += delegate(Order order)
            {
                returnedOrder = order;
                manualResetEvent.Set();
            };

            orderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(sellOrder1, returnedOrder);
            Assert.AreEqual(OrderState.Cancelled, sellOrder1.OrderState);
            Assert.AreEqual(0, orderBook.Asks.Count());
        }

        [Test]
        public void OrderCancelBidTest_CancelsTheBidAndWaitsForEventToBeRaised_HandlesEventsAndValidatesOrderState()
        {
            Order buyOrder = OrderFactory.CreateOrder("1233", "XBT/USD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 1251, new StubbedOrderIdGenerator());
            OrderId orderId = buyOrder.OrderId;

            LimitOrderBook orderBook = new LimitOrderBook("XBT/USD");

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Order returnedOrder = null;
            orderBook.OrderAccepted += delegate(Order order, Price matchedPrice, Volume matchedVolume)
            {
                returnedOrder = order;
                Assert.AreEqual(0, matchedPrice.Value);
                Assert.AreEqual(0, matchedVolume.Value);
                manualResetEvent.Set();
            };

            orderBook.AddOrder(buyOrder);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(buyOrder, returnedOrder);
            Assert.AreEqual(OrderState.Accepted, buyOrder.OrderState);
            Assert.AreEqual(1, orderBook.Bids.Count());

            manualResetEvent.Reset();
            returnedOrder = null;
            orderBook.OrderCancelled += delegate(Order order)
            {
                returnedOrder = order;
                manualResetEvent.Set();
            };

            orderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(buyOrder, returnedOrder);
            Assert.AreEqual(OrderState.Cancelled, buyOrder.OrderState);
            Assert.AreEqual(0, orderBook.Bids.Count());
        }

        [Test]
        public void BidCancelOrderTest_CancelsOrderAndUpdatesOrderBookAndDepth_ValidatesEntriesAndOrderStateToConfirm()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            OrderId orderId = buyOrder2.OrderId;
            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            
            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(OrderState.Cancelled, buyOrder2.OrderState);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        [Test]
        public void AskCancelOrderTest_CancelsOrderAndUpdatesOrderBookAndDepth_ValidatesEntriesAndOrderStateToConfirm()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            
            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());
            OrderId orderId = sellOrder2.OrderId;

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(OrderState.Cancelled, sellOrder2.OrderState);
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(null, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        [Test]
        [Category("Unit")]
        public void AskCancelThenBidMatchTest_TestsWhetherTheOrderIsCancelledAndThenBidOrderisMatched_VerifiesThroughBooksLists()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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
            
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

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

            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(sellOrder1.OrderId);

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void BidCancelThenAskMatchTest_TestsWhetherTheOrderIsCancelledAndThenBidOrderisMatched_VerifiesThroughBooksLists()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

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

            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(buyOrder1.OrderId);

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true, true));

            Assert.AreEqual(0, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        [Category("Unit")]
        public void BidCancelFailTest_ChekcsIfMatchedOrderIsNotCancelled_VerifiesUsingBooksLists()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 948, new StubbedOrderIdGenerator());

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder3, true, true));

            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Asks
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);

            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(buyOrder3.OrderId);
            Assert.IsFalse(cancelOrderResponse);

            // All bids and Asks are the same so no cancel got executed
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(945), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[1].OrderCount);

            // Asks
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
        }

        [Test]
        [Category("Unit")]
        public void AskCancelFailTest_ChekcsIfMatchedOrderIsNotCancelled_VerifiesUsingBooksLists()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

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

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));
            Assert.IsTrue(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder3, true, true));

            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);
            
            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);

            bool cancelOrderResponse = exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(sellOrder3.OrderId);
            Assert.IsFalse(cancelOrderResponse);

            // All bids and Asks are the same so no cancel got executed
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());

            // Bids
            Assert.AreEqual(new Price(941), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.BidLevels[0].OrderCount);

            // Asks
            Assert.AreEqual(new Price(948), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[0].OrderCount);
            Assert.AreEqual(new Price(949), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        #endregion Order Cancel Tests

        #region Rejection Tests

        [Test]
        [Category("Unit")]
        public void BuyMarketOrderReject_CHecksIfOrderGetsRejectedWhenNoLimitOrdersAreOnBook_VerifiesThroughOrdersState()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_MARKET,
                Constants.ORDER_SIDE_BUY, 100, 0, new StubbedOrderIdGenerator());

            bool matched = exchange.PlaceNewOrder(buyOrder1);
            Assert.IsFalse(matched);
            Assert.AreEqual(OrderState.Rejected, buyOrder1.OrderState);
        }

        [Test]
        [Category("Unit")]
        public void SellMarketOrderReject_CHecksIfOrderGetsRejectedWhenNoLimitOrdersAreOnBook_VerifiesThroughOrdersState()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order sellOrder1 = OrderFactory.CreateOrder("123456", "BTCUSD", Constants.ORDER_TYPE_MARKET,
                Constants.ORDER_SIDE_SELL, 100, 0, new StubbedOrderIdGenerator());

            bool matched = exchange.PlaceNewOrder(sellOrder1);
            Assert.IsFalse(matched);
            Assert.AreEqual(OrderState.Rejected, sellOrder1.OrderState);
        }

        #endregion Rejection Tests

        #region Order Book Changed Tests

        [Test]
        public void OrderBookChangedDepthEventTest_DepthOrderBookRaisesEventForDepthWhenOrderBookChanges_EventIsHandled()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            OrderId orderId = buyOrder2.OrderId;
            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(2000);

            Assert.AreEqual(OrderState.Cancelled, buyOrder2.OrderState);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        [Test]
        public void OrderBookChangedBBoEventTest_DepthOrderBookRaisesEventForDepthWhenOrderBookChanges_EventIsHandled()
        {
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 941, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 100, 945, new StubbedOrderIdGenerator());
            OrderId orderId = buyOrder2.OrderId;

            Order sellOrder1 = OrderFactory.CreateOrder("1244", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 949, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1222", "BTCUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 100, 948, new StubbedOrderIdGenerator());

            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.ExchangeEssentials.First().LimitOrderBook, sellOrder1, true));

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);            
            
            exchange.ExchangeEssentials.First().LimitOrderBook.CancelOrder(orderId);
            manualResetEvent.WaitOne(2000);
            Assert.AreEqual(OrderState.Cancelled, buyOrder2.OrderState);
            Assert.AreEqual(1, exchange.ExchangeEssentials.First().LimitOrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.ExchangeEssentials.First().LimitOrderBook.Asks.Count());
        }

        #endregion Order Book Changed Tests

        #region Helper Methods

        /// <summary>
        /// Adds an Order and verifies if the order has been added and wheterh or not a match has occurred
        /// </summary>
        /// <param name="orderBook"></param>
        /// <param name="order"></param>
        /// <param name="matchExpected"></param>
        /// <param name="completeExpected"></param>
        /// <returns></returns>
        private bool AddAndVerify(LimitOrderBook orderBook, Order order, bool matchExpected, bool completeExpected = false)
        {
            bool matched = orderBook.AddOrder(order);

            if (matched == matchExpected)
            {
                if (completeExpected)
                {
                    return order.OrderState == OrderState.Complete;
                }
                else
                {
                    return order.OrderState == OrderState.Accepted;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if Order was partially or fully filled
        /// </summary>
        /// <param name="orderBook"></param>
        /// <param name="order"></param>
        /// <param name="isPartiallyFilled"> </param>
        /// <returns></returns>
        private bool FilledPartiallyOrComplete(LimitOrderBook orderBook, Order order, bool isPartiallyFilled)
        {
            bool matched = orderBook.AddOrder(order);

            if (isPartiallyFilled)
            {
                return order.OrderState == OrderState.PartiallyFilled;
            }
            else
            {
                return matched && order.OrderState == OrderState.Complete;
            }
        }

        #endregion Helper Methods
    }
}
