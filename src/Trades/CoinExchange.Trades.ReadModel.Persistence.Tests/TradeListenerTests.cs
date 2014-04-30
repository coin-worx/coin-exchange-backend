using System.Threading;
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
    public class TradeListenerTests : AbstractDaoIntegrationTests
    {

        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private TradeEventListener _listener;
        private IPersistanceRepository _persistance;

        public ITradeRepository TradeRepository
        {
            set { _tradeRepository = value; }
        }

        public IPersistanceRepository Persistance
        {
            set { _persistance = value; }
        }

        private ISessionFactory sessionFactory;
        private ITradeRepository _tradeRepository;


        public ISessionFactory SessionFactory
        {
            set { sessionFactory = value; }
        }

        [SetUp]
        public new void SetUp()
        {
            BeforeSetup();
            //_persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            //_orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository;
            //initialize journaler
            _eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
            _listener = new TradeEventListener(_persistance);
            AfterSetup();
        }

        [Test]
        [Category("Integration")]
        public void PublishTradeToOutputDisruptor_IfOrderListenerIsInitiated_ItShouldSaveInDatabase()
        {
            Order buyOrder = OrderFactory.CreateOrder("123", "XBTUSD", "limit", "buy", 10, 100,
              new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 10, 100,
               new StubbedOrderIdGenerator());
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            OutputDisruptor.Publish(trade);
            _manualResetEvent.WaitOne(5000);
            TradeReadModel model = _tradeRepository.GetById(trade.TradeId.Id.ToString());
            Assert.NotNull(model);
            Assert.AreEqual(model.BuyOrderId,buyOrder.OrderId.Id.ToString());
            Assert.AreEqual(model.SellOrderId, sellOrder.OrderId.Id.ToString());
            Assert.AreEqual(model.Price,100);
            Assert.AreEqual(model.CurrencyPair,"XBTUSD");
            Assert.AreEqual(model.BuyTraderId,"123");
            Assert.AreEqual(model.SellTraderId, "1234");
            Assert.AreEqual(model.Volume,10);
        }

        [TearDown]
        public new void TearDown()
        {
            BeforeTearDown();
            EndTransaction();
            AfterTearDown();
        }

        protected virtual void BeforeSetup() { }
        protected virtual void AfterSetup() { }
        protected virtual void BeforeTearDown() { }
        protected virtual void AfterTearDown() { }
    }
}
