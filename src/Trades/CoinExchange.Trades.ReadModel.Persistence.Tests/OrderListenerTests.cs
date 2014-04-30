using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.EventHandlers;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NHibernate;
using NUnit.Framework;
using Spring.Context.Support;
using Spring.Data.NHibernate;
using Spring.Data.NHibernate.Support;
using Spring.Testing.NUnit;
using Spring.Transaction.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderListenerTests:AbstractDaoIntegrationTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private OrderEventListener _listener;
        private IPersistanceRepository _persistance;

        public IOrderRepository OrderRepository
        {
            set { _orderRepository = value; }
        }

        public IPersistanceRepository Persistance
        {
            set { _persistance = value; }
        }

        private ISessionFactory sessionFactory;
        private IOrderRepository _orderRepository;


        public ISessionFactory SessionFactory
        {
            set { sessionFactory = value; }
        }

        [SetUp]
        public new void SetUp()
        {
            BeforeSetup();
            //initialize journaler
            _eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
            _listener = new OrderEventListener(_persistance);
            AfterSetup();
        }

        [TearDown]
        public new void TearDown()
        {
            BeforeTearDown();
            AfterTearDown();
        }

        [Test]
        [Category("Integration")]
        public void PublishOrderToOutputDisruptor_IfOrderListenerIsInitiated_ItShouldSaveInDatabase()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            OutputDisruptor.Publish(order);
            _manualResetEvent.WaitOne(5000);
            OrderReadModel receivedOrder = _orderRepository.GetOrderById(order.OrderId.Id.ToString());
            Assert.NotNull(receivedOrder);
            Assert.AreEqual(receivedOrder.OrderId,order.OrderId.Id.ToString());
            Assert.AreEqual(receivedOrder.OrderSide, order.OrderSide.ToString());
            Assert.AreEqual(receivedOrder.OrderType, order.OrderType.ToString());
            Assert.AreEqual(receivedOrder.Price, order.Price.Value);
            Assert.AreEqual(receivedOrder.CurrencyPair, order.CurrencyPair);
       }

        protected virtual void BeforeSetup() { }
        protected virtual void AfterSetup() { }
        protected virtual void BeforeTearDown() { }
        protected virtual void AfterTearDown() { }
      }
}
