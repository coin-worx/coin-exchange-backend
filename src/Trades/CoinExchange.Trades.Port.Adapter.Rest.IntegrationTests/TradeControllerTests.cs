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
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NHibernate.Engine;
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
        private DatabaseUtility _databaseUtility;
        private IEventStore inputEventStore;
        private IEventStore outputEventStore;
        private Journaler inputJournaler;
        private Journaler outputJournaler;


        [SetUp]
        public new void SetUp()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            inputJournaler = new Journaler(inputEventStore);
            outputJournaler = new Journaler(outputEventStore);
            IList<CurrencyPair> currencyPairs = new List<CurrencyPair>();
            currencyPairs.Add(new CurrencyPair("BTCUSD", "USD", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCLTC", "LTC", "BTC"));
            currencyPairs.Add(new CurrencyPair("BTCDOGE", "DOGE", "BTC"));
            Exchange exchange = new Exchange(currencyPairs);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
        }

        [TearDown]
        public new void TearDown()
        {
            _databaseUtility.Create();
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            inputEventStore.RemoveAllEvents();
            outputEventStore.RemoveAllEvents();
        }

        [Test]
        [Category("Integration")]
        public void GetAllTradesTest_TestsTheMethodThatWillGetAllTradesForACurrencypair_AssertsTheValuesOfTheFetchedTrades()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            //Exchange exchange = new Exchange();
            //IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            //IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler inputJournaler = new Journaler(inputEventStore);
            //Journaler outputJournaler = new Journaler(outputEventStore);
            //InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            //OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

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

            Assert.AreEqual(493, newObjectsList[2][1]);
            Assert.AreEqual(1000, newObjectsList[2][2]);

            Assert.AreEqual(492, newObjectsList[1][1]);
            Assert.AreEqual(300, newObjectsList[1][2]);

            Assert.AreEqual(491, newObjectsList[0][1]);
            Assert.AreEqual(100, newObjectsList[0][2]);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();

            tradeRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetTradeDetails_SendMatchingOrdersWithDifferentTraders_TradeDetailsShouldGetReceived()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            
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
            OrderController orderController1 = (OrderController)applicationContext["OrderController"];
            orderController1.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController1.Request.Headers.Add("Auth", "55555");
            orderController1.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 490,
                Volume = 300,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            IHttpActionResult httpActionResult = tradeController.GetTradeHistory(new TradeHistoryParams("", ""));
            OkNegotiatedContentResult<object> okResponseObject = (OkNegotiatedContentResult<object>)httpActionResult;

            IList<object> objectList = (IList<object>)okResponseObject.Content;
            object[] details = objectList[0] as object[];
            string tradeId = details[0] as string;

            //verify trader("123456789") details
            httpActionResult = tradeController.TradeDetails(tradeId);
            OkNegotiatedContentResult<TradeDetailsRepresentation> tradeDetails =
                (OkNegotiatedContentResult<TradeDetailsRepresentation>) httpActionResult;
            Assert.AreEqual(tradeDetails.Content.TradeId, tradeId);
            Assert.AreEqual(tradeDetails.Content.ExecutionPrice,(decimal)details[2]);
            Assert.AreEqual(tradeDetails.Content.Volume, (decimal)details[3]);
            Assert.AreEqual(tradeDetails.Content.Order.Price,491);
            Assert.AreEqual(tradeDetails.Content.Order.Side, OrderSide.Buy.ToString());
            Assert.AreEqual(tradeDetails.Content.Order.Type, OrderType.Limit.ToString());
            Assert.AreEqual(tradeDetails.Content.Order.Volume, 100);
            Assert.AreEqual(tradeDetails.Content.Order.Status, OrderState.Complete.ToString());

            //verify trader("55555") details
            TradeController tradeController1 = (TradeController)applicationContext["TradeController"];
            tradeController1.Request = new HttpRequestMessage(HttpMethod.Post, "");
            tradeController1.Request.Headers.Add("Auth", "55555");
            httpActionResult = tradeController1.TradeDetails(tradeId);
            OkNegotiatedContentResult<TradeDetailsRepresentation> tradeDetails1 =
                (OkNegotiatedContentResult<TradeDetailsRepresentation>)httpActionResult;
            Assert.AreEqual(tradeDetails1.Content.TradeId, tradeId);
            Assert.AreEqual(tradeDetails1.Content.ExecutionPrice, (decimal)details[2]);
            Assert.AreEqual(tradeDetails1.Content.Volume, (decimal)details[3]);
            Assert.AreEqual(tradeDetails1.Content.Order.Price, 490);
            Assert.AreEqual(tradeDetails1.Content.Order.Side, OrderSide.Sell.ToString());
            Assert.AreEqual(tradeDetails1.Content.Order.Type, OrderType.Limit.ToString());
            Assert.AreEqual(tradeDetails1.Content.Order.Volume, 300);
            Assert.AreEqual(tradeDetails1.Content.Order.Status,OrderState.PartiallyFilled.ToString());

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
            //Exchange exchange = new Exchange();
            //IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            //IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler inputJournaler = new Journaler(inputEventStore);
            //Journaler outputJournaler = new Journaler(outputEventStore);
            //InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            //OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

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
            IHttpActionResult httpActionResult = tradeController.GetTradeHistory(new TradeHistoryParams("", ""));
            OkNegotiatedContentResult<object> okResponseObject = (OkNegotiatedContentResult<object>)httpActionResult;

            IList<object> objectList = (IList<object>)okResponseObject.Content;
            List<object[]> newObjectsList = new List<object[]>();

            for (int i = 0; i < objectList.Count; i++)
            {
                object[] objects = objectList[i] as object[];
                newObjectsList.Add(objects);
            }

            Assert.AreEqual(493, newObjectsList[2][2]);
            Assert.AreEqual(1000, newObjectsList[2][3]);

            Assert.AreEqual(492, newObjectsList[1][2]);
            Assert.AreEqual(300, newObjectsList[1][3]);

            Assert.AreEqual(491, newObjectsList[0][2]);
            Assert.AreEqual(100, newObjectsList[0][3]);

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
            //Exchange exchange = new Exchange();
            //IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            //IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            //Journaler inputJournaler = new Journaler(inputEventStore);
            //Journaler outputJournaler = new Journaler(outputEventStore);
            //InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            //OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });

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
            IHttpActionResult httpActionResult = tradeController.GetTradeHistory(new TradeHistoryParams("2014/05/09", "2014/05/09"));
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
