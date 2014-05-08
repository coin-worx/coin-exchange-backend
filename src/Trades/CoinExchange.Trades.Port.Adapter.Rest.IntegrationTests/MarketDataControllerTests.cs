using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.MemoryImages;
using Disruptor;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Tests the controller that queries MarketData from the ReadModel
    /// </summary>
    [TestFixture]
    class MarketDataControllerTests
    {
        #region End-to-End Test

        [Test]
        [Category("Integration")]
        public void SubmitOrderAndGetOrderBookTest_SubmitsTheOrderAndChecksIfOrderBookReflectsTheChangesExpected_VerifiesThroughOrderBooksFields()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[]{outputJournaler});

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
            
            OkNegotiatedContentResult<NewOrderRepresentation> orderRepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>) orderHttpResult;
            Assert.IsNotNull(orderRepresentationContent.Content);
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

            Assert.AreEqual(1000, orderBooks.Item1.ToList()[0].Item1); // Volume @ slot 1 in bid OrderBook
            Assert.AreEqual(493, orderBooks.Item1.ToList()[0].Item2); // Price @ slot 1 in bid OrderBook
            Assert.AreEqual(700, orderBooks.Item2.ToList()[0].Item1); // Volume @ slot 1 in ask OrderBook
            Assert.AreEqual(497, orderBooks.Item2.ToList()[0].Item2); // Price @ slot 1 in ask OrderBook

            Assert.AreEqual(300, orderBooks.Item1.ToList()[1].Item1); // Volume @ slot 2 in bid OrderBook
            Assert.AreEqual(492, orderBooks.Item1.ToList()[1].Item2); // Price @ slot 2 in bid OrderBook
            Assert.AreEqual(800, orderBooks.Item2.ToList()[1].Item1); // Volume @ slot 2 in ask OrderBook
            Assert.AreEqual(498, orderBooks.Item2.ToList()[1].Item2); // Price @ slot 2 in ask OrderBook

            Assert.AreEqual(100, orderBooks.Item1.ToList()[2].Item1); // Volume @ slot 3 in bid OrderBook
            Assert.AreEqual(491, orderBooks.Item1.ToList()[2].Item2); // Price @ slot 3 in bid OrderBook
            Assert.AreEqual(900, orderBooks.Item2.ToList()[2].Item1); // Volume @ slot 3 in ask OrderBook
            Assert.AreEqual(499, orderBooks.Item2.ToList()[2].Item2); // Price @ slot 3 in ask OrderBook

            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
        }

        [Test]
        [Category("Integration")]
        public void SubmitOrdersAndGetDepthTest_SubmitsAnOrderAndGetsTheDepthAsTheResult_VerifiesIfDepthIsAsExpected()
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

            OkNegotiatedContentResult<NewOrderRepresentation> orderRepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>)orderHttpResult;
            Assert.IsNotNull(orderRepresentationContent.Content);
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 491,
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
                Price = 498,
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
            IHttpActionResult marketDataHttpResult = marketController.GetDepth("BTCUSD");

            OkNegotiatedContentResult<Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]>>)marketDataHttpResult;

            Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> returnedDepth = okResponseMessage.Content;

            Assert.AreEqual(1000, returnedDepth.Item1[0].Item1); // Volume of the first Bid DepthLevel
            Assert.AreEqual(493, returnedDepth.Item1[0].Item2); // Price of the first Bid DepthLevel
            Assert.AreEqual(400, returnedDepth.Item1[1].Item1); // Volume of the second Bid DepthLevel
            Assert.AreEqual(491, returnedDepth.Item1[1].Item2); // Price of the second Bid DepthLevel

            Assert.AreEqual(700, returnedDepth.Item2[0].Item1); // Volume of the first Bid DepthLevel
            Assert.AreEqual(497, returnedDepth.Item2[0].Item2); // Price of the first Bid DepthLevel
            Assert.AreEqual(1700, returnedDepth.Item2[1].Item1); // Volume of the second Bid DepthLevel
            Assert.AreEqual(498, returnedDepth.Item2[1].Item2); // Price of the second Bid DepthLevel
        }

        [Test]
        [Category("Integration")]
        public void SubmitsThenCancelsOrders_ChecksOrderBook_AssertsOnExpectedValues()
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

            Assert.AreEqual(1000, orderBooks.Item1.ToList()[0].Item1); // Volume @ slot 1 in bid OrderBook
            Assert.AreEqual(493, orderBooks.Item1.ToList()[0].Item2); // Price @ slot 1 in bid OrderBook
            Assert.AreEqual(700, orderBooks.Item2.ToList()[0].Item1); // Volume @ slot 1 in ask OrderBook
            Assert.AreEqual(497, orderBooks.Item2.ToList()[0].Item2); // Price @ slot 1 in ask OrderBook

            Assert.AreEqual(300, orderBooks.Item1.ToList()[1].Item1); // Volume @ slot 2 in bid OrderBook
            Assert.AreEqual(492, orderBooks.Item1.ToList()[1].Item2); // Price @ slot 2 in bid OrderBook
            Assert.AreEqual(800, orderBooks.Item2.ToList()[1].Item1); // Volume @ slot 2 in ask OrderBook
            Assert.AreEqual(498, orderBooks.Item2.ToList()[1].Item2); // Price @ slot 2 in ask OrderBook

            Assert.AreEqual(100, orderBooks.Item1.ToList()[2].Item1); // Volume @ slot 3 in bid OrderBook
            Assert.AreEqual(491, orderBooks.Item1.ToList()[2].Item2); // Price @ slot 3 in bid OrderBook
            Assert.AreEqual(900, orderBooks.Item2.ToList()[2].Item1); // Volume @ slot 3 in ask OrderBook
            Assert.AreEqual(499, orderBooks.Item2.ToList()[2].Item2); // Price @ slot 3 in ask OrderBook

            IHttpActionResult httpActionResult = orderController.CancelOrder(order1RepresentationContent.Content.OrderId);
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(300000);
        }

        #endregion End-to-End Test

        #region TickerInfoResponse
        [Test]
        [Category("Integration")]
        public void Test()
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
                Pair = "XBTUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });

            OkNegotiatedContentResult<NewOrderRepresentation> orderRepresentationContent = (OkNegotiatedContentResult<NewOrderRepresentation>)orderHttpResult;
            Assert.IsNotNull(orderRepresentationContent.Content);
            manualResetEvent.Reset();

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 300,
                Side = "buy",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 1000,
                Side = "sell",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 900,
                Side = "buy",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 498,
                Volume = 800,
                Side = "sell",
                Type = "limit"
            });

            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 497,
                Volume = 700,
                Side = "sell",
                Type = "limit"
            });

            manualResetEvent.WaitOne(80000);
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            IHttpActionResult tickerinfo = marketController.TickerInfo("XBTUSD");
            OkNegotiatedContentResult<TickerInfoReadModel> readmodel = tickerinfo as OkNegotiatedContentResult<TickerInfoReadModel>;
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
        }

        #endregion

        [Test]
        [Category("Integration")]
        public void GetOrderBookTest_ChecksIfOrderBookIsRetreivedProperly_ValidatesReturnedOrderBook()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            MarketController marketController = (MarketController)applicationContext["MarketController"];
            LimitOrderBook limitOrderBook = new LimitOrderBook("XBTUSD");

            Order buyOrder1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 150, 481.34M, new StubbedOrderIdGenerator());
            Order buyOrder2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 482.34M, new StubbedOrderIdGenerator());
            Order buyOrder3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_BUY, 50, 483.34M, new StubbedOrderIdGenerator());
            Order sellOrder1 = OrderFactory.CreateOrder("1233", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 150, 491.34M, new StubbedOrderIdGenerator());
            Order sellOrder2 = OrderFactory.CreateOrder("1234", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 492.34M, new StubbedOrderIdGenerator());
            Order sellrder3 = OrderFactory.CreateOrder("1222", "XBTUSD", Constants.ORDER_TYPE_LIMIT,
                Constants.ORDER_SIDE_SELL, 50, 493.34M, new StubbedOrderIdGenerator());

            limitOrderBook.PlaceOrder(buyOrder1);
            limitOrderBook.PlaceOrder(buyOrder2);
            limitOrderBook.PlaceOrder(buyOrder3);
            limitOrderBook.PlaceOrder(sellOrder1);
            limitOrderBook.PlaceOrder(sellOrder2);
            limitOrderBook.PlaceOrder(sellrder3);

            OrderBookMemoryImage orderBookMemoryImage = (OrderBookMemoryImage) applicationContext["OrderBookMemoryImage"];
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);
            IHttpActionResult httpActionResult = marketController.GetOrderBook("XBTUSD");

            OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>> okResponseMessage =
                (OkNegotiatedContentResult<Tuple<OrderRepresentationList, OrderRepresentationList>>)httpActionResult;

            Assert.IsNotNull(okResponseMessage);
            Assert.IsNotNull(okResponseMessage.Content);
            // Check the Currency Pair for Bid Book
            Assert.AreEqual("XBTUSD", okResponseMessage.Content.Item1.CurrencyPair);
            // Check the Currency Pair for Ask Book
            Assert.AreEqual("XBTUSD", okResponseMessage.Content.Item2.CurrencyPair);

            // Count of the number of Bids in the Bid Order Book
            Assert.AreEqual(3, okResponseMessage.Content.Item1.Count());
            // Count of the number of Asks in the Ask Order Book
            Assert.AreEqual(3, okResponseMessage.Content.Item2.Count());

            Assert.AreEqual(50, okResponseMessage.Content.Item1.ToList()[0].Item1);// Highest Bid Volumein Bid Order Book
            Assert.AreEqual(483.34M, okResponseMessage.Content.Item1.ToList()[0].Item2);// Highest Bid Price in Bid Order Book

            Assert.AreEqual(150, okResponseMessage.Content.Item2.ToList()[0].Item1);// Highest Ask Volumein Ask Order Book
            Assert.AreEqual(491.34M, okResponseMessage.Content.Item2.ToList()[0].Item2);// Highest Ask Price in Ask Order Book
        }

        [Test]
        [Category("Integration")]
        public void GetDepthFromControllerTest_TestsTheLinkBetweenMarketControllerAndMArketQueryService_ChecksTheOutputToBeAsExpected()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            MarketController marketController = (MarketController)applicationContext["MarketController"];
             Depth depth = new Depth("XBTUSD", 3);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(490), new Volume(200), OrderSide.Sell);

            DepthMemoryImage depthMemoryImage = (DepthMemoryImage)applicationContext["DepthMemoryImage"];
            depthMemoryImage.OnDepthArrived(depth);
            
            IHttpActionResult httpActionResult = marketController.GetDepth("XBTUSD");

            OkNegotiatedContentResult<Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]>> returnedDepths =
            (OkNegotiatedContentResult<Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]>>) httpActionResult;

            Assert.IsNotNull(returnedDepths.Content);
            // Bid Depth First Element represented as Item1
            Assert.AreEqual(300, returnedDepths.Content.Item1.ToList()[0].Item1); // Aggregated Volume
            Assert.AreEqual(492, returnedDepths.Content.Item1.ToList()[0].Item2); // Price
            Assert.AreEqual(2, returnedDepths.Content.Item1.ToList()[0].Item3); // OrderCount

            // Ask Depth First Element represented as Item2
            Assert.AreEqual(400, returnedDepths.Content.Item2.ToList()[0].Item1); // Aggregated Volume
            Assert.AreEqual(490, returnedDepths.Content.Item2.ToList()[0].Item2); // Price
            Assert.AreEqual(3, returnedDepths.Content.Item2.ToList()[0].Item3); // OrderCount
        }
    }
}
