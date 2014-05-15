using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.EventHandlers;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NHibernate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OhlcCalculationTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private TradeEventListener _listener=ContextRegistry.GetContext()["TradeEventListener"] as TradeEventListener;
        private IPersistanceRepository _persistance=ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
        private IOhlcRepository _ohlcRepository=ContextRegistry.GetContext()["OhlcRepository"] as IOhlcRepository;
        private ITradeRepository _tradeRepository=ContextRegistry.GetContext()["TradeRepository"] as ITradeRepository;
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public new void SetUp()
        {
            BeforeSetup();
            log4net.Config.XmlConfigurator.Configure();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            //_persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            //_orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository;
            //initialize journaler
            _eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
           // _listener = new TradeEventListener(_persistance);
            AfterSetup();
        }

        [TearDown]
        public new void TearDown()
        {
            BeforeTearDown();
            _databaseUtility.Create();
            OutputDisruptor.ShutDown();
            AfterTearDown();
        }

        [Test]
        [Category("Integration")]
        public void CheckBarFormation_WhenANewTradeIsArrived_NewUpdatedBarShouldGetSaved()
        {
            Order buyOrder = OrderFactory.CreateOrder("123", "XBTUSD", "limit", "buy", 10, 100,
              new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 10, 100,
               new StubbedOrderIdGenerator());

            DateTime dateTime = DateTime.Now.AddSeconds(-1*DateTime.Now.Second);
            Trade trade1 = new Trade(new TradeId("1"), "XBTUSD", new Price(10), new Volume(10),dateTime.AddSeconds(10), buyOrder, sellOrder);
            Trade trade2 = new Trade(new TradeId("2"), "XBTUSD", new Price(15), new Volume(15), dateTime.AddSeconds(15), buyOrder, sellOrder);
            Trade trade3 = new Trade(new TradeId("3"), "XBTUSD", new Price(20), new Volume(5), dateTime.AddSeconds(20), buyOrder, sellOrder);
            Trade trade4 = new Trade(new TradeId("4"), "XBTUSD", new Price(5), new Volume(10), dateTime.AddSeconds(40), buyOrder, sellOrder);
            Trade trade5 = new Trade(new TradeId("5"), "XBTUSD", new Price(2), new Volume(10), dateTime.AddMinutes(1), buyOrder, sellOrder);
            Trade trade6 = new Trade(new TradeId("6"), "XBTUSD", new Price(10), new Volume(5), dateTime.AddMinutes(1.1), buyOrder, sellOrder);
            
            OutputDisruptor.Publish(trade1);
            OutputDisruptor.Publish(trade2);
            OutputDisruptor.Publish(trade3);
            OutputDisruptor.Publish(trade4);
            OutputDisruptor.Publish(trade5);
            OutputDisruptor.Publish(trade6);
            _manualResetEvent.WaitOne(10000);
            OhlcReadModel model = _ohlcRepository.GetOhlcByDateTime(dateTime.AddMinutes(1));
            OhlcReadModel model2 = _ohlcRepository.GetOhlcByDateTime(dateTime.AddMinutes(2));

            //bar 1 verification(will form from trade 1-4)
            Assert.NotNull(model);
            Assert.AreEqual(model.High,20);
            Assert.AreEqual(model.Open, 10);
            Assert.AreEqual(model.Low, 5);
            Assert.AreEqual(model.Close, 5);
            Assert.AreEqual(model.Volume, 40);

            //bar 2 verification(will form from trade 5-6)
            Assert.NotNull(model2);
            Assert.AreEqual(model2.High, 10);
            Assert.AreEqual(model2.Open, 2);
            Assert.AreEqual(model2.Low, 2);
            Assert.AreEqual(model2.Close, 10);
            Assert.AreEqual(model2.Volume, 15);
        }

        protected virtual void BeforeSetup() { }
        protected virtual void AfterSetup() { }
        protected virtual void BeforeTearDown() { }
        protected virtual void AfterTearDown() { }
    }
}
