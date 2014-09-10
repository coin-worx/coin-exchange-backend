using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using NUnit.Framework;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class CurrencyPairRepositoryTests:AbstractConfiguration
    {
        private ICurrencyPairRepository _currencyPairRepository;

        public ICurrencyPairRepository CurrencyPairRepository
        {
            set { _currencyPairRepository = value; }
        }
        private DatabaseUtility _databaseUtility;
        [Test]
        [Category("Integration")]
        public void GetCurrencyPair_IfGetByIdIsCalled_CurrencyPairShouldRetrieveFromDatabase()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
            CurrencyPair pair = _currencyPairRepository.GetById("XBTUSD");
            Assert.NotNull(pair);
            Assert.AreEqual(pair.CurrencyPairName,"XBTUSD");
            Assert.AreEqual(pair.BaseCurrency,"XBT");
            Assert.AreEqual(pair.QuoteCurrency,"USD");
        }
    }
}
