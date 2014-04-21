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

        [SetUp]
        public void SetUp()
        {
            //initialize journaler
            Journaler journaler = new Journaler(new RavenEventStore());
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
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
                new StubbedOrderIdGenerator());
            InputPayload payload = InputPayload.CreatePayload(order);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(5000);
            //TODO:Need to verify that event has been stored
        }
        
    }
}
