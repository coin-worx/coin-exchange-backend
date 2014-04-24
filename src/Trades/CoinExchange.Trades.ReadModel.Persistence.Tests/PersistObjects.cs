using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class PersistObjects
    {
        private IPersistance _persistance;
        [SetUp]
        public void Setup()
        {
            _persistance = ContextRegistry.GetContext()["Persist"] as IPersistance;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void PersistOrder()
        {
            OrderReadModel model=new OrderReadModel();
            model.OrderId = "1233";
            model.OrderSide = "Buy";
            model.OrderType = "market";
            model.CurrencyPair = "XBTUSD";
            model.Price = 0;
            model.Status = "Closed";
            model.TraderId = "234";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
        }

        [Test]
        public void PersistTrade()
        {
           TradeReadModel model=new TradeReadModel();
            model.CurrencyPair = "XBTUSD";
            model.ExecutionDateTime = DateTime.Now;
            model.OrderId = "123";
            model.TradeId = "123";
            model.Volume = 120;
            model.Price = 100;
            _persistance.SaveOrUpdate(model);
        }
        [Test]
        public void PersistOhlc()
        {
           OhlcReadModel model=new OhlcReadModel();
            model.Close = 10;
            model.DateTime = DateTime.Now;
            model.High = 12;
            model.Low = 10;
            model.Open = 10;
            model.Volume = 23;
            _persistance.SaveOrUpdate(model);
        }

        [Test]
        public void PersistAssetPair()
        {
            AssetPairReadModel model=new AssetPairReadModel();
            model.BaseCurrency = "XBT";
            model.QuoteCurrency = "USD";
            model.CurrencyPair = "XBT/USD";
            model.PairId = "XBTUSD";
            _persistance.SaveOrUpdate(model);
        }
    }
}
