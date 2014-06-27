using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.MemoryImages;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    public class OrderbookReplayTests
    {
        private DatabaseUtility _databaseUtility;
        private IEventStore inputEventStore;
        private IEventStore outputEventStore;
        private Journaler inputJournaler;
        private Journaler outputJournaler;
        private Exchange _exchange;
        private IApplicationContext _applicationContext;

        [SetUp]
        public void SetUp()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            inputJournaler = new Journaler(inputEventStore);
            outputJournaler = new Journaler(outputEventStore);
            _exchange = new Exchange();
            _exchange.EnableSnaphots(5000);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { _exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
        }

        [TearDown]
        public void TearDown()
        {
            _databaseUtility.Create();
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            inputEventStore.RemoveAllEvents();
            outputEventStore.RemoveAllEvents();
        }

        [Test]
        [Category("Integration")]
        public void ReplayOrderBook_IfScenario1IsExecuted_VerifyTheWholeSystemState()
        {
            _applicationContext = ContextRegistry.GetContext();
            Scenario1();
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("XBTUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            OrderBookRepresentation representation = okResponseMessage.Content as OrderBookRepresentation;
            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = new Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids, representation.Asks);

            marketDataHttpResult = marketController.GetDepth("XBTUSD");
            OkNegotiatedContentResult<object> okResponseMessageDepth = (OkNegotiatedContentResult<object>)marketDataHttpResult;
            DepthTupleRepresentation beforeReplayDepth = okResponseMessageDepth.Content as DepthTupleRepresentation;
           
            IOrderRepository orderRepository = (IOrderRepository)_applicationContext["OrderRepository"];
            List<OrderReadModel> before=orderRepository.GetAllOrderOfTrader("5555");
            before=before.Concat(orderRepository.GetAllOrderOfTrader("4444")).ToList();

            ITradeRepository tradeRepository = (ITradeRepository)_applicationContext["TradeRepository"];
            IList<TradeReadModel> beforeReplayTrades = tradeRepository.GetAll();
            IList<object> beforeReplayEvents = outputEventStore.GetAllEvents();
            //down the exchange, make new exchange and reply
            CrashAndInitializeAgain();
            
            marketController = (MarketController)_applicationContext["MarketController"];
            marketDataHttpResult = marketController.GetOrderBook("XBTUSD");

            okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            representation = okResponseMessage.Content as OrderBookRepresentation;
            Tuple<OrderRepresentationList, OrderRepresentationList> orderBook1 = new Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids, representation.Asks);

            //verify orderbook state
            VerifyOrderBookStateAfterReplay(orderBooks,orderBook1);

            List<OrderReadModel> after = orderRepository.GetAllOrderOfTrader("5555");
            after=after.Concat(orderRepository.GetAllOrderOfTrader("4444")).ToList();
            //verify order table in database
            VerifyDatabaseStateAfterReplay(before,after);
            IList<TradeReadModel> afterReplayTrades = tradeRepository.GetAll();
            //verify trades table in database
            VerifyDatabaseStateAfterReplay(beforeReplayTrades,afterReplayTrades);
            IList<object> afterReplayEvents = outputEventStore.GetAllEvents();
            //verify event store state
            VerifyEventStoreStateAfterReplay(beforeReplayEvents,afterReplayEvents);

            marketDataHttpResult = marketController.GetDepth("XBTUSD");
            okResponseMessageDepth = (OkNegotiatedContentResult<object>)marketDataHttpResult;
            DepthTupleRepresentation afterReplayDepth = okResponseMessageDepth.Content as DepthTupleRepresentation;
            VerifyDepthBeforeAndAfterReplay(beforeReplayDepth,afterReplayDepth);
        }

        [Test]
        [Category("Integration")]
        public void ReplayOrderBook_IfScenario2IsExecuted_VerifyTheWholeSystemState()
        {
            _applicationContext = ContextRegistry.GetContext();
            Scenario2();
            MarketController marketController = (MarketController)_applicationContext["MarketController"];
            IHttpActionResult marketDataHttpResult = marketController.GetOrderBook("XBTUSD");

            OkNegotiatedContentResult<object> okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            OrderBookRepresentation representation = okResponseMessage.Content as OrderBookRepresentation;
            Tuple<OrderRepresentationList, OrderRepresentationList> orderBooks = new Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids, representation.Asks);

            marketDataHttpResult = marketController.GetDepth("XBTUSD");
            OkNegotiatedContentResult<object> okResponseMessageDepth = (OkNegotiatedContentResult<object>)marketDataHttpResult;
            DepthTupleRepresentation beforeReplayDepth = okResponseMessageDepth.Content as DepthTupleRepresentation;

            IOrderRepository orderRepository = (IOrderRepository)_applicationContext["OrderRepository"];
            List<OrderReadModel> before = orderRepository.GetAllOrderOfTrader("5555");
            before = before.Concat(orderRepository.GetAllOrderOfTrader("4444")).ToList();

            ITradeRepository tradeRepository = (ITradeRepository)_applicationContext["TradeRepository"];
            IList<TradeReadModel> beforeReplayTrades = tradeRepository.GetAll();
            IList<object> beforeReplayEvents = outputEventStore.GetAllEvents();
            //down the exchange, make new exchange and reply
            CrashAndInitializeAgainWithSnapshot();

            marketController = (MarketController)_applicationContext["MarketController"];
            marketDataHttpResult = marketController.GetOrderBook("XBTUSD");

            okResponseMessage =
                (OkNegotiatedContentResult<object>)marketDataHttpResult;
            representation = okResponseMessage.Content as OrderBookRepresentation;
            Tuple<OrderRepresentationList, OrderRepresentationList> orderBook1 = new Tuple<OrderRepresentationList, OrderRepresentationList>(representation.Bids, representation.Asks);

            //verify orderbook state
            VerifyOrderBookStateAfterReplay(orderBooks, orderBook1);

            List<OrderReadModel> after = orderRepository.GetAllOrderOfTrader("5555");
            after = after.Concat(orderRepository.GetAllOrderOfTrader("4444")).ToList();
            //verify order table in database
            VerifyDatabaseStateAfterReplay(before, after);
            IList<TradeReadModel> afterReplayTrades = tradeRepository.GetAll();
            //verify trades table in database
            VerifyDatabaseStateAfterReplay(beforeReplayTrades, afterReplayTrades);
            IList<object> afterReplayEvents = outputEventStore.GetAllEvents();
            //verify event store state
            VerifyEventStoreStateAfterReplay(beforeReplayEvents, afterReplayEvents);

            marketDataHttpResult = marketController.GetDepth("XBTUSD");
            okResponseMessageDepth = (OkNegotiatedContentResult<object>)marketDataHttpResult;
            DepthTupleRepresentation afterReplayDepth = okResponseMessageDepth.Content as DepthTupleRepresentation;
            VerifyDepthBeforeAndAfterReplay(beforeReplayDepth, afterReplayDepth);
        }

        private void CrashAndInitializeAgain()
        {
            //crash
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            inputEventStore.ShutDown();
            outputEventStore.ShutDown();
            ContextRegistry.Clear();

            //initialize
            inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            inputJournaler = new Journaler(inputEventStore);
            outputJournaler = new Journaler(outputEventStore);
            _exchange = new Exchange();
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { _exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
            _applicationContext = ContextRegistry.GetContext();
            LimitOrderBookReplayService service=new LimitOrderBookReplayService();
            service.ReplayOrderBooks(_exchange,outputJournaler);
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            resetEvent.WaitOne(20000);
        }

        private void CrashAndInitializeAgainWithSnapshot()
        {
            //crash
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();
            inputEventStore.ShutDown();
            outputEventStore.ShutDown();
            _exchange.StopTimer();
            ContextRegistry.Clear();

            //initialize
            inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            inputJournaler = new Journaler(inputEventStore);
            outputJournaler = new Journaler(outputEventStore);
            _exchange = new Exchange(outputEventStore.LoadLastSnapshot());
            _exchange.EnableSnaphots(5000);
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { _exchange, inputJournaler });
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
            _applicationContext = ContextRegistry.GetContext();
            LimitOrderBookReplayService service = new LimitOrderBookReplayService();
            service.ReplayOrderBooks(_exchange, outputJournaler);
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            resetEvent.WaitOne(20000);
        }

        private void VerifyOrderBookStateAfterReplay(Tuple<OrderRepresentationList, OrderRepresentationList> orderBookBefore, Tuple<OrderRepresentationList, OrderRepresentationList> orderBookAfter)
        {
            Assert.AreEqual(orderBookBefore.Item1.Count(),orderBookAfter.Item1.Count());
            Assert.AreEqual(orderBookBefore.Item2.Count(), orderBookAfter.Item2.Count());
            var bids = orderBookBefore.Item1.ToList();
            var bids1 = orderBookAfter.Item1.ToList();
            for (int i = 0; i <bids.Count; i++)
            {
                Assert.AreEqual(bids[i].DateTime,bids1[i].DateTime);
                Assert.AreEqual(bids[i].Price, bids1[i].Price);
                Assert.AreEqual(bids[i].Volume, bids1[i].Volume);
            }

            var asks = orderBookBefore.Item2.ToList();
            var asks1 = orderBookAfter.Item2.ToList();
            for (int i = 0; i < asks.Count; i++)
            {
                Assert.AreEqual(asks[i].DateTime, asks1[i].DateTime);
                Assert.AreEqual(asks[i].Price, asks1[i].Price);
                Assert.AreEqual(asks[i].Volume, asks1[i].Volume);
            }
        }

        private void VerifyDatabaseStateAfterReplay(List<OrderReadModel> beforeReplay,List<OrderReadModel> afterReplay )
        {
            Assert.AreEqual(beforeReplay.Count,afterReplay.Count);
            for (int i = 0; i < beforeReplay.Count; i++)
            {
                Assert.AreEqual(beforeReplay[i].AveragePrice, afterReplay[i].AveragePrice);
                Assert.AreEqual(beforeReplay[i].ClosingDateTime, afterReplay[i].ClosingDateTime);
                Assert.AreEqual(beforeReplay[i].CurrencyPair, afterReplay[i].CurrencyPair);
                Assert.AreEqual(beforeReplay[i].DateTime, afterReplay[i].DateTime); 
                Assert.AreEqual(beforeReplay[i].OpenQuantity, afterReplay[i].OpenQuantity);
                Assert.AreEqual(beforeReplay[i].OrderId, afterReplay[i].OrderId);
                Assert.AreEqual(beforeReplay[i].Price, afterReplay[i].Price);
                Assert.AreEqual(beforeReplay[i].Side, afterReplay[i].Side);
                Assert.AreEqual(beforeReplay[i].Status, afterReplay[i].Status);
                Assert.AreEqual(beforeReplay[i].TraderId, afterReplay[i].TraderId);
                Assert.AreEqual(beforeReplay[i].Type, afterReplay[i].Type);
                Assert.AreEqual(beforeReplay[i].Volume, afterReplay[i].Volume);
                Assert.AreEqual(beforeReplay[i].VolumeExecuted, afterReplay[i].VolumeExecuted);
            }
        }

        private void VerifyDatabaseStateAfterReplay(IList<TradeReadModel> beforeReplay, IList<TradeReadModel> afterReplay)
        {
            Assert.AreEqual(beforeReplay.Count, afterReplay.Count);
            for (int i = 0; i < beforeReplay.Count; i++)
            {
                Assert.AreEqual(beforeReplay[i].BuyOrderId, afterReplay[i].BuyOrderId);
                Assert.AreEqual(beforeReplay[i].BuyTraderId, afterReplay[i].BuyTraderId);
                Assert.AreEqual(beforeReplay[i].CurrencyPair, afterReplay[i].CurrencyPair);
                Assert.AreEqual(beforeReplay[i].ExecutionDateTime, afterReplay[i].ExecutionDateTime);
                Assert.AreEqual(beforeReplay[i].Price, afterReplay[i].Price);
                Assert.AreEqual(beforeReplay[i].SellOrderId, afterReplay[i].SellOrderId);
                Assert.AreEqual(beforeReplay[i].SellTraderId, afterReplay[i].SellTraderId);
                Assert.AreEqual(beforeReplay[i].TradeId, afterReplay[i].TradeId);
                Assert.AreEqual(beforeReplay[i].Volume, afterReplay[i].Volume);
            }
        }

        private void VerifyEventStoreStateAfterReplay(IList<object> beforeReplay,IList<object> afterReplay)
        {
            Assert.AreEqual(beforeReplay.Count,afterReplay.Count);
            for (int i = 0; i < beforeReplay.Count; i++)
            {
                if (beforeReplay[i] is Order)
                {
                    Order beforeReplayOrder = beforeReplay[i] as Order;
                    Order afterReplayOrder = afterReplay[i] as Order;
                    Assert.NotNull(beforeReplayOrder);
                    Assert.NotNull(afterReplayOrder);
                    Assert.AreEqual(beforeReplayOrder.CurrencyPair, afterReplayOrder.CurrencyPair);
                    Assert.AreEqual(beforeReplayOrder.DateTime, afterReplayOrder.DateTime);
                    Assert.AreEqual(beforeReplayOrder.FilledCost, afterReplayOrder.FilledCost);
                    Assert.AreEqual(beforeReplayOrder.FilledQuantity, afterReplayOrder.FilledQuantity);
                    Assert.AreEqual(beforeReplayOrder.OpenQuantity, afterReplayOrder.OpenQuantity);
                    Assert.AreEqual(beforeReplayOrder.OrderId, afterReplayOrder.OrderId);
                    Assert.AreEqual(beforeReplayOrder.OrderSide, afterReplayOrder.OrderSide);
                    Assert.AreEqual(beforeReplayOrder.OrderState, afterReplayOrder.OrderState);
                    Assert.AreEqual(beforeReplayOrder.OrderType, afterReplayOrder.OrderType);
                    Assert.AreEqual(beforeReplayOrder.Price, afterReplayOrder.Price);
                    Assert.AreEqual(beforeReplayOrder.TraderId, afterReplayOrder.TraderId);
                    Assert.AreEqual(beforeReplayOrder.Volume, afterReplayOrder.Volume);
                    Assert.AreEqual(beforeReplayOrder.VolumeExecuted, afterReplayOrder.VolumeExecuted);
                }
                if (beforeReplay[i] is Trade)
                {
                    Trade beforeReplayTrade = beforeReplay[i] as Trade;
                    Trade afterReplayTrade = afterReplay[i] as Trade;
                    Assert.NotNull(beforeReplayTrade);
                    Assert.NotNull(afterReplayTrade);
                    Assert.AreEqual(beforeReplayTrade.CurrencyPair, afterReplayTrade.CurrencyPair);
                    Assert.AreEqual(beforeReplayTrade.ExecutedVolume, afterReplayTrade.ExecutedVolume);
                    Assert.AreEqual(beforeReplayTrade.ExecutionPrice, afterReplayTrade.ExecutionPrice);
                    Assert.AreEqual(beforeReplayTrade.ExecutionTime, afterReplayTrade.ExecutionTime);
                    Assert.AreEqual(beforeReplayTrade.TradeId, afterReplayTrade.TradeId);
                }
            }
        }

        private void VerifyDepthBeforeAndAfterReplay(DepthTupleRepresentation beforeReplay,
            DepthTupleRepresentation afterReplay)
        {
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(beforeReplay.BidDepth[i].Volume, afterReplay.BidDepth[i].Volume);
                Assert.AreEqual(beforeReplay.BidDepth[i].Price, afterReplay.BidDepth[i].Price);
                Assert.AreEqual(beforeReplay.BidDepth[i].OrderCount, afterReplay.BidDepth[i].OrderCount);

                Assert.AreEqual(beforeReplay.AskDepth[i].Volume, afterReplay.AskDepth[i].Volume);
                Assert.AreEqual(beforeReplay.AskDepth[i].Price, afterReplay.AskDepth[i].Price);
                Assert.AreEqual(beforeReplay.AskDepth[i].OrderCount, afterReplay.AskDepth[i].OrderCount);
            }
        }

        private void Scenario1()
        {
            string currencyPair = "XBTUSD";
            Order order1 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 100, 400, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order1));
            Order order2 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 101, 401, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order2));
            Order order3 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 102, 402, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order3));
            Order order4 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 103, 403, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order4));
            Order order5 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 104, 404, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order5));
            Order order6 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 105, 405, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order6));
            Order order7 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 106, 406, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order7));
            Order order8 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 100, 410, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order8));
            Order order9 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 101, 409, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order9));
            Order order10 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 102, 407, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order10));
            Order order11 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 103, 406, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order11));
            Order order12 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 104, 405, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order12));
            Order order13 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 105, 411, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order13));
            Order order14 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 106, 412, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order14));
            Order order15 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 107, 400, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order15));
            ManualResetEvent resetEvent=new ManualResetEvent(false);
            resetEvent.WaitOne(20000);
        }

        private void Scenario2()
        {
            string currencyPair = "XBTUSD";
            Order order1 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 100, 400, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order1));
            Order order2 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 101, 401, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order2));
            Order order3 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 102, 402, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order3));
            Order order4 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 103, 403, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order4));
            Order order5 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 104, 404, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order5));
            Order order6 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 105, 405, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order6));
            Order order7 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 106, 406, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order7));
            Order order8 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 100, 410, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(new OrderCancellation(order6.OrderId,order6.TraderId,currencyPair)));
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order8));
            Order order9 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 101, 409, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order9));
            Order order10 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 102, 407, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order10));
            Order order11 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 103, 406, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order11));
            Order order12 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 104, 405, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order12));
            Order order13 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 105, 411, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order13));
            Order order14 = OrderFactory.CreateOrder("4444", currencyPair, "limit", "sell", 106, 412, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order14));
            Order order15 = OrderFactory.CreateOrder("5555", currencyPair, "limit", "buy", 107, 400, new StubbedOrderIdGenerator());
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order15));
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(new OrderCancellation(order14.OrderId, order14.TraderId, currencyPair)));
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            resetEvent.WaitOne(20000);
        }
    }
}
