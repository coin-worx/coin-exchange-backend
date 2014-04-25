using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class TradePersistenceTests
    {
        private IPersistanceRepository _persistance;
        private ITradeRepository _tradeRepository;
        
        [SetUp]
        public void Setup()
        {
            _persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            _tradeRepository = ContextRegistry.GetContext()["TradeRepository"] as ITradeRepository;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void SaveTradeReadModel_IfSaveOrUpdateMethodIsCalled_ItShouldGetSavedInTheDatabase()
        {
            string id = DateTime.Now.ToString();
            TradeReadModel model = new TradeReadModel();
            model.CurrencyPair = "XBTUSD";
            model.ExecutionDateTime = DateTime.Now;
            model.OrderId = "123";
            model.TradeId = id;
            model.TraderId = "123";
            model.Volume = 120;
            model.Price = 100;
            _persistance.SaveOrUpdate(model);
            TradeReadModel getSavedModel = _tradeRepository.GetById(id);
            Assert.NotNull(getSavedModel);
            AssertAreEqual(getSavedModel,model);
        }

        [Test]
        public void GetRecentTrades_IfRecentTradesRequestArrives_ItShouldReturnRecentTrades()
        {
            string id = DateTime.Now.ToString();
            TradeReadModel model = new TradeReadModel();
            model.CurrencyPair = "XBTUSD";
            model.ExecutionDateTime = DateTime.Now;
            model.OrderId = "123";
            model.TradeId = id;
            model.TraderId = "123";
            model.Volume = 120;
            model.Price = 100;
            _persistance.SaveOrUpdate(model);
            IList<object> getTrades = _tradeRepository.GetRecentTrades("", "XBTUSD");
            Assert.NotNull(getTrades);
        }

        [Test]
        public void GetTradesOfTrader_IfTraderIdIsProvided_AllTradesOfTraderShouldReturn()
        {
            string id = DateTime.Now.Millisecond.ToString();
            TradeReadModel model = new TradeReadModel();
            model.CurrencyPair = "XBTUSD";
            model.ExecutionDateTime = DateTime.Now;
            model.OrderId = "123";
            model.TradeId = id;
            model.TraderId = "999";
            model.Volume = 120;
            model.Price = 100;
            _persistance.SaveOrUpdate(model);
            model.TradeId = DateTime.Now.Millisecond.ToString();
            IList<TradeReadModel> getTrades = _tradeRepository.GetTraderTradeHistory("999");
            Assert.NotNull(getTrades);
            bool check = true;
            for (int i = 0; i < getTrades.Count; i++)
            {
                if (!getTrades[i].TraderId.Equals("999"))
                {
                    check = false;
                }
            }
            Assert.AreEqual(check,true);
        }

        private void AssertAreEqual(TradeReadModel expected, TradeReadModel actual)
        {
            Assert.AreEqual(expected.TradeId,actual.TradeId);
            Assert.AreEqual(expected.TraderId, actual.TraderId);
            Assert.AreEqual(expected.Volume, actual.Volume);
            Assert.AreEqual(expected.OrderId, actual.OrderId);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.ExecutionDateTime, actual.ExecutionDateTime);
            Assert.AreEqual(expected.CurrencyPair, actual.CurrencyPair);

        }
    }
}
