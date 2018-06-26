/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
