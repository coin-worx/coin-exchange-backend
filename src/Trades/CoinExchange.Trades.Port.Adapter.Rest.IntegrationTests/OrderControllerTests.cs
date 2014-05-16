using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.MemoryImages;
using CoinExchange.Trades.ReadModel.Persistence.NHibernate;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Tests the services in the OrderController
    /// </summary>
    [TestFixture]
    class OrderControllerTests
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
            Exchange exchange = new Exchange();
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
        public void SendNewBuyOrderTest_TestsTheReturnedOrderRepresentationIfItIsAsExpected_VerfiesTheSubmittedState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
                                                {
                                                    Pair = "BTCUSD", Price = 491, Volume = 100, Side = "buy", Type = "limit"
                                                });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Buy", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void SendNewSellOrderTest_TestsTheReturnedOrderRepresentationIfItIsAsExpected_VerfiesTheSubmittedState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Sell", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelBuyOrderTest_CancelsAnOrderAndChecksTheReponse_VerifiesTheReponsesState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Buy", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            IHttpActionResult actionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>) actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.AreEqual(true, cancelOrderResponse.Pending);
            Assert.AreEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelSellOrderTest_CancelsAnOrderAndChecksTheReponse_VerifiesTheReponsesState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Sell", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            IHttpActionResult actionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>)actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.AreEqual(true, cancelOrderResponse.Pending);
            Assert.AreEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelBuyOrderFailTest_CancelsAnOrderAndExpectsItToReturnExceptionMessage_VerifiesTheReponsesState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Buy", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            // Give Invalid Order ID to Cancel Order
            IHttpActionResult actionResult = orderController.CancelOrder("1543227");
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>)actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.IsFalse(cancelOrderResponse.Pending);
            Assert.AreNotEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelSellOrderFailTest_CancelsAnOrderAndExpectsItToReturnExceptionMessage_VerifiesTheReponsesState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Sell", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            // Give Invalid Order ID to Cancel Order
            IHttpActionResult actionResult = orderController.CancelOrder("1543227");
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>)actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.IsFalse(cancelOrderResponse.Pending);
            Assert.AreNotEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelBuyThenCancelAgainTest_FirstCancelShouldSucceedSecondShouldFail_VerifiesThroughTheResponse()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Buy", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            IHttpActionResult firstActionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> firstCancelResponse = (OkNegotiatedContentResult<CancelOrderResponse>)firstActionResult;

            CancelOrderResponse firstCancelOrderResponse = firstCancelResponse.Content;

            Assert.IsNotNull(firstCancelOrderResponse);
            Assert.AreEqual(true, firstCancelOrderResponse.Pending);
            Assert.AreEqual("Cancel Request Accepted", firstCancelOrderResponse.ResponseMessage);

            IHttpActionResult actionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>)actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.IsFalse(cancelOrderResponse.Pending);
            Assert.AreNotEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void CancelSellThenCancelAgainTest_FirstCancelShouldSucceedSecondShouldFail_VerifiesThroughTheResponse()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(3000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            NewOrderRepresentation newOrderRepresentation = okResponseMessage.Content;
            Assert.IsNotNull(newOrderRepresentation);
            Assert.AreEqual("BTCUSD", newOrderRepresentation.Pair);
            Assert.AreEqual(491, newOrderRepresentation.Price);
            Assert.AreEqual(100, newOrderRepresentation.Volume);
            Assert.AreEqual("Sell", newOrderRepresentation.Side);
            Assert.AreEqual("Limit", newOrderRepresentation.Type);

            IHttpActionResult firstActionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> firstCancelResponse = (OkNegotiatedContentResult<CancelOrderResponse>)firstActionResult;

            CancelOrderResponse firstCancelOrderResponse = firstCancelResponse.Content;

            Assert.IsNotNull(firstCancelOrderResponse);
            Assert.AreEqual(true, firstCancelOrderResponse.Pending);
            Assert.AreEqual("Cancel Request Accepted", firstCancelOrderResponse.ResponseMessage);

            IHttpActionResult actionResult = orderController.CancelOrder(newOrderRepresentation.OrderId);
            Thread.Sleep(4000);
            OkNegotiatedContentResult<CancelOrderResponse> okResponseCancel = (OkNegotiatedContentResult<CancelOrderResponse>)actionResult;

            CancelOrderResponse cancelOrderResponse = okResponseCancel.Content;

            Assert.IsNotNull(cancelOrderResponse);
            Assert.IsFalse(cancelOrderResponse.Pending);
            Assert.AreNotEqual("Cancel Request Accepted", cancelOrderResponse.ResponseMessage);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetOpenOrders_RetreivesTheListOfOpenOrdersFromTheDatabase_VerifiesThatResultingOrdersAreInExpectedRange()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            IHttpActionResult orderHttpResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            OkNegotiatedContentResult<NewOrderRepresentation> order1RepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>)orderHttpResult;
            Assert.IsNotNull(order1RepresentationContent.Content);

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

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 499,
                Volume = 900,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 498,
                Volume = 800,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 497,
                Volume = 700,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)marketDataHttpResult;

            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = okResponseMessage.Content;
            Assert.AreEqual(3, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(3, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            IHttpActionResult queryOpenOrders = orderController.QueryOpenOrders(false.ToString());

            Assert.IsNotNull(queryOpenOrders);
            OkNegotiatedContentResult<List<OrderReadModel>> reponseMessage =
                                                        (OkNegotiatedContentResult<List<OrderReadModel>>) queryOpenOrders;

            List<OrderReadModel> orderlist = reponseMessage.Content;

            Assert.AreEqual(491, orderlist[0].Price);
            Assert.AreEqual("BTCUSD", orderlist[0].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[0].OrderSide);
            Assert.AreEqual(100, orderlist[0].Volume);
            Assert.AreEqual(492, orderlist[1].Price);
            Assert.AreEqual("BTCUSD", orderlist[1].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[1].OrderSide);
            Assert.AreEqual(300, orderlist[1].Volume);
            Assert.AreEqual(493, orderlist[2].Price);
            Assert.AreEqual("BTCUSD", orderlist[2].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[2].OrderSide);
            Assert.AreEqual(1000, orderlist[2].Volume);
            Assert.AreEqual(499, orderlist[3].Price);
            Assert.AreEqual("BTCUSD", orderlist[3].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[3].OrderSide);
            Assert.AreEqual(900, orderlist[3].Volume);
            Assert.AreEqual(498, orderlist[4].Price);
            Assert.AreEqual("BTCUSD", orderlist[4].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[4].OrderSide);
            Assert.AreEqual(800, orderlist[4].Volume);
            Assert.AreEqual(497, orderlist[5].Price);
            Assert.AreEqual("BTCUSD", orderlist[5].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[5].OrderSide);
            Assert.AreEqual(700, orderlist[5].Volume);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetOpenOrdersAndIncludeTrades_RetreivesTheListOfOpenOrdersAndListOfTradesFromTheDatabase_VerifiesThatResultingOrdersAreInExpectedRange()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            IHttpActionResult orderHttpResult = orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 491,Volume = 100, Side = "buy", Type = "limit"
                                        });

            OkNegotiatedContentResult<NewOrderRepresentation> order1RepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>)orderHttpResult;
            Assert.IsNotNull(order1RepresentationContent.Content);

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

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 497, Volume = 1000, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 498, Volume = 300, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
                                        {
                                            Pair = "BTCUSD", Price = 493, Volume = 100, Side = "sell", Type = "limit"
                                        });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)marketDataHttpResult;

            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = okResponseMessage.Content;
            Assert.AreEqual(3, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(2, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            IHttpActionResult queryOpenOrders = orderController.QueryOpenOrders(true.ToString());

            Assert.IsNotNull(queryOpenOrders);
            OkNegotiatedContentResult<List<OrderReadModel>> reponseMessage =
                                                        (OkNegotiatedContentResult<List<OrderReadModel>>)queryOpenOrders;

            List<OrderReadModel> orderlist = reponseMessage.Content;

            Assert.AreEqual(491, orderlist[0].Price);
            Assert.AreEqual("BTCUSD", orderlist[0].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[0].OrderSide);
            Assert.AreEqual(100, orderlist[0].Volume);
            Assert.AreEqual(492, orderlist[1].Price);
            Assert.AreEqual("BTCUSD", orderlist[1].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[1].OrderSide);
            Assert.AreEqual(300, orderlist[1].Volume);
            Assert.AreEqual(493, orderlist[2].Price);
            Assert.AreEqual("BTCUSD", orderlist[2].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[2].OrderSide);
            Assert.AreEqual(1000, orderlist[2].Volume);
            Assert.AreEqual(900, orderlist[2].OpenQuantity);
            Assert.AreEqual(100, orderlist[2].VolumeExecuted);

            // List of Trades associated with this order, coming in as a list of object[] in each, where object[] contains
            // TraderId, ExecutionDateTime, Price, Volume, Currencypair respectively.
            IList<object> objectList = orderlist[2].Trades;
            IList<object[]> tradesList = new List<object[]>();
            for (int i = 0; i < objectList.Count; i++)
            {
                object[] objects = objectList[i] as object[];
                tradesList.Add(objects);
            }

            Assert.AreEqual(1, tradesList.Count);
            Assert.AreEqual(493, tradesList[0][2]);
            Assert.AreEqual(100, tradesList[0][3]);
            Assert.AreEqual("BTCUSD", tradesList[0][4]);

            Assert.AreEqual(497, orderlist[3].Price);
            Assert.AreEqual("BTCUSD", orderlist[3].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[3].OrderSide);
            Assert.AreEqual(1000, orderlist[3].Volume);
            Assert.AreEqual(498, orderlist[4].Price);
            Assert.AreEqual("BTCUSD", orderlist[4].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[4].OrderSide);
            Assert.AreEqual(300, orderlist[4].Volume);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetClosedOrdersAndIncludeTrades_RetreivesTheListOfClosedOrdersAndListOfTradesFromTheDatabase_VerifiesThatResultingOrdersAreInExpectedRange()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            IHttpActionResult orderHttpResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            OkNegotiatedContentResult<NewOrderRepresentation> order1RepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>)orderHttpResult;
            Assert.IsNotNull(order1RepresentationContent.Content);

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

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

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
                Price = 498,
                Volume = 300,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 497,
                Volume = 100,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(7000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)marketDataHttpResult;

            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = okResponseMessage.Content;
            Assert.AreEqual(2, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(2, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            IHttpActionResult queryClosedOrders = orderController.QueryClosedOrders(new QueryClosedOrdersParams(true, "", ""));

            Assert.IsNotNull(queryClosedOrders);
            OkNegotiatedContentResult<List<OrderReadModel>> reponseMessage =
                                                        (OkNegotiatedContentResult<List<OrderReadModel>>)queryClosedOrders;

            List<OrderReadModel> orderlist = reponseMessage.Content;

            Assert.AreEqual(493, orderlist[0].Price);
            Assert.AreEqual("BTCUSD", orderlist[0].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[0].OrderSide);

            // List of Trades associated with this order, coming in as a list of object[] in each, where object[] contains
            // TraderId, ExecutionDateTime, Price, Volume, Currencypair respectively.
            IList<object> buyObjectList = orderlist[0].Trades;
            IList<object[]> buyTradesList = new List<object[]>();
            for (int i = 0; i < buyObjectList.Count; i++)
            {
                object[] objects = buyObjectList[i] as object[];
                buyTradesList.Add(objects);
            }
            // Trades of the Buy Order 493@1000
            Assert.AreEqual(1, buyTradesList.Count);
            Assert.AreEqual(493, buyTradesList[0][2]);
            Assert.AreEqual(1000, buyTradesList[0][3]);
            Assert.AreEqual("BTCUSD", buyTradesList[0][4]);

            Assert.AreEqual(493, orderlist[1].Price);
            Assert.AreEqual("BTCUSD", orderlist[1].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[1].OrderSide);

            // List of Trades associated with this order, coming in as a list of object[] in each, where object[] contains
            // TraderId, ExecutionDateTime, Price, Volume, Currencypair respectively.
            IList<object> sellObjectList = orderlist[1].Trades;
            IList<object[]> sellTradesList = new List<object[]>();
            for (int i = 0; i < sellObjectList.Count; i++)
            {
                object[] objects = sellObjectList[i] as object[];
                sellTradesList.Add(objects);
            }

            // Trades of the Sell Order 493@1000
            Assert.AreEqual(1, sellTradesList.Count);
            Assert.AreEqual(493, sellTradesList[0][2]);
            Assert.AreEqual(1000, sellTradesList[0][3]);
            Assert.AreEqual("BTCUSD", sellTradesList[0][4]);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            orderRepository.RollBack();
        }

        [Test]
        [Category("Integration")]
        public void GetClosedOrders_RetreivesTheListOfClosedOrdersFromTheDatabase_VerifiesThatResultingOrdersAreInExpectedRange()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

            IOrderRepository orderRepository = (IOrderRepository)applicationContext["OrderRepository"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            IHttpActionResult buyOrderHttpResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            OkNegotiatedContentResult<NewOrderRepresentation> buyOrderRepresentation = (OkNegotiatedContentResult<NewOrderRepresentation>)buyOrderHttpResult;
            Assert.IsNotNull(buyOrderRepresentation.Content);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 492,
                Volume = 300,
                Side = "buy",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);

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
                Price = 499,
                Volume = 900,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 498,
                Volume = 800,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);

            IHttpActionResult sellOrderHttpContent = orderController.CreateOrder(new CreateOrderParam()
                                            {
                                                Pair = "BTCUSD", Price = 497, Volume = 700, Side = "sell", Type = "limit"
                                            });

            OkNegotiatedContentResult<NewOrderRepresentation> sellOrderRepresentation = (OkNegotiatedContentResult<NewOrderRepresentation>)sellOrderHttpContent;
            Assert.IsNotNull(sellOrderRepresentation.Content);

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            Thread.Sleep(7000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)marketDataHttpResult;

            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = okResponseMessage.Content;
            Assert.AreEqual(3, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(3, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            orderController.CancelOrder(buyOrderRepresentation.Content.OrderId);
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(3000);

            orderController.CancelOrder(sellOrderRepresentation.Content.OrderId);
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(4000);

            IHttpActionResult queryOpenOrders = orderController.QueryClosedOrders(new QueryClosedOrdersParams(false,"", ""));

            Assert.IsNotNull(queryOpenOrders);
            OkNegotiatedContentResult<List<OrderReadModel>> reponseMessage =
                                                        (OkNegotiatedContentResult<List<OrderReadModel>>)queryOpenOrders;

            List<OrderReadModel> orderlist = reponseMessage.Content;

            Assert.AreEqual(2, orderlist.Count);
            Assert.AreEqual(491, orderlist[0].Price);
            Assert.AreEqual("BTCUSD", orderlist[0].CurrencyPair);
            Assert.AreEqual("Buy", orderlist[0].OrderSide);
            Assert.AreEqual(100, orderlist[0].Volume);
            Assert.AreEqual(497, orderlist[1].Price);
            Assert.AreEqual("BTCUSD", orderlist[1].CurrencyPair);
            Assert.AreEqual("Sell", orderlist[1].OrderSide);
            Assert.AreEqual(700, orderlist[1].Volume);

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();

            orderRepository.RollBack();
        }
    }
}
