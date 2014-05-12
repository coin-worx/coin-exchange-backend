using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests.Performance
{
    [TestFixture]
    internal class ListAndMapsTests
    {
        private const string PerformanceTest = "Performance";

        [Test]
        [Category(PerformanceTest)]
        public void TestSortedListVsList_WhichOneIsFaster()
        {
            List<Order> orders = new List<Order>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                Order order = new Order(new OrderId("1"), "XBTUSD", new Price(491 + i), OrderSide.Sell, OrderType.Limit,
                                        new Volume(2000),
                                        new TraderId("1"));
                orders.Add(order);
                /*orders = orders.OrderBy(x => x.Price.Value).ToList();*/
                orders.Sort((x, y) => x.Price.Value.CompareTo(y.Price.Value));
            }

            stopwatch.Stop();

            TimeSpan timeSpan = stopwatch.Elapsed;

            Console.WriteLine("Time taken for list:" + timeSpan);

            stopwatch.Reset();

            timeSpan = stopwatch.Elapsed;

            Console.WriteLine("Watch reset:" + timeSpan);

            SortedList<decimal, Order> _sortedList = new SortedList<decimal, Order>();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                Order order = new Order(new OrderId("1"), "XBTUSD", new Price(491 + i), OrderSide.Sell, OrderType.Limit,
                                        new Volume(2000),
                                        new TraderId("1"));
                _sortedList.Add(i, order);
            }
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;

            Console.WriteLine("Time taken for SortedList:" + timeSpan);
        }

        /// <summary>
        /// tests whether LINQ or For is fast
        /// </summary>
        [Test]
        [Category(PerformanceTest)]
        public void LinqVsFor()
        {
            List<OrderId> orderIds = new List<OrderId>();
            Order[] orders = new Order[10000];
            Random random = new Random();
            for (int i = 0; i < orders.Length; i++)
            {
                bool isBuy = ((i % 2) == 0);
                decimal delta = isBuy ? 1880 : 1884;

                Price price = new Price(random.Next(1, 10) + delta);

                Volume volume = new Volume(random.Next() % 10 + 1 * 100);

                OrderId orderId = new OrderId("i");
                orderIds.Add(orderId);
                orders[i] = new Order(orderId, "BTCUSD", price, isBuy ? OrderSide.Buy :
                OrderSide.Sell, OrderType.Limit, volume, new TraderId(random.Next(1, 100).ToString()));
            }

            // Start time for checking LINQ's performance to search for an item
            var linqStart = DateTime.Now;
            Order linqOrder = (from order1 in orders
                           where order1.OrderId.Id == "3007"
                           select order1).ToList().First();
            var linqEnd = DateTime.Now;

            Console.WriteLine("Linq's Time: " + (linqEnd - linqStart).TotalSeconds);

            var forStart = DateTime.Now;
            Order forOrder = null;
            for (int i = 0; i < orders.Length; i++)
            {
                if (orders[i].OrderId.Id == "3007")
                {
                    forOrder = orders[i];
                    break;
                }
            }

            var forEnd = DateTime.Now;
            Console.WriteLine("For's Time: " + (forEnd - forStart).TotalSeconds);

            var foreachStart = DateTime.Now;

            Order foreachOrder = null;
            foreach (var order in orders)
            {
                if (order.OrderId.Id == "3007")
                {
                    foreachOrder = order;
                    break;
                }
            }

            var foreachEnd = DateTime.Now;
            Console.WriteLine("Foreach's Time: " + (foreachStart - foreachEnd).TotalSeconds);
        }

    /* [Test]
        public void DepthLevelMapTest_IfSideIsBuy_WillSortInDescendingOrder()
        {
            DepthLevelMap depthLevelMap = new DepthLevelMap("XBTUSD", OrderSide.Sell);
            depthLevelMap.addle(new Price(495.43M), new Volume(300), OrderSide.Sell);
            depthLevelMap.AddDepthLevel(new Price(485.65M), new Volume(300), OrderSide.Sell);
            depthLevelMap.AddDepthLevel(new Price(490.65M), new Volume(300), OrderSide.Sell);

            Assert.AreEqual(485.65M, depthLevelMap.DepthLevels.First().Key, "First element of Sell Orders list");
            Assert.AreEqual(495.43M, depthLevelMap.DepthLevels.Last().Key, "Last element of Sell Orders list");
        }

        [Test]
        public void DepthLevelMapTest_IfSideIsBuy_WillSortInAscendingOrder()
        {
            DepthLevelMap depthLevelMap = new DepthLevelMap("XBTUSD", OrderSide.Buy);
            depthLevelMap.AddDepthLevel(new Price(485.33M), new Volume(300), OrderSide.Buy);
            depthLevelMap.AddDepthLevel(new Price(490.33M), new Volume(300), OrderSide.Buy);
            depthLevelMap.AddDepthLevel(new Price(495.33M), new Volume(300), OrderSide.Buy);

            Assert.AreEqual(495.33M, depthLevelMap.DepthLevels.First().Key, "First element of Buy Orders list");
            Assert.AreEqual(485.33M, depthLevelMap.DepthLevels.Last().Key, "Last element of Buy Orders list");
        }*/
    }
}
