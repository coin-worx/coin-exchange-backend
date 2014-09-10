using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests by actually persisting the Fee objects to the database
    /// </summary>
    [TestFixture]
    class FeeDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IFeeRepository _feeRepository;

        [SetUp]
        public void Setup()
        {
            _feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];
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
        public void SaveFeeAndRetreiveByCurrencyPairNameTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Fee fee = new Fee("LTC/BTC", 4000, 500);

            _persistanceRepository.SaveOrUpdate(fee);

            Fee retrievedFee = _feeRepository.GetFeeByCurrencyPair("LTC/BTC");
            Assert.IsNotNull(retrievedFee);

            Assert.AreEqual(fee.PercentageFee, retrievedFee.PercentageFee);
            Assert.AreEqual(fee.Amount, retrievedFee.Amount);
        }

        [Test]
        public void SaveFeeAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Fee fee = new Fee("LTC/BTC", 4000, 500);

            _persistanceRepository.SaveOrUpdate(fee);

            Fee retrievedFee = _feeRepository.GetFeeByCurrencyPair("LTC/BTC");
            Assert.IsNotNull(retrievedFee);
            int id = retrievedFee.Id;
            _persistanceRepository.SaveOrUpdate(retrievedFee);

            retrievedFee = _feeRepository.GetFeeById(id);
            Assert.AreEqual(fee.PercentageFee, retrievedFee.PercentageFee);
            Assert.AreEqual(fee.Amount, retrievedFee.Amount);
        }
    }
}
