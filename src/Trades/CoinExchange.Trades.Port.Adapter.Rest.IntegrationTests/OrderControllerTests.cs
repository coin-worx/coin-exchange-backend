using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.MemoryImages;
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
        [Test]
        [Category("Integration")]
        public void SendNewBuyOrderTest_TestsTheReturnedOrderRepresentationIfItIsAsExpected_VerfiesTheSubmittedState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];

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
        }

        [Test]
        [Category("Integration")]
        public void SendNewSellOrderTest_TestsTheReturnedOrderRepresentationIfItIsAsExpected_VerfiesTheSubmittedState()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];

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
        }

        [Test]
        [Category("Integration")]
        public void GetOpenOrders_RetreivesTheListOfOpenOrdersFromTheDatabase_VerifiesThatResultingOrdersAreInExpectedRange()
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
            OrderController orderController = (OrderController)applicationContext["OrderController"];

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

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 497,
                Volume = 700,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)marketDataHttpResult;

            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = okResponseMessage.Content;
            Assert.AreEqual(3, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(3, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            IHttpActionResult queryClosedOrders = orderController.QueryClosedOrders(null);
            // ToDo: Closed Orders test continue
        }

        // ToDo: Open Orders test
    }
}
