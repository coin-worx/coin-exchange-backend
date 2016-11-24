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
using System.Diagnostics;
using System.Threading;
using CoinExchange.Common.Tests;
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
using System.Configuration;
using Constants = CoinExchange.Common.Domain.Model.Constants;
namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderListenerTests//:AbstractConfiguration
    {
        private ManualResetEvent _manualResetEvent;
        private IEventStore _eventStore;
        private IBalanceValidationService _balanceValidationService = (IBalanceValidationService)ContextRegistry.GetContext()["BalanceValidationService"];
        private OrderEventListener _listener;
        private IPersistanceRepository _persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;

        public IOrderRepository OrderRepository
        {
            set { _orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository; }
        }

        public IPersistanceRepository Persistance
        {
            set { _persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository; }
        }

        public IBalanceValidationService BalanceValidationService
        {
            set { _balanceValidationService = (IBalanceValidationService)ContextRegistry.GetContext()["BalanceValidationService"]; }
        }

        private ISessionFactory sessionFactory;
        private IOrderRepository _orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository;


        public ISessionFactory SessionFactory
        {
            set { sessionFactory = value; }
        }

        private DatabaseUtility _databaseUtility;
        [SetUp]
        public new void SetUp()
        {
            BeforeSetup();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility=new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            
            //initialize journaler
            _eventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler journaler = new Journaler(_eventStore);
            //assign journaler to disruptor as its consumer
            OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { journaler });
            _manualResetEvent = new ManualResetEvent(false);
            _listener = new OrderEventListener(_persistance, _balanceValidationService);
            AfterSetup();
        }

        [TearDown]
        public new void TearDown()
        {
            OutputDisruptor.ShutDown();
            _databaseUtility.Create();
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
            Assert.AreEqual(receivedOrder.Side, order.OrderSide.ToString());
            Assert.AreEqual(receivedOrder.Type, order.OrderType.ToString());
            Assert.AreEqual(receivedOrder.Price, order.Price.Value);
            Assert.AreEqual(receivedOrder.CurrencyPair, order.CurrencyPair);
       }

        protected virtual void BeforeSetup() { }
        protected virtual void AfterSetup() { }
        protected virtual void BeforeTearDown() { }
        protected virtual void AfterTearDown() { }
      }
}
