using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
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
            //Assert.NotNull(getTrades);
            //bool check = false;
            //for (int i = 0; i < getTrades.Count; i++)
            //{
            //    if (!(getTrades[i].BuyTraderId.Equals("999") || getTrades[i].SellTraderId.Equals("1234")))
            //    {
            //        check = false;
            //        break;
            //    }
            //    check = true;
            //}
            //Assert.AreEqual(check, true);
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
