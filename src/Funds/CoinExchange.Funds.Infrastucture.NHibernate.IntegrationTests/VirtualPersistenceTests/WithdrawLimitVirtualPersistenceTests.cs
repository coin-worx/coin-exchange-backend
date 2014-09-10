using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    /// <summary>
    /// Tests that do not actually save the objects in the database, but use the configuration for NHibernate to virtually 
    /// save and retreive objects on the fly
    /// </summary>
    [TestFixture]
    class WithdrawLimitVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawLimitRepository _withdrawLimitRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IWithdrawLimitRepository WithdrawLimitRepository
        {
            set { _withdrawLimitRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
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
