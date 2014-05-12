using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Test]
        [Category("Integration")]
        public void Test()
        {
            CurrencyPair pair = _currencyPairRepository.GetById("BTC/USD");
            Assert.NotNull(pair);
            Assert.AreEqual(pair.CurrencyPairName,"BTC/USD");
            Assert.AreEqual(pair.BaseCurrency,"USD");
            Assert.AreEqual(pair.QuoteCurrency,"BTC");
        }
    }
}
