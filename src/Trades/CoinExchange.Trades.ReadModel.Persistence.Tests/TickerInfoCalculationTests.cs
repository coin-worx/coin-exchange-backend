using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
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

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    internal class TickerInfoCalculationTests : AbstractConfiguration
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private TradeEventListener _listener;
        private IPersistanceRepository _persistance;
        private IOhlcRepository _ohlcRepository;
        private ITickerInfoRepository _tickerInfoRepository;

        public ITickerInfoRepository TickerInfoRepository
        {
            set { _tickerInfoRepository = value; }
        }
        
        public IPersistanceRepository Persistance
        {
            set { _persistance = value; }
        }

        private ISessionFactory sessionFactory;
        public ISessionFactory SessionFactory
        {
            set { sessionFactory = value; }
        }

        public TradeEventListener Listener
        {
            set { _listener = value; }
        }

        public IOhlcRepository OhlcRepository
        {
            set { _ohlcRepository = value; }
        }

        [SetUp]
        public new void SetUp()
        {
            BeforeSetup();
            log4net.Config.XmlConfigurator.Configure();
            //_persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            //_orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository;
            //initialize journaler
            _eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
            _manualResetEvent = new ManualResetEvent(false);
            //  _listener = new TradeEventListener(_persistance);
            AfterSetup();
        }

        [TearDown]
        public new void TearDown()
        {
            BeforeTearDown();
            EndTransaction();
            AfterTearDown();
        }

        [Test]
        [Category("Integration")]
        public void VerifyTickerInfoCalculations_WhenANewTradeIsArrived_NewUpdatedTickerInfoShouldGetSaved()
        {
            Order buyOrder = OrderFactory.CreateOrder("123", "XBTUSD", "limit", "buy", 10, 100,
                new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 10, 100,
                new StubbedOrderIdGenerator());

            DateTime dateTime = DateTime.Now.AddSeconds(-1*DateTime.Now.Second);
            Trade trade5 = new Trade(new TradeId("1"), "XBTUSD", new Price(2), new Volume(10), dateTime.AddDays(-1),
                buyOrder, sellOrder);
            Trade trade6 = new Trade(new TradeId("2"), "XBTUSD", new Price(3), new Volume(5), dateTime.AddDays(-1).AddMinutes(1),
                buyOrder, sellOrder);
            Trade trade1 = new Trade(new TradeId("3"), "XBTUSD", new Price(10), new Volume(10), dateTime.AddSeconds(10),
                buyOrder, sellOrder);
            Trade trade2 = new Trade(new TradeId("4"), "XBTUSD", new Price(15), new Volume(15), dateTime.AddSeconds(15),
                buyOrder, sellOrder);
            Trade trade3 = new Trade(new TradeId("5"), "XBTUSD", new Price(20), new Volume(5), dateTime.AddSeconds(20),
                buyOrder, sellOrder);
            Trade trade4 = new Trade(new TradeId("6"), "XBTUSD", new Price(5), new Volume(10), dateTime.AddSeconds(40),
                buyOrder, sellOrder);
            
            OutputDisruptor.Publish(trade5);
            _manualResetEvent.WaitOne(4000);
            OutputDisruptor.Publish(trade6);
            _manualResetEvent.WaitOne(4000);
            OutputDisruptor.Publish(trade1);
            _manualResetEvent.WaitOne(4000);
            OutputDisruptor.Publish(trade2);
            _manualResetEvent.WaitOne(4000);
            OutputDisruptor.Publish(trade3);
            _manualResetEvent.WaitOne(4000);
            OutputDisruptor.Publish(trade4);
            _manualResetEvent.WaitOne(10000);

            TickerInfoReadModel model = _tickerInfoRepository.GetTickerInfoByCurrencyPair("XBTUSD");
            Assert.NotNull(model);
            Assert.AreEqual(model.CurrencyPair,"XBTUSD");
            Assert.AreEqual(model.TradePrice, 5);
            Assert.AreEqual(model.TradeVolume, 10);
            Assert.AreEqual(model.OpeningPrice, 10);
            Assert.AreEqual(model.TodaysHigh, 20);
            Assert.AreEqual(model.Last24HoursHigh, 20);
            Assert.AreEqual(model.TodaysLow, 5);
            Assert.AreEqual(model.Last24HoursLow, 3);
            Assert.AreEqual(model.TodaysVolume, 40);
            Assert.AreEqual(model.Last24HourVolume, 45);
            Assert.AreEqual(model.TodaysTrades, 4);
            Assert.AreEqual(model.Last24HourTrades, 5);
            Assert.AreEqual(model.TodaysVolumeWeight, 11.875m);
            Assert.AreEqual(model.Last24HourVolumeWeight, 10.8889m);
        }

        protected virtual void BeforeSetup()
        {
        }

        protected virtual void AfterSetup()
        {
        }

        protected virtual void BeforeTearDown()
        {
        }

        protected virtual void AfterTearDown()
        {
        }
    }
}

