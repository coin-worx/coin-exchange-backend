using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
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
    class WithdrawFeesVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawFeesRepository _withdrawFeesRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IWithdrawFeesRepository WithdrawFeesRepository
        {
            set { _withdrawFeesRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
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
