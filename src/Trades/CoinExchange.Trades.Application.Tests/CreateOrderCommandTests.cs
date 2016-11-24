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
using System.Threading;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using Disruptor;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.Tests
{
    [TestFixture]
    public class CreateOrderCommandTests:IEventHandler<InputPayload>
    {
        private IOrderApplicationService _orderseService;
        private ManualResetEvent _manualResetEvent;
        private Order _receivedOrder;
        
        [SetUp]
        public void SetUp()
        {
            _orderseService = ContextRegistry.GetContext()["OrderApplicationService"] as IOrderApplicationService;
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[]{this});
            _manualResetEvent=new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
            InputDisruptorPublisher.Shutdown();
        }

        [Test]
        public void CreateMarketOrder_SendCreateOrderCommand_ReturnNewOrderRepresentationAndPublishOrderToDisruptor()
        {
            NewOrderRepresentation representation=_orderseService.CreateOrder(new CreateOrderCommand(0, "market", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
            Assert.AreEqual(representation.OrderId,_receivedOrder.OrderId.Id.ToString());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateOrder_SendCreateOrderCommandWithZeroVolume_ThrowException()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(0, "market", "buy", "XBTUSD", 0, "1234"));
            _manualResetEvent.WaitOne(3000);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateOrder_SendCreateOrderCommandWithInvalidOrderType_ThrowException()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(0, "mar", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
        }

        [Test]
        public void CreateLimitOrder_SendCreateOrderCommand_ReturnNewOrderRepresentationAndPublishOrderToDisruptor()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(10, "limit", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
            Assert.AreEqual(representation.OrderId, _receivedOrder.OrderId.Id.ToString());
        }

        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            if (data.IsOrder)
            {
                _receivedOrder = data.Order;
                _manualResetEvent.Set();
            }
        }
    }
}
