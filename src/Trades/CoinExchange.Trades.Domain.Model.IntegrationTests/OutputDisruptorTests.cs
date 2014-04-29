using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.IntegrationTests
{
    [TestFixture]
    public class OutputDisruptorTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;

        [SetUp]
        public void SetUp()
        {
            //initialize journaler
            _eventStore = new RavenNEventStore();
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        [Category("Integration")]
        public void AddOrder()
        {
            Exchange exchange=new Exchange();
            string currencyPair = "BTCUSD";
            Order buyOrder = OrderFactory.CreateOrder("1234", currencyPair, "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", currencyPair, "market", "sell", 5, 10,
               new StubbedOrderIdGenerator());
            exchange.PlaceNewOrder(buyOrder);
            exchange.PlaceNewOrder(sellOrder);
            _manualResetEvent.WaitOne(5000);
            //TODO:Testcase in progress
        }

    }
}
