using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.BalanceService;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    public class BalanceQueryServiceTests
    {
        private DatabaseUtility _databaseUtility;
        

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void Teardown()
        {
            _databaseUtility.Create();
        }

        [Test]
        [Category("Integration")]
        public void TestBalanceQueryService_WhenAccountIdIsSupplied_ItShouldReturnListOfBalanceDetails()
        {
            IFundsPersistenceRepository repository =(IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];
            Balance balance=new Balance(new Currency("BTC"),new AccountId(1),1000,1000 );
            Balance balance2 = new Balance(new Currency("LTC"), new AccountId(1), 2000, 2000);
            repository.SaveOrUpdate(balance);
            repository.SaveOrUpdate(balance2);
            IBalanceQueryService _balanceQueryService =(IBalanceQueryService) ContextRegistry.GetContext()["BalanceQueryService"];
            List<BalanceDetails> balanceDetailses=_balanceQueryService.GetBalances(new AccountId(1));
            Assert.NotNull(balanceDetailses);
            Assert.AreEqual(2,balanceDetailses.Count);
            Assert.AreEqual(balanceDetailses[0].Currency,"BTC");
            Assert.AreEqual(balanceDetailses[0].Balance,1000);
            Assert.AreEqual(balanceDetailses[1].Currency, "LTC");
            Assert.AreEqual(balanceDetailses[1].Balance, 2000);
        }
    }
}
