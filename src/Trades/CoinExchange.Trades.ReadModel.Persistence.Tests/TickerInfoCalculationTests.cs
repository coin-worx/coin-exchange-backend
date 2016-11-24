/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
    internal class TickerInfoCalculationTests
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private TradeEventListener _listener=ContextRegistry.GetContext()["TradeEventListener"] as TradeEventListener;
        private IPersistanceRepository _persistance=ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
        private IOhlcRepository _ohlcRepository=ContextRegistry.GetContext()["OhlcRepository"] as IOhlcRepository;
        private ITickerInfoRepository _tickerInfoRepository = ContextRegistry.GetContext()["TickerInfoRepository"] as ITickerInfoRepository;
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
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {journaler});
            _manualResetEvent = new ManualResetEvent(false);
            //  _listener = new TradeEventListener(_persistance);
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
            Assert.AreEqual(model.Last24HourVolumeWeight, 10.88889m);
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

