using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    class ListAndMapsTests
    {
        [Test]
        public void TestSortedListVsList_WhichOneISFaster()
        {
            List<Order> orders = new List<Order>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                Order order = new Order(new OrderId(1), "XBTUSD", new Price(491 + i), OrderSide.Sell, OrderType.Limit, new Volume(2000), 
                    new TraderId(1));
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

            SortedList<decimal,Order> _sortedList = new SortedList<decimal, Order>();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                Order order = new Order(new OrderId(1), "XBTUSD", new Price(491 + i), OrderSide.Sell, OrderType.Limit, new Volume(2000),
                    new TraderId(1));
                _sortedList.Add(i, order);
            }
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;

            Console.WriteLine("Time taken for SortedList:" + timeSpan);
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
