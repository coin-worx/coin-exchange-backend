using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Application.MarketDataServices.Representation;
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
using NHibernate.Linq;
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
        private DatabaseUtility _databaseUtility;
        private IApplicationContext _applicationContext;
        private IEventStore inputEventStore;
        private IEventStore outputEventStore;
        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            _applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
        }

        [TearDown]
        public void TearDown()
        {
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            _databaseUtility.Create();
            inputEventStore.RemoveAllEvents();
            outputEventStore.RemoveAllEvents();
        }
        #region End-to-End Test

        [Test]
        [Category("Integration")]
        public void SubmitOrderAndGetOrderBookTest_SubmitsTheOrderAndChecksIfOrderBookReflectsTheChangesExpected_VerifiesThroughOrderBooksFields()
        {
            // Get the context
            //IApplicationContext applicationContext = ContextRegistry.GetContext();
            
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

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
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            OrderBookRepresentation representation = okResponseMessage.Content as OrderBookRepresentation;
            System.Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = new System.Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids , representation.Asks);
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

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(8000);
            
        }

        [Test]
        [Category("Integration")]
        public void SubmitOrdersAndGetDepthTest_SubmitsAnOrderAndGetsTheDepthAsTheResult_VerifiesIfDepthIsAsExpected()
        {
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

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
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetDepth("BTCUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            DepthTupleRepresentation returnedDepth = okResponseMessage.Content as DepthTupleRepresentation;

            Assert.AreEqual(1000, returnedDepth.BidDepth[0].Item1); // Volume of the first Bid DepthLevel
            Assert.AreEqual(493, returnedDepth.BidDepth[0].Item2); // Price of the first Bid DepthLevel
            Assert.AreEqual(400, returnedDepth.BidDepth[1].Item1); // Volume of the second Bid DepthLevel
            Assert.AreEqual(491, returnedDepth.BidDepth[1].Item2); // Price of the second Bid DepthLevel

            Assert.AreEqual(700, returnedDepth.AskDepth[0].Item1); // Volume of the first Ask DepthLevel
            Assert.AreEqual(497, returnedDepth.AskDepth[0].Item2); // Price of the first Ask DepthLevel
            Assert.AreEqual(1700, returnedDepth.AskDepth[1].Item1); // Volume of the second Ask DepthLevel
            Assert.AreEqual(498, returnedDepth.AskDepth[1].Item2); // Price of the second Ask DepthLevel

        }

        [Test]
        [Category("Integration")]
        public void SubmitsThenCancelsOrders_ChecksOrderBook_AssertsOnExpectedValues()
        {
            // Get the context
            //IApplicationContext applicationContext = ContextRegistry.GetContext();
           
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

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
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            OrderBookRepresentation representation = okResponseMessage.Content as OrderBookRepresentation;

            System.Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = new System.Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids,representation.Asks);
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
            manualResetEvent.WaitOne(3000);

            marketDataHttpResult = marketController.GetOrderBook("BTCUSD");

            okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            representation = okResponseMessage.Content as OrderBookRepresentation;

            orderBooks = new System.Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids,representation.Asks);
            Assert.AreEqual(2, orderBooks.Item1.Count()); // Count of the orders in the Bid Order book
            Assert.AreEqual(3, orderBooks.Item2.Count());// Count of the orders in the Ask Order book

            Assert.AreEqual(1000, orderBooks.Item1.ToList()[0].Item1); // Volume @ slot 1 in bid OrderBook
            Assert.AreEqual(493, orderBooks.Item1.ToList()[0].Item2); // Price @ slot 1 in bid OrderBook
            Assert.AreEqual(700, orderBooks.Item2.ToList()[0].Item1); // Volume @ slot 1 in ask OrderBook
            Assert.AreEqual(497, orderBooks.Item2.ToList()[0].Item2); // Price @ slot 1 in ask OrderBook

            Assert.AreEqual(300, orderBooks.Item1.ToList()[1].Item1); // Volume @ slot 2 in bid OrderBook
            Assert.AreEqual(492, orderBooks.Item1.ToList()[1].Item2); // Price @ slot 2 in bid OrderBook
            Assert.AreEqual(800, orderBooks.Item2.ToList()[1].Item1); // Volume @ slot 2 in ask OrderBook
            Assert.AreEqual(498, orderBooks.Item2.ToList()[1].Item2); // Price @ slot 2 in ask OrderBook

            /*Assert.AreEqual(100, orderBooks.Item1.ToList()[2].Item1); // Volume @ slot 3 in bid OrderBook
            Assert.AreEqual(491, orderBooks.Item1.ToList()[2].Item2); // Price @ slot 3 in bid OrderBook*/
            Assert.AreEqual(900, orderBooks.Item2.ToList()[2].Item1); // Volume @ slot 3 in ask OrderBook
            Assert.AreEqual(499, orderBooks.Item2.ToList()[2].Item2); // Price @ slot 3 in ask OrderBook
        }

        #endregion End-to-End Test

        #region TickerInfoResponse and ohlc response
        [Test]
        [Category("Integration")]
        public void SendSomeOrders_IfOrdersAreMatching_OhlcAndTickerInfoWillBeFormed()
        {
            // Get the context
           // IApplicationContext applicationContext = ContextRegistry.GetContext();
           
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request=new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth","123456789");

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
            manualResetEvent.WaitOne(3000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 300,
                Side = "buy",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(3000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 1000,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(3000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 492,
                Volume = 900,
                Side = "buy",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(3000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 498,
                Volume = 800,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(3000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 497,
                Volume = 700,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(20000);
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult tickerinfo = marketController.TickerInfo("XBTUSD");
            OkNegotiatedContentResult<object> readmodel = (OkNegotiatedContentResult<object>)tickerinfo;
            Assert.NotNull(readmodel);
            TickerInfoReadModel model = readmodel.Content as TickerInfoReadModel;
            Assert.NotNull(model);
            Assert.AreEqual(model.CurrencyPair, "XBTUSD");
            Assert.AreEqual(model.TradePrice, 492);
            Assert.AreEqual(model.TradeVolume, 700);
            Assert.AreEqual(model.OpeningPrice, 492);
            Assert.AreEqual(model.TodaysHigh, 492);
            Assert.AreEqual(model.Last24HoursHigh, 492);
            Assert.AreEqual(model.TodaysLow, 492);
            Assert.AreEqual(model.Last24HoursLow, 492);
            Assert.AreEqual(model.TodaysVolume, 1000);
            Assert.AreEqual(model.Last24HourVolume, 1000);
            Assert.AreEqual(model.TodaysTrades, 2);
            Assert.AreEqual(model.Last24HourTrades, 2);
            Assert.AreEqual(model.TodaysVolumeWeight, 492);
            Assert.AreEqual(model.Last24HourVolumeWeight, 492);
            Assert.AreEqual(model.AskPrice,497);
            Assert.AreEqual(model.AskVolume, 700);
            Assert.AreEqual(model.BidPrice,492);
            Assert.AreEqual(model.BidVolume,200);

            IHttpActionResult ohlcActionResult = marketController.OhlcInfo("XBTUSD");
            OkNegotiatedContentResult<OhlcRepresentation> representation = (OkNegotiatedContentResult<OhlcRepresentation>)ohlcActionResult;
            Assert.NotNull(representation);
            OhlcRepresentation ohlc = representation.Content as OhlcRepresentation;
            Assert.NotNull(ohlc);
            object[] ohlcValues = ohlc.OhlcInfo[0] as object[];
            Assert.AreEqual(ohlc.PairName,"XBTUSD");
            Assert.AreEqual(ohlcValues[0], 492);//open
            Assert.AreEqual(ohlcValues[1], 492);//high
            Assert.AreEqual(ohlcValues[2], 492);//low
            Assert.AreEqual(ohlcValues[3], 492);//close
        }

        #endregion

        [Test]
        [Category("Integration")]
        public void GetOrderBookTest_ChecksIfOrderBookIsRetreivedProperly_ValidatesReturnedOrderBook()
        {
            // Get the context
           // IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
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

            OrderBookMemoryImage orderBookMemoryImage = (OrderBookMemoryImage) _applicationContext["OrderBookMemoryImage"];
            orderBookMemoryImage.OnOrderBookChanged(limitOrderBook);
            IHttpActionResult httpActionResult = marketController.GetOrderBook("XBTUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)httpActionResult;
            OrderBookRepresentation representation = okResponseMessage.Content as OrderBookRepresentation;

            Assert.IsNotNull(okResponseMessage);
            Assert.IsNotNull(representation);
            // Check the Currency Pair for Bid Book
            Assert.AreEqual("XBTUSD", representation.Bids.CurrencyPair);
            // Check the Currency Pair for Ask Book
            Assert.AreEqual("XBTUSD", representation.Asks.CurrencyPair);

            // Count of the number of Bids in the Bid Order Book
            Assert.AreEqual(3, representation.Bids.Count());
            // Count of the number of Asks in the Ask Order Book
            Assert.AreEqual(3, representation.Asks.Count());

            Assert.AreEqual(50, representation.Bids.ToList()[0].Item1);// Highest Bid Volumein Bid Order Book
            Assert.AreEqual(483.34M, representation.Bids.ToList()[0].Item2);// Highest Bid Price in Bid Order Book

            Assert.AreEqual(150, representation.Asks.ToList()[0].Item1);// Highest Ask Volumein Ask Order Book
            Assert.AreEqual(491.34M, representation.Asks.ToList()[0].Item2);// Highest Ask Price in Ask Order Book
        }

        [Test]
        [Category("Integration")]
        public void GetDepthFromControllerTest_TestsTheLinkBetweenMarketControllerAndMArketQueryService_ChecksTheOutputToBeAsExpected()
        {
            // Get the context
            //IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            Depth depth = new Depth("XBTUSD", 3);

            depth.AddOrder(new Price(491), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(100), OrderSide.Buy);
            depth.AddOrder(new Price(492), new Volume(200), OrderSide.Buy);

            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(490), new Volume(100), OrderSide.Sell);
            depth.AddOrder(new Price(490), new Volume(200), OrderSide.Sell);

            DepthMemoryImage depthMemoryImage = (DepthMemoryImage)_applicationContext["DepthMemoryImage"];
            depthMemoryImage.OnDepthArrived(depth);
            
            IHttpActionResult httpActionResult = marketController.GetDepth("XBTUSD");

            OkNegotiatedContentResult<object> result =
            (OkNegotiatedContentResult<object>) httpActionResult;

            DepthTupleRepresentation returnedDepths = result.Content as DepthTupleRepresentation;

            Assert.IsNotNull(returnedDepths);
            // Bid Depth First Element represented as Item1
            Assert.AreEqual(300, returnedDepths.BidDepth.ToList()[0].Item1); // Aggregated Volume
            Assert.AreEqual(492, returnedDepths.BidDepth.ToList()[0].Item2); // Price
            Assert.AreEqual(2, returnedDepths.BidDepth.ToList()[0].Item3); // OrderCount

            // Ask Depth First Element represented as Item2
            Assert.AreEqual(400, returnedDepths.AskDepth.ToList()[0].Item1); // Aggregated Volume
            Assert.AreEqual(490, returnedDepths.AskDepth.ToList()[0].Item2); // Price
            Assert.AreEqual(3, returnedDepths.AskDepth.ToList()[0].Item3); // OrderCount
        }

        [Test]
        [Category("Integration")]
        public void GetRateTest_ChecksTheRateForTheGivenCurrencyPairAfterSubmittingDepthLevels_ChecksTheRateReturnedToVerify()
        {
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            BBOMemoryImage bboMemoryImage = (BBOMemoryImage)_applicationContext["BBOMemoryImage"];

            DepthLevel bidLevel1 = new DepthLevel(new Price(491));
            DepthLevel bidLevel2 = new DepthLevel(new Price(492));
            DepthLevel bidLevel3 = new DepthLevel(new Price(493));

            DepthLevel askLevel1 = new DepthLevel(new Price(494));
            DepthLevel askLevel2 = new DepthLevel(new Price(495));
            DepthLevel askLevel3 = new DepthLevel(new Price(496));
            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel1, askLevel1);
            IHttpActionResult httpActionResult = marketController.GetRate("XBT/USD");

            OkNegotiatedContentResult<Rate> returnedOkRate = (OkNegotiatedContentResult<Rate>)httpActionResult;
            Rate rate = returnedOkRate.Content;

            Assert.AreEqual("XBT/USD", rate.CurrencyPair);
            Assert.AreEqual(492.5, rate.RateValue); // MidPoint of bidLevel1 = 491 & askLevel1 = 494

            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel2, askLevel2);
            httpActionResult = marketController.GetRate("XBT/USD");
            returnedOkRate = (OkNegotiatedContentResult<Rate>)httpActionResult;
            rate = returnedOkRate.Content;

            Assert.AreEqual("XBT/USD", rate.CurrencyPair);
            Assert.AreEqual(493.5, rate.RateValue); // MidPoint of bidLevel2 = 492 & askLevel2 = 495

            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel3, askLevel3);
            httpActionResult = marketController.GetRate("XBT/USD");
            returnedOkRate = (OkNegotiatedContentResult<Rate>)httpActionResult;
            rate = returnedOkRate.Content;

            Assert.AreEqual("XBT/USD", rate.CurrencyPair);
            Assert.AreEqual(494.5, rate.RateValue); // MidPoint of bidLevel3 = 493 & askLevel3 = 496
        }

        [Test]
        [Category("Integration")]
        public void GetRatesTest_ChecksTheRatesForAllCurrencyPairsSubmittingDepthLevels_ChecksTheRatesReturnedToVerify()
        {
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            BBOMemoryImage bboMemoryImage = (BBOMemoryImage)_applicationContext["BBOMemoryImage"];

            DepthLevel xbtUsdBidLevel = new DepthLevel(new Price(491));
            DepthLevel ltcUsdBidLevel = new DepthLevel(new Price(492));
            DepthLevel btcUsdBidLevel = new DepthLevel(new Price(493));

            DepthLevel xbtUsdAskLevel = new DepthLevel(new Price(494));
            DepthLevel ltcUsdAskLevel = new DepthLevel(new Price(495));
            DepthLevel btcUsdAskLevel = new DepthLevel(new Price(496));
            bboMemoryImage.OnBBOArrived("XBT/USD", xbtUsdBidLevel, xbtUsdAskLevel);
            bboMemoryImage.OnBBOArrived("LTC/USD", ltcUsdBidLevel, ltcUsdAskLevel);
            bboMemoryImage.OnBBOArrived("BTC/USD", btcUsdBidLevel, btcUsdAskLevel);
            IHttpActionResult httpActionResult = marketController.GetAllRates();

            OkNegotiatedContentResult<RatesList> returnedOkRate = (OkNegotiatedContentResult<RatesList>)httpActionResult;
            RatesList ratesList = returnedOkRate.Content;

            Assert.AreEqual("XBT/USD", ratesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(492.5, ratesList.ToList()[0].RateValue); // MidPoint of xbtUsdBidLevel = 491 & xbtUsdAskLevel = 494
            Assert.AreEqual("LTC/USD", ratesList.ToList()[1].CurrencyPair);
            Assert.AreEqual(493.5, ratesList.ToList()[1].RateValue); // MidPoint of ltcUsdBidLevel = 492 & ltcUsdAskLevel = 495
            Assert.AreEqual("BTC/USD", ratesList.ToList()[2].CurrencyPair);
            Assert.AreEqual(494.5, ratesList.ToList()[2].RateValue); // MidPoint of btcUsdBidLevel = 493 & btcUsdAskLevel = 496
        }

        [Test]
        [Category("Integration")]
        public void GetRateEndToEndTests_SubmitsOrdersAndChecksTheRateForOneCurrencyPair_VerifiesThroughTheReturnedRate()
        {
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");
            MarketController marketController = (MarketController)_applicationContext["MarketController"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            IHttpActionResult orderHttpResult = orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 491,
                Volume = 100,
                Side = "buy",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 494,
                Volume = 900,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);

            IHttpActionResult httpActionResult = marketController.GetRate("XBTUSD");
            OkNegotiatedContentResult<Rate> okResponse = (OkNegotiatedContentResult<Rate>)httpActionResult;

            Rate rate = okResponse.Content;

            Assert.AreEqual("XBTUSD", rate.CurrencyPair);
            Assert.AreEqual(492.5, rate.RateValue); // MidPoint of xbtUsdBidLevel = 491 & xbtUsdAskLevel = 494
            
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 493,
                Volume = 1000,
                Side = "buy",
                Type = "limit"
            });
            
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            httpActionResult = marketController.GetRate("XBTUSD");
            okResponse = (OkNegotiatedContentResult<Rate>)httpActionResult;

            rate = okResponse.Content;

            Assert.AreEqual("XBTUSD", rate.CurrencyPair);
            Assert.AreEqual(493.5, rate.RateValue); // MidPoint of xbtUsdBidLevel = 493 & xbtUsdAskLevel = 496
            // Fill the existing top Ask order
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 494,
                Volume = 900,
                Side = "buy",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            // Send a new Ask order to stay on top of the ask book
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "XBTUSD",
                Price = 496,
                Volume = 900,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            httpActionResult = marketController.GetRate("XBTUSD");
            okResponse = (OkNegotiatedContentResult<Rate>)httpActionResult;

            rate = okResponse.Content;

            Assert.AreEqual("XBTUSD", rate.CurrencyPair);
            Assert.AreEqual(494.5, rate.RateValue); // MidPoint of xbtUsdBidLevel = 493 & xbtUsdAskLevel = 496
        }

        [Test]
        [Category("Integration")]
        public void GetRatesEndToEndTests_SubmitsOrdersAndChecksTheRatesForCurrencyPairs_VerifiesThroughTheReturnedRatesList()
        {
            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)_applicationContext["OrderController"];
            orderController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            orderController.Request.Headers.Add("Auth", "123456789");

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
                Pair = "XBTUSD",
                Price = 494,
                Volume = 900,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            orderController.CreateOrder(new CreateOrderParam()
            {
                Pair = "BTCUSD",
                Price = 496,
                Volume = 700,
                Side = "sell",
                Type = "limit"
            });
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(2000);
            MarketController marketController = (MarketController)_applicationContext["MarketController"];

            IHttpActionResult httpActionResult = marketController.GetAllRates();
            OkNegotiatedContentResult<RatesList> okResponse = (OkNegotiatedContentResult<RatesList>) httpActionResult;

            RatesList ratesList = okResponse.Content;

            Assert.AreEqual("XBTUSD", ratesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(492.5, ratesList.ToList()[0].RateValue); // MidPoint of xbtUsdBidLevel = 491 & xbtUsdAskLevel = 494
            Assert.AreEqual("BTCUSD", ratesList.ToList()[1].CurrencyPair);
            Assert.AreEqual(494.5, ratesList.ToList()[1].RateValue); // MidPoint of btcUsdBidLevel = 493 & btcUsdAskLevel = 496
        }
    }
}
