using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests that actaully persist data in the database and cleanup on tear down
    /// </summary>
    [TestFixture]
    class WithdrawLimitDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawLimitRepository _withdrawLimitRepository;

        [SetUp]
        public void Setup()
        {
            _withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
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
        public void SaveWithdrawLimitAndRetreiveByTierLevelTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawLimit withdrawLimit = new WithdrawLimit("tierlevel1", 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawLimit);

            WithdrawLimit retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedWithdrawLimit);

            Assert.AreEqual(withdrawLimit.DailyLimit, retrievedWithdrawLimit.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, retrievedWithdrawLimit.MonthlyLimit);
        }

        [Test]
        public void SaveWithdrawLimitAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            WithdrawLimit withdrawLimit = new WithdrawLimit("tierlevel1", 4000, 500);

            _persistanceRepository.SaveOrUpdate(withdrawLimit);

            WithdrawLimit retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedWithdrawLimit);
            int id = retrievedWithdrawLimit.Id;
            _persistanceRepository.SaveOrUpdate(retrievedWithdrawLimit);

            retrievedWithdrawLimit = _withdrawLimitRepository.GetWithdrawLimitById(id);
            Assert.AreEqual(withdrawLimit.DailyLimit, retrievedWithdrawLimit.DailyLimit);
            Assert.AreEqual(withdrawLimit.MonthlyLimit, retrievedWithdrawLimit.MonthlyLimit);
        }
    }
}
