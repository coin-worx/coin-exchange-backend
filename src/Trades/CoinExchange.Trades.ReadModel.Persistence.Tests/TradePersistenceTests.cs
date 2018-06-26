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
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class TradePersistenceTests:AbstractConfiguration
    {
        private IPersistanceRepository _persistanceRepository;
        private ITradeRepository _tradeRepository;

        /// <summary>
        /// Depenedency Injected to TradeRepository
        /// </summary>
        public ITradeRepository TradeRepository
        {
            set { _tradeRepository = value; }
        }

        /// <summary>
        /// Depenedency Injected to PersistanceRepository
        /// </summary>
        public IPersistanceRepository PersistanceRepository
        {
            set { _persistanceRepository = value; }
        }


        [Test]
        public void SaveTradeReadModel_IfSaveOrUpdateMethodIsCalled_ItShouldGetSavedInTheDatabase()
        {
            Order buyOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
               new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
               new StubbedOrderIdGenerator());
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            TradeReadModel model = ReadModelAdapter.GetTradeReadModel(trade);
            _persistanceRepository.SaveOrUpdate(model);
            TradeReadModel getSavedModel = _tradeRepository.GetById(trade.TradeId.Id.ToString());
            Assert.NotNull(getSavedModel);
            AssertAreEqual(getSavedModel, model);
        }

        [Test]
        public void GetRecentTrades_IfRecentTradesRequestArrives_ItShouldReturnRecentTrades()
        {
            Order buyOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
               new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
               new StubbedOrderIdGenerator());
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            TradeReadModel model = ReadModelAdapter.GetTradeReadModel(trade);
            _persistanceRepository.SaveOrUpdate(model);
            IList<object> getTrades = _tradeRepository.GetRecentTrades("", "XBTUSD");
            Assert.NotNull(getTrades);
            Assert.AreEqual(getTrades.Count, 1);
            object[] received = getTrades[0] as object[];
            Assert.AreEqual(received[1], 100);
            Assert.AreEqual(received[2], 10);
        }

        [Test]
        public void GetRecentTrades_IfRecentTradesRequestArrives_TradesShouldBeSortedbyDateTimeDesc()
        {
            Order buyOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
               new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
               new StubbedOrderIdGenerator());
            ManualResetEvent resetEvent=new ManualResetEvent(false);
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Trade trade1 = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Trade trade2 = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            TradeReadModel model = ReadModelAdapter.GetTradeReadModel(trade);
            _persistanceRepository.SaveOrUpdate(model);
            TradeReadModel model1 = ReadModelAdapter.GetTradeReadModel(trade1);
            _persistanceRepository.SaveOrUpdate(model1);
            TradeReadModel model2 = ReadModelAdapter.GetTradeReadModel(trade2);
            _persistanceRepository.SaveOrUpdate(model2);
            IList<object> getTrades = _tradeRepository.GetRecentTrades("", "XBTUSD");
            Assert.NotNull(getTrades);
            Assert.AreEqual(getTrades.Count, 3);
            object[] received = getTrades[0] as object[];
            object[] received1 = getTrades[1] as object[];
            object[] received2 = getTrades[2] as object[];
            Assert.True(Convert.ToDateTime(received[0])> Convert.ToDateTime(received1[0]));
            Assert.True(Convert.ToDateTime(received1[0]) > Convert.ToDateTime(received2[0]));
        }

        [Test]
        public void GetTradesOfTrader_IfTraderIdIsProvided_AllTradesOfTraderShouldReturn()
        {
            Order buyOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
               new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
               new StubbedOrderIdGenerator());
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            TradeReadModel model = ReadModelAdapter.GetTradeReadModel(trade);
            _persistanceRepository.SaveOrUpdate(model);
            //model.TradeId = DateTime.Now.Millisecond.ToString();
            var getTrades = _tradeRepository.GetTraderTradeHistory("1234");
            Assert.NotNull(getTrades);
            Assert.AreEqual(getTrades.Count,1);
            object[] received = getTrades[0] as object[];
            Assert.AreEqual(received[2],100);
            Assert.AreEqual(received[3],10);
            Assert.AreEqual(received[4],"XBTUSD");
        }

        [Test]
        public void GetTradesOfTrader_IfTraderIdIsProvided_AllTradesOfTraderShouldBeDateTimeSortedDesc()
        {
            Order buyOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
                new StubbedOrderIdGenerator());
            Order sellOrder = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
               new StubbedOrderIdGenerator());
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            //Trade trade=new Trade("XBTUSD",new Price(100),new Volume(10),DateTime.Now,buyOrder,sellOrder);
            Trade trade = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Trade trade1 = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            Trade trade2 = TradeFactory.GenerateTrade("XBTUSD", new Price(100), new Volume(10), buyOrder, sellOrder);
            resetEvent.WaitOne(2000);
            resetEvent.Reset();
            TradeReadModel model = ReadModelAdapter.GetTradeReadModel(trade);
            _persistanceRepository.SaveOrUpdate(model);
            TradeReadModel model1 = ReadModelAdapter.GetTradeReadModel(trade1);
            _persistanceRepository.SaveOrUpdate(model1);
            TradeReadModel model2 = ReadModelAdapter.GetTradeReadModel(trade2);
            _persistanceRepository.SaveOrUpdate(model2);
            IList<object> getTrades = _tradeRepository.GetTraderTradeHistory("1234");
            Assert.NotNull(getTrades);
            Assert.AreEqual(getTrades.Count, 3);
            object[] received = getTrades[0] as object[];
            object[] received1 = getTrades[1] as object[];
            object[] received2 = getTrades[2] as object[];
            Assert.True(Convert.ToDateTime(received[1]) > Convert.ToDateTime(received1[1]));
            Assert.True(Convert.ToDateTime(received1[1]) > Convert.ToDateTime(received2[1]));
        }
        private void AssertAreEqual(TradeReadModel expected, TradeReadModel actual)
        {
            Assert.AreEqual(expected.TradeId, actual.TradeId);
            Assert.AreEqual(expected.BuyTraderId, actual.BuyTraderId);
            Assert.AreEqual(expected.SellTraderId, actual.SellTraderId);
            Assert.AreEqual(expected.Volume, actual.Volume);
            Assert.AreEqual(expected.BuyOrderId, actual.BuyOrderId);
            Assert.AreEqual(expected.SellOrderId, actual.SellOrderId);
            Assert.AreEqual(expected.Price, actual.Price);
            //Assert.AreEqual(expected.ExecutionDateTime, actual.ExecutionDateTime);
            Assert.AreEqual(expected.CurrencyPair, actual.CurrencyPair);
        }
    }
}
