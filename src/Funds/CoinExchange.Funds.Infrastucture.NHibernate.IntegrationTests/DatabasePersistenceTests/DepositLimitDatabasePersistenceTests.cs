using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests that actually store Deposit Limit objects in the database and cleanup on tear down
    /// </summary>
    [TestFixture]
    class DepositLimitDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IDepositLimitRepository _depositLimitRepository;

        [SetUp]
        public void Setup()
        {
            _depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
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
        public void SaveDepositLimitAndRetreiveByTierLevelTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            DepositLimit depositLimit = new DepositLimit("tierlevel1", 500, 4000);

            _persistanceRepository.SaveOrUpdate(depositLimit);

            DepositLimit retrievedDepositLimit = _depositLimitRepository.GetDepositLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedDepositLimit);

            Assert.AreEqual(depositLimit.DailyLimit, retrievedDepositLimit.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, retrievedDepositLimit.MonthlyLimit);
        }

        [Test]
        public void SaveDepositLimitAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            DepositLimit depositLimit = new DepositLimit("tierlevel1", 500, 4000);

            _persistanceRepository.SaveOrUpdate(depositLimit);

            DepositLimit retrievedDepositLimit = _depositLimitRepository.GetDepositLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedDepositLimit);
            int id = retrievedDepositLimit.Id;
            _persistanceRepository.SaveOrUpdate(retrievedDepositLimit);

            retrievedDepositLimit = _depositLimitRepository.GetDepositLimitById(id);
            Assert.AreEqual(depositLimit.DailyLimit, retrievedDepositLimit.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, retrievedDepositLimit.MonthlyLimit);
        }
    }
}
