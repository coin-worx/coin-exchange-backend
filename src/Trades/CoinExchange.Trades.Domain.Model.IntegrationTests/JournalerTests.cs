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
using Disruptor;
using Newtonsoft.Json;
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
            _eventStore=new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { journaler });
            _manualResetEvent=new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
            InputDisruptorPublisher.Shutdown();
            _eventStore.RemoveAllEvents();
        }

        [Test]
        [Category("Integration")]
        public void CreateOrder_PublishToInputDisruptor_JournalerShouldSaveTheEvent()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            InputPayload payload = InputPayload.CreatePayload(order);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(5000);
            //retrieve order
            Order savedOrder = _eventStore.GetEvent(order.OrderId.Id.ToString()) as Order;
            Assert.NotNull(savedOrder);
            Assert.AreEqual(savedOrder, order);
        }

        [Test]
        [Category("Integration")]
        public void CreateAndLogOrder_GetAllTheOrdersBackFromTheEventStore_CheckIfOrderIsRetreivedProperly()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            InputPayload payload = InputPayload.CreatePayload(order);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(5000);
            //retrieve order
            Order savedOrder = _eventStore.GetEvent(order.OrderId.Id.ToString()) as Order;
            Assert.NotNull(savedOrder);
            Assert.AreEqual(savedOrder, order);
        }
    }
}
