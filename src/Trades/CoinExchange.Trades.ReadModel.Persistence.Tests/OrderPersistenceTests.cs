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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate;
using NHibernate.Criterion;
using NUnit.Framework;
using Spring.Context.Support;
using Order = CoinExchange.Trades.Domain.Model.OrderAggregate.Order;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderPersistenceTests:AbstractConfiguration
    {
        private IPersistanceRepository _persistanceRepository;
        private IOrderRepository _orderRepository;

        /// <summary>
        /// Injected dependency to OrderRepository
        /// </summary>
        public IOrderRepository OrderRepository
        {
            set { _orderRepository = value; }
        }

        /// <summary>
        /// Injected dependency to Persistance Repository
        /// </summary>
        public IPersistanceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }
        
        [Test]
        public void SaveOrderReadModel_IfSaveOrUpdateMethodIsCalled_ItShouldGetSavedInTheDatabase()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            string id = DateTime.Now.ToString();
            order.OrderState=OrderState.Complete;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel getReadModel = _orderRepository.GetOrderById(order.OrderId.Id.ToString());
            Assert.NotNull(getReadModel);
            AssertAreEqual(getReadModel, model);
            Assert.NotNull(getReadModel.ClosingDateTime);
        }

        [Test]
        public void SaveOrderReadModel_IfStateIsAccepted_ClosingTimeShouldBeNull()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            string id = DateTime.Now.ToString();
            order.OrderState = OrderState.Accepted;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel getReadModel = _orderRepository.GetOrderById(order.OrderId.Id.ToString());
            Assert.NotNull(getReadModel);
            AssertAreEqual(getReadModel, model);
            Assert.Null(getReadModel.ClosingDateTime);
        }
        [Test]
        public void GetOpenOrders_IfTraderIdIsProvided_ItShouldRetireveAllOpenOrdersOfTrader()
        {
            Order order = OrderFactory.CreateOrder("999", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            Order order1 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            string id = DateTime.Now.ToString();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel model1 = ReadModelAdapter.GetOrderReadModel(order1);
            _persistanceRepository.SaveOrUpdate(model1);
            IList<OrderReadModel> getReadModel = _orderRepository.GetOpenOrders("999");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count,1);
            AssertAreEqual(getReadModel[0],model);
        }

        [Test]
        public void GetOpenOrders_IfTraderIdIsProvided_RetrievedOrdersShouldBeDateTimeSortedAsDescending()
        {
            ManualResetEvent resetEvent=new ManualResetEvent(false);
            Order order = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Order order1 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Order order2 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel model1 = ReadModelAdapter.GetOrderReadModel(order1);
            _persistanceRepository.SaveOrUpdate(model1);
            OrderReadModel model2 = ReadModelAdapter.GetOrderReadModel(order2);
            _persistanceRepository.SaveOrUpdate(model2);
            IList<OrderReadModel> getReadModel = _orderRepository.GetOpenOrders("100");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count, 3);
            Assert.True(getReadModel[0].DateTime>getReadModel[1].DateTime);
            Assert.True(getReadModel[1].DateTime > getReadModel[2].DateTime);
        }

        [Test]
        public void GetClosedOrders_IfTraderIdIsProvided_ItShouldRetireveAllClosedOrdersOfTrader()
        {
            Order order = OrderFactory.CreateOrder("999", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState=OrderState.Complete;
            string id = DateTime.Now.ToString();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetClosedOrders("999");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count, 1);
            AssertAreEqual(getReadModel[0], model);
        }

        [Test]
        public void GetClosedOrders_IfTraderIdIsProvided_RetrievedOrdersShouldBeDateTimeSortedAsDescending()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            Order order = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState=OrderState.Complete;
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Order order1 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order1.OrderState = OrderState.Complete;
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Order order2 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order2.OrderState = OrderState.Complete;
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel model1 = ReadModelAdapter.GetOrderReadModel(order1);
            _persistanceRepository.SaveOrUpdate(model1);
            OrderReadModel model2 = ReadModelAdapter.GetOrderReadModel(order2);
            _persistanceRepository.SaveOrUpdate(model2);
            IList<OrderReadModel> getReadModel = _orderRepository.GetClosedOrders("100");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count, 3);
            Assert.True(getReadModel[0].DateTime > getReadModel[1].DateTime);
            Assert.True(getReadModel[1].DateTime > getReadModel[2].DateTime);
        }


        private void AssertAreEqual(OrderReadModel expected, OrderReadModel actual)
        {
            Assert.AreEqual(expected.OrderId, actual.OrderId);
            Assert.AreEqual(expected.Side, actual.Side);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.TraderId, actual.TraderId);
            Assert.AreEqual(expected.VolumeExecuted, actual.VolumeExecuted);
            Assert.AreEqual(expected.CurrencyPair, actual.CurrencyPair);
        }
    }
}
