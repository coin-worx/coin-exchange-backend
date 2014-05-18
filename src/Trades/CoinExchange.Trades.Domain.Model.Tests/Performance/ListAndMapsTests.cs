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
    }
}
