using System;
using System.Collections.Generic;
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
using CoinExchange.Trades.ReadModel.EventHandlers;
using CoinExchange.Trades.ReadModel.Repositories;
using Disruptor;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderListenerTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private OrderEventListener _listener;
        private IPersistanceRepository _persistance;

        [SetUp]
        public void SetUp()
        {
            _persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            //initialize journaler
            _eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
            _listener = new OrderEventListener(_persistance);
        }

        [TearDown]
        public void TearDown()
        {

        }

        //TODO: Think of proper testcase SSR
        [Test]
        [Category("Integration")]
        public void PublishOrderToOutputDisruptor_ItShouldSaveInEventStoreAndDatabase()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            byte[] array = ObjectToByteArray(order);
            OutputDisruptor.Publish(array);
            _manualResetEvent.WaitOne(10000);
            //TODO: need to verify all things happened safely.
        }

        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}
