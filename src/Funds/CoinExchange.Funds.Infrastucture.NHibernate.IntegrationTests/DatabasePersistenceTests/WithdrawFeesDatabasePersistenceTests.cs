using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests for actually persisting WithdrawFees objects in the database
    /// </summary>
    [TestFixture]
    class WithdrawFeesDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawFeesRepository _withdrawFeesRepository;

        [SetUp]
        public void Setup()
        {
            _withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            _persistanceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
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
        public void SaveWithdrawFeesAndRetreiveByCurrencyNameTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawFees withdrawFees = new WithdrawFees(new Currency("LTC", true), 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawFees);

            WithdrawFees retrievedWithdrawFees = _withdrawFeesRepository.GetWithdrawFeesByCurrencyName("LTC");
            Assert.IsNotNull(retrievedWithdrawFees);

            Assert.AreEqual(withdrawFees.MinimumAmount, retrievedWithdrawFees.MinimumAmount);
            Assert.AreEqual(withdrawFees.Fee, retrievedWithdrawFees.Fee);
        }

        [Test]
        public void SaveWithdrawFeesAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawFees withdrawFees = new WithdrawFees(new Currency("LTC", true), 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawFees);

            WithdrawFees retrievedWithdrawFees = _withdrawFeesRepository.GetWithdrawFeesByCurrencyName("LTC");
            Assert.IsNotNull(retrievedWithdrawFees);
            int id = retrievedWithdrawFees.Id;
            _persistanceRepository.SaveOrUpdate(retrievedWithdrawFees);

            retrievedWithdrawFees = _withdrawFeesRepository.GetWithdrawFeesById(id);
            Assert.AreEqual(withdrawFees.MinimumAmount, retrievedWithdrawFees.MinimumAmount);
            Assert.AreEqual(withdrawFees.Fee, retrievedWithdrawFees.Fee);
        }
    }
}
