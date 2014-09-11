using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    /// <summary>
    /// Tests that do not actually save the objects in the database, but use the configuration for NHibernate to virtually 
    /// save and retreive objects on the fly
    /// </summary>
    [TestFixture]
    class FeeVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IFeeRepository _feeRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFeeRepository FeeRepository
        {
            set { _feeRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveFeeAndRetreiveByCurrencyPairNameTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Fee fee = new Fee("LTC/BTC", 4000, 500);

            _persistanceRepository.SaveOrUpdate(fee);

            List<Fee> retrievedFee = _feeRepository.GetFeeByCurrencyPair("LTC/BTC");
            Assert.IsNotNull(retrievedFee);

            Assert.AreEqual(fee.PercentageFee, retrievedFee[0].PercentageFee);
            Assert.AreEqual(fee.Amount, retrievedFee[0].Amount);
        }

        [Test]
        public void SaveFeeAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Fee fee = new Fee("LTC/BTC", 4000, 500);

            _persistanceRepository.SaveOrUpdate(fee);

            List<Fee> retrievedFee = _feeRepository.GetFeeByCurrencyPair("LTC/BTC");
            Assert.IsNotNull(retrievedFee);
            int id = retrievedFee[0].Id;
            _persistanceRepository.SaveOrUpdate(retrievedFee);

            fee = _feeRepository.GetFeeById(id);
            Assert.AreEqual(fee.PercentageFee, fee.PercentageFee);
            Assert.AreEqual(fee.Amount, fee.Amount);
        }
    }
}
