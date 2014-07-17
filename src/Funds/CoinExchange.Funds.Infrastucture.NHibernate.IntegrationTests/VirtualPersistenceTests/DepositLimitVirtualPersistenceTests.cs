using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    /// <summary>
    /// Tests that do not actually save the objects in the database, but use the configuration for NHibernate to virtually 
    /// save and retreive objects on the fly
    /// </summary>
    [TestFixture]
    class DepositLimitVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IDepositLimitRepository _depositLimitRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IDepositLimitRepository DepositLimitRepository
        {
            set { _depositLimitRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveDepositLimitAndRetreiveByTierLevelTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            DepositLimit depositLimit = new DepositLimit("tierlevel1", 4000, 500);

            _persistanceRepository.SaveOrUpdate(depositLimit);

            DepositLimit retrievedDepositLimit = _depositLimitRepository.GetDepositLimitByTierLevel("tierlevel1");
            Assert.IsNotNull(retrievedDepositLimit);

            Assert.AreEqual(depositLimit.DailyLimit, retrievedDepositLimit.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, retrievedDepositLimit.MonthlyLimit);
        }

        [Test]
        public void SaveDepositLimitAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            DepositLimit depositLimit = new DepositLimit("tierlevel1", 4000, 500);

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
