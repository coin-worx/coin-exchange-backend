using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    public class LimitOrderBookTests
    {
        #region Sort Tests

        [Test]
        public void BidsOrderListSortTest_ChecksIfTheBidsListIsSortedProperly_ValidatesByComparingEntriesInTheBidsListUsingLoop()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order order1 = new Order(new OrderId(1), "XBTUSD", new Price(493.34M), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), 
                      new TraderId(1));
            Order order2 = new Order(new OrderId(1), "XBTUSD", new Price(491.34M), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order order3 = new Order(new OrderId(1), "XBTUSD", new Price(494.34M), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order order4 = new Order(new OrderId(1), "XBTUSD", new Price(492.34M), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));

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

            Order order1 = new Order(new OrderId(1), "XBTUSD", new Price(493.34M), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order order2 = new Order(new OrderId(1), "XBTUSD", new Price(491.34M), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order order3 = new Order(new OrderId(1), "XBTUSD", new Price(494.34M), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order order4 = new Order(new OrderId(1), "XBTUSD", new Price(492.34M), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));

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

            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(493.34M), OrderSide.Buy,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(491.34M), OrderSide.Buy,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(494.34M), OrderSide.Buy,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(492.34M), OrderSide.Buy,
                OrderType.Limit, new Volume(250), new TraderId(1)));

            Assert.AreEqual(4, limitOrderBook.BidCount, "Count of Buy Orders");
            Assert.AreEqual(494.34M, limitOrderBook.Bids.First().Price.Value, "First element of Buy Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Bids.ToList()[1].Price.Value, "Second element of Buy Orders list");
            Assert.AreEqual(492.34M, limitOrderBook.Bids.ToList()[2].Price.Value, "Third element of Buy Orders list");
            Assert.AreEqual(491.34M, limitOrderBook.Bids.Last().Price.Value, "Last element of Buy Orders list");
        }

        [Test]
        public void AsksOrderListSortTest_ChecksIfTheAsksListIsSortedProperly_ValidatesByComparingEntriesInTheAsksList()
        {
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(493.34M), OrderSide.Sell,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(491.34M), OrderSide.Sell,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(494.34M), OrderSide.Sell,
                OrderType.Limit, new Volume(250), new TraderId(1)));
            limitOrderBook.PlaceOrder(new Order(new OrderId(1), "XBTUSD", new Price(492.34M), OrderSide.Sell,
                OrderType.Limit, new Volume(250), new TraderId(1)));

            Assert.AreEqual(4, limitOrderBook.AskCount, "Count of Sell Orders");
            Assert.AreEqual(491.34M, limitOrderBook.Asks.First().Price.Value, "First element of Sell Orders list");
            Assert.AreEqual(492.34M, limitOrderBook.Asks.ToList()[1].Price.Value, "Second element of Sell Orders list");
            Assert.AreEqual(493.34M, limitOrderBook.Asks.ToList()[2].Price.Value, "Third element of Sell Orders list");
            Assert.AreEqual(494.34M, limitOrderBook.Asks.Last().Price.Value, "Last element of Sell Orders list");
        }

        #endregion Sort Tests

        #region Addition Tests

        [Test]
        public void BidsAcceptedTest_VerifiesIftheBidsAreBeingAcceptedAndEventsRaisedProperly_ValidatesTheStateOfTheReceivedOrderToVerify()
        {
            Order buyOrder1 = new Order(new OrderId(1), "XBTUSD", new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));

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
        public void AsksAcceptedTest_VerifiesIftheAsksAreBeingAcceptedAndEventsRaisedProperly_ValidatesTheStateOfTheReceivedOrderToVerify()
        {
            Order sellOrder1 = new Order(new OrderId(1), "XBT/USD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));

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
        public void AddBidAndFillTest_TestsIfTheBidMatchesSuccessfully_ChecksOrderStateAfterEventRaiseToConfirm()
        {
            Order sellOrder1 = new Order(new OrderId(1), "XBT/USD", new Price(491), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder1 = new Order(new OrderId(1), "XBT/USD", new Price(491.5M), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));

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
            Order buyOrder1 = new Order(new OrderId(1), "XBT/USD", new Price(491.5M), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "XBT/USD", new Price(491M), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));

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
        public void MatchBidAndDepthUpdatetest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder2.Price, 1, sellOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.OrderBook, buyOrder1, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(250), sellOrder2.Price, new Price(312750M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());
        }

        [Test]
        public void MatchAskAndDepthUpdatetest_EntersAskOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            Exchange exchange = new Exchange();

            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1250), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.OrderBook, sellOrder2, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder2, new Volume(250), buyOrder2.Price, new Price(312500M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(null, 0, null));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());
        }


        [Test]
        public void BidMultipleMatchesTest_EntersBidOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(500), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1250), OrderSide.Buy,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1252), OrderSide.Sell,
                      OrderType.Limit, new Volume(250), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder3, true));

            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.DepthOrderBook.Depth);

            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder2.Price, 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.OrderBook, buyOrder1, true, true));
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(500), sellOrder2.Price, new Price(1251 * 500)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(buyOrder2.Price, 1, buyOrder2.Volume));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());
        }

        [Test]
        public void AskMultipleMatchesTest_EntersAskOrdertoMatchAndUpdatesDepth_ValidatesDepthLevelsIfTheyPerformAsExpected()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(500), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1252), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder4 = new Order(new OrderId(1), "BTCUSD", new Price(1249), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));

            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(1254), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(1253), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(1253), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder4 = new Order(new OrderId(1), "BTCUSD", new Price(1251), OrderSide.Sell,
                      OrderType.Limit, new Volume(700), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder4, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));

            Assert.AreEqual(4, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            DepthCheck depthCheck = new DepthCheck(exchange.DepthOrderBook.Depth);

            // Place bids in descending order and asks in ascending order
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1252), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1251), 2, new Volume(600)));
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1249), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1253), 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1254), 1, new Volume(200)));

            FillCheck fillCheck = new FillCheck();

            Assert.IsTrue(AddAndVerify(exchange.OrderBook, sellOrder4, true, true));
            depthCheck.Reset();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder4, new Volume(700), buyOrder1.Price, new Price(875800M)));

            depthCheck.Reset();
            Assert.IsTrue(depthCheck.VerifyBid(new Price(1249), 1, new Volume(100)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1253), 2, new Volume(500)));
            Assert.IsTrue(depthCheck.VerifyAsk(new Price(1254), 1, new Volume(200)));
            Assert.IsTrue(depthCheck.VerifyAsk(sellOrder1.Price, 1, sellOrder1.Volume));
            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());
        }

        [Test]
        public void BidPartialFillTest_TestsThatABidMatchesAndBecomesPartiallyFilled_ValidatesDepthAndOrderStateAfterPartialFill()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(945), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(940), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder3, true));

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(buyOrder3.Price, exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(buyOrder3.Volume, exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder1, true, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsFalse(fillCheck.VerifyFilled(buyOrder1, new Volume(500), sellOrder2.Price, new Price(1251 * 500)));
            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, buyOrder1.OrderState);
            Assert.AreEqual(100, buyOrder1.OpenQuantity.Value);
            Assert.AreEqual(300, buyOrder1.FilledQuantity.Value);

            Assert.AreEqual(new Price(945), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(940), exchange.DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[2].OrderCount);
        }

        [Test]
        public void AskPartialFillTest_TestsThatAAskMatchesAndBecomesPartiallyFilled_ValidatesDepthAndOrderStateAfterPartialFill()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(945), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(940), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(600), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(941), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(940), exchange.DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[2].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder3, true, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(400), buyOrder1.Price, new Price(1251 * 500)));
            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, sellOrder3.OrderState);
            Assert.AreEqual(200, sellOrder3.OpenQuantity.Value);
            Assert.AreEqual(400, sellOrder3.FilledQuantity.Value);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(2, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        [Test]
        public void MultiplePartialBidMatch_ChecksIfBidMatchesWithMultipleAsksButFillsPartially_ValidatesThroughDepthLevelsAndFillChecks()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(945), OrderSide.Buy,
                      OrderType.Limit, new Volume(700), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(940), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(300), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(943), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder3, true));

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(buyOrder3.Price, exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(buyOrder3.Volume, exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(940), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(943), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[2].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.AskLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[2].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, buyOrder1, true));

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(1, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(buyOrder1, exchange.OrderBook.Bids.First());
            Assert.AreEqual(sellOrder1, exchange.OrderBook.Asks.First());
        }

        [Test]
        public void MultipleMatchedPartialAskFill_ChecksIfAskMatchesWithMultipleAsksButFillsPartially_ValidatesThroughDepthLevelsAndFillChecks()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(945), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(941), OrderSide.Buy,
                      OrderType.Limit, new Volume(400), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(947), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(700), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder3, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));

            Assert.AreEqual(3, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(new Price(945), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(new Price(941), exchange.DepthOrderBook.Depth.BidLevels[2].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[2].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[2].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(2, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, sellOrder3, true));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(600), buyOrder1.Price, new Price(1251 * 500)));
            Assert.AreEqual(1, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(3, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(OrderState.PartiallyFilled, sellOrder3.OrderState);
            Assert.AreEqual(100, sellOrder3.OpenQuantity.Value);
            Assert.AreEqual(600, sellOrder3.FilledQuantity.Value);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(100), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(947), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(300), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(2, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.AreEqual(new Price(941), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(400), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(0, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);
        }

        [Test]
        public void MultipleRepeatMatchBidTest_MatchesManyAsksToBidButBidVolumeRemains_VerifiesUsingFillCheckAndDepthLevel()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(930), OrderSide.Buy,
                      OrderType.Limit, new Volume(200), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(900), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder4 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, buyOrder2, true));

            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(900), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(930), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, sellOrder1, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder1, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, sellOrder2, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder2, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, sellOrder3, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder3, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, sellOrder4, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(sellOrder4, new Volume(100), buyOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(2, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(0, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(new Volume(500), exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(930), exchange.DepthOrderBook.Depth.BidLevels[1].Price);
            Assert.AreEqual(new Volume(200), exchange.DepthOrderBook.Depth.BidLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.BidLevels[1].OrderCount);

            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);
        }

        [Test]
        public void MultipleRepeatMatchAskTest_MatchesManyBidsToAskButAskVolumeRemains_VerifiesUsingFillCheckAndDepthLevel()
        {
            Exchange exchange = new Exchange();

            Order buyOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder3 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order buyOrder4 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Buy,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder1 = new Order(new OrderId(1), "BTCUSD", new Price(951), OrderSide.Sell,
                      OrderType.Limit, new Volume(100), new TraderId(1));
            Order sellOrder2 = new Order(new OrderId(1), "BTCUSD", new Price(942), OrderSide.Sell,
                      OrderType.Limit, new Volume(1000), new TraderId(1));

            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder1, true));
            Assert.IsFalse(AddAndVerify(exchange.OrderBook, sellOrder2, true));

            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(1000), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(951), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, buyOrder1, false));

            FillCheck fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder1, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, buyOrder2, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder2, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, buyOrder3, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder3, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.IsTrue(FilledPartiallyOrComplete(exchange.OrderBook, buyOrder4, false));

            fillCheck = new FillCheck();
            Assert.IsTrue(fillCheck.VerifyFilled(buyOrder4, new Volume(100), sellOrder2.Price, new Price(942 * 100)));
            Assert.AreEqual(0, exchange.OrderBook.Bids.Count());
            Assert.AreEqual(2, exchange.OrderBook.Asks.Count());

            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[0].Price);
            Assert.AreEqual(null, exchange.DepthOrderBook.Depth.BidLevels[0].AggregatedVolume);
            Assert.AreEqual(0, exchange.DepthOrderBook.Depth.BidLevels[0].OrderCount);

            Assert.AreEqual(new Price(942), exchange.DepthOrderBook.Depth.AskLevels[0].Price);
            Assert.AreEqual(new Volume(600), exchange.DepthOrderBook.Depth.AskLevels[0].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[0].OrderCount);

            Assert.AreEqual(new Price(951), exchange.DepthOrderBook.Depth.AskLevels[1].Price);
            Assert.AreEqual(new Volume(100), exchange.DepthOrderBook.Depth.AskLevels[1].AggregatedVolume);
            Assert.AreEqual(1, exchange.DepthOrderBook.Depth.AskLevels[1].OrderCount);
        }

        #endregion Addition Tests

        #region Order Book Changed Tests

        [Test]
        public void OrderBookChangedDepthEventTest_DepthOrderBookRaisesEventForDepthWhenOrderBookChanges_EventIsHandled()
        {
            Exchange exchange = new Exchange();
            

        }

        #endregion Order Book Changed Tests

        #region Helper Methods

        /// <summary>
        /// Adds an Order and verifies if 
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
