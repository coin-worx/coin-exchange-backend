using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests.Performance
{
    [TestFixture]
    class OrderBookPerformanceTest
    {
        private Exchange _exchange = null;
        [Test]
        public void PerformanceTest()
        {
            // Initialize the output Disruptor and assign the journaler as the event handler
            IEventStore eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(eventStore);
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _exchange = new Exchange();
            List<OrderId> orderIds = new List<OrderId>();
            // Create Orders
            Order[] orders = new Order[10000];
            Random random = new Random();

            for (int i = 0; i < orders.Length; i++)
            {
                bool isBuy = ((i % 2) == 0);
                decimal delta = isBuy ? 1880 : 1884;

                Price price = new Price(random.Next(1, 10) + delta);

                Volume volume = new Volume(random.Next() % 10 + 1 * 100);

                OrderId orderId = new OrderId(random.Next(1, 100));
                orderIds.Add(orderId);
                orders[i] = new Order(orderId, "BTCUSD", price, isBuy ? OrderSide.Buy :
                OrderSide.Sell, OrderType.Limit,  volume, new TraderId(random.Next(1,100)));
            }
            JustAddOrdersToList(orders);
            AddOrdersAndCancel(_exchange.ExchangeEssentials.First().LimitOrderBook, orders, orderIds);
        }

        private int AddOrders(LimitOrderBook orderBook, Order[] orders)
        {
            int count = 0;
            var start = DateTime.Now;
            for (int i = 0; i < orders.Length; i++)
            {
                if (orders[i].Price != null && orders[i].Volume != null)
                {
                    orderBook.AddOrder(orders[i]);
                    count++;
                }
                else
                {
                    throw new Exception();
                }
            }

            var end = DateTime.Now;
            Console.WriteLine("Count: {0} Time elapsed: {1} seconds", count, (end - start).TotalSeconds);
            Console.WriteLine("Bids: " + orderBook.Bids.Count() + ", Ask: " + orderBook.Asks.Count() + ", Trades: " + _exchange.ExchangeEssentials.First().TradeListener.Trades.Count());

            return count;
        }

        /// <summary>
        /// Adds and cancels orders
        /// </summary>
        /// <param name="orderBook"></param>
        /// <param name="orders"></param>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        private void AddOrdersAndCancel(LimitOrderBook orderBook, Order[] orders, List<OrderId> orderIds)
        {
            Console.WriteLine(orders.Length + " orders received.");
            var overallStart = DateTime.Now;
            Console.WriteLine("Start time: " + DateTime.Now);

            int count = 0;
            var startAdd = DateTime.Now;
            for (int i = 0; i < orders.Length; i++)
            {
                if (orders[i].Price != null && orders[i].Volume != null)
                {
                    orderBook.AddOrder(orders[i]);
                    count++;
                }
                else
                {
                    throw new Exception();
                }
            }

            var endAdd = DateTime.Now;
            Console.WriteLine(count + " orders added. : {0} | Time elapsed: {1} seconds", count, (endAdd - startAdd).TotalSeconds);
            Console.WriteLine("Bids: " + orderBook.Bids.Count() + ", Ask: " + orderBook.Asks.Count() + ", Trades: " + _exchange.ExchangeEssentials.First().TradeListener.Trades.Count());

            var startCancel = DateTime.Now;
            count = 0;
            foreach (OrderId orderId in orderIds)
            {
                if (orderId != null)
                {
                    if (orderBook.CancelOrder(orderId))
                    {
                        count++;
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            var endCancel = DateTime.Now;
            Console.WriteLine(count + " orders cancelled. : {0} | Time elapsed: {1} seconds", count, (endCancel - startCancel).TotalSeconds);
            Console.WriteLine("Bids: " + orderBook.Bids.Count() + ", Ask: " + orderBook.Asks.Count());
            
            var overAllEnd = DateTime.Now;
            Console.WriteLine("Overall Operation Time elapsed: {0} seconds", (overAllEnd - overallStart).TotalSeconds);
            Console.Write("End time: " + DateTime.Now);
        }

        private void JustAddOrdersToList(Order[] orders)
        {
            int count = 0;
            List<Order> orderList = new List<Order>();
            var startAdd = DateTime.Now;
            for (int i = 0; i < orders.Length; i++)
            {
                orderList.Add(orders[i]);
                ++count;
            }

            var endAdd = DateTime.Now;
            Console.WriteLine(count + " JUST orders added. : {0} | Time elapsed: {1} seconds", count, (endAdd - startAdd).TotalSeconds);
        }
    }
}
