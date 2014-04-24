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
    public class TradePersistenceTestCases
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
            Assert.AreEqual(getSavedModel.TradeId,id);
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
            model.TraderId = "TestTrader";
            model.Volume = 120;
            model.Price = 100;
            _persistance.SaveOrUpdate(model);
            model.TradeId = DateTime.Now.Millisecond.ToString();
            IList<TradeReadModel> getTrades = _tradeRepository.GetTraderTradeHistory("TestTrader");
            Assert.NotNull(getTrades);
            bool check = true;
            for (int i = 0; i < getTrades.Count; i++)
            {
                if (!getTrades[i].TraderId.Equals("TestTrader"))
                {
                    check = false;
                }
            }
            Assert.AreEqual(check,true);
        }
        
    }
}
