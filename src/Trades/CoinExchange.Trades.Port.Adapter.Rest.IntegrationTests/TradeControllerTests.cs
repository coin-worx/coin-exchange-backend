using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Test cases for the Trades Controller
    /// </summary>
    class TradeControllerTests
    {
        // NOTE: MAKE SURE THERE ARE NO ORDERS OR TRADES IN THE DATABASE TABLES OTHERWISE TEST RESUTLS WILL BE AFFECTED

        [Test]
        [Category("Integration")]
        public void GetAllTradesTest_TestsTheMethodThatWillGetAllTradesForACurrencypair_AssertsTheValuesOfTheFetchedTrades()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

            // Get the instance through Spring configuration
            TradeController tradeController = (TradeController)applicationContext["TradeController"];
            tradeController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tradeController.Request.Headers.Add("Auth", "123456789");
            ITradeRepository tradeRepository = (ITradeRepository)applicationContext["TradeRepository"];

            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD", Price = 491, Volume = 100, Side = "buy", Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 492, Volume = 300, Side = "buy", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 493, Volume = 1000, Side = "buy", Type = "limit"
                                        });

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 493, Volume = 1000, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 492, Volume = 300, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 491, Volume = 100, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            IHttpActionResult httpActionResult = tradeController.RecentTrades("BTCUSD", "");
            OkNegotiatedContentResult<IList<object>> okResponse = (OkNegotiatedContentResult<IList<object>>) httpActionResult;

            IList<object> objectList = (IList<object>)okResponse.Content;
            List<object[]> newObjectsList = new List<object[]>();

            for (int i = 0; i < objectList.Count; i++)
            {
                object[] objects = objectList[i] as object[];
                newObjectsList.Add(objects);
            }

            Assert.AreEqual(493, newObjectsList[0][1]);
            Assert.AreEqual(1000, newObjectsList[0][2]);

            Assert.AreEqual(492, newObjectsList[1][1]);
            Assert.AreEqual(300, newObjectsList[1][2]);

            Assert.AreEqual(491, newObjectsList[2][1]);
            Assert.AreEqual(100, newObjectsList[2][2]);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();

            tradeRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetTradesForTraderTest_TestsTheMethodThatWillGetAllTradesForACurrencypair_AssertsTheValuesOfTheFetchedTrades()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

            // Get the instance through Spring configuration
            TradeController tradeController = (TradeController)applicationContext["TradeController"];
            tradeController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tradeController.Request.Headers.Add("Auth", "123456789");

            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            ITradeRepository tradeRepository = (ITradeRepository)applicationContext["TradeRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 492,
                Volume = 300,
                Side = "buy",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 493,
                Volume = 1000,
                Side = "buy",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 493,
                Volume = 1000,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 492,
                Volume = 300,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            IHttpActionResult httpActionResult = tradeController.GetTradeHistory(new TradeHistoryParams("", "", false, 
                "", ""));
            OkNegotiatedContentResult<object> okResponseObject = (OkNegotiatedContentResult<object>)httpActionResult;

            IList<object> objectList = (IList<object>)okResponseObject.Content;
            List<object[]> newObjectsList = new List<object[]>();

            for (int i = 0; i < objectList.Count; i++)
            {
                object[] objects = objectList[i] as object[];
                newObjectsList.Add(objects);
            }

            Assert.AreEqual(493, newObjectsList[0][2]);
            Assert.AreEqual(1000, newObjectsList[0][3]);

            Assert.AreEqual(492, newObjectsList[1][2]);
            Assert.AreEqual(300, newObjectsList[1][3]);

            Assert.AreEqual(491, newObjectsList[2][2]);
            Assert.AreEqual(100, newObjectsList[2][3]);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            tradeRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetTradesForTraderBetweenTimeRangeTest_TestsTheMethodThatWillGetAllTradesForACurrencypair_AssertsTheValuesOfTheFetchedTrades()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

            // Get the instance through Spring configuration
            TradeController tradeController = (TradeController)applicationContext["TradeController"];
            tradeController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tradeController.Request.Headers.Add("Auth", "123456789");

            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            ITradeRepository tradeRepository = (ITradeRepository)applicationContext["TradeRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 492,
                Volume = 300,
                Side = "buy",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 493,
                Volume = 1000,
                Side = "buy",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 493,
                Volume = 1000,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 492,
                Volume = 300,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            IHttpActionResult httpActionResult = tradeController.GetTradeHistory(new TradeHistoryParams("", "", false,
                "2014/05/09", "2014/05/09"));
            OkNegotiatedContentResult<object> okResponseObject = (OkNegotiatedContentResult<object>)httpActionResult;

            IList<object> objectList = (IList<object>)okResponseObject.Content;
            List<object[]> newObjectsList = new List<object[]>();

            for (int i = 0; i < objectList.Count; i++)
            {
                object[] objects = objectList[i] as object[];
                newObjectsList.Add(objects);
            }

            Assert.AreEqual(493, newObjectsList[0][2]);
            Assert.AreEqual(1000, newObjectsList[0][3]);

            Assert.AreEqual(492, newObjectsList[1][2]);
            Assert.AreEqual(300, newObjectsList[1][3]);

            Assert.AreEqual(491, newObjectsList[2][2]);
            Assert.AreEqual(100, newObjectsList[2][3]);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();

            tradeRepository.RollBack();
        }

        /// <summary>
        /// Tests the functionality to fecth all the open orders
        /// </summary>
        [Test]
        public void GetOpenOrdersTestCase()
        {
            //OrderController orderController = new OrderController();
            //IHttpActionResult httpActionResult = orderController.QueryOpenOrders(null);

            //// The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            //// Http response messages sent to the client.  If it is not null, menas the request was successful
            //Assert.IsNotNull(httpActionResult);
            //Assert.AreEqual(httpActionResult.GetType(), typeof(OkNegotiatedContentResult<List<Order>>));
            //OkNegotiatedContentResult<List<Order>> okResponseMessage = (OkNegotiatedContentResult<List<Order>>)httpActionResult;

            //// If the response message contains content and its count is greater than 0, our test is successful
            //Assert.IsNotNull(okResponseMessage.Content);
            //Assert.GreaterOrEqual(okResponseMessage.Content.Count(), 1, "Count of the contents in the OK response message");
        }
    }
}
