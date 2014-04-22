using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.IntegrationTests
{
    [TestFixture]
    public class JournalerTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;

        [SetUp]
        public void SetUp()
        {
            //initialize journaler
            _eventStore=new RavenNEventStore();
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { journaler });
            _manualResetEvent=new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        [Category("Integration")]
        public void CreateOrder_PublishToInputDisruptor_JournalerShouldSaveTheEvent()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            InputPayload payload = InputPayload.CreatePayload(order);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(5000);
            //retrieve order
            Order.Order savedOrder = _eventStore.GetEvent(order.OrderId.Id.ToString()) as Order.Order;
            Assert.NotNull(savedOrder);
            Assert.AreEqual(savedOrder,order);

        }
        
    }
}
