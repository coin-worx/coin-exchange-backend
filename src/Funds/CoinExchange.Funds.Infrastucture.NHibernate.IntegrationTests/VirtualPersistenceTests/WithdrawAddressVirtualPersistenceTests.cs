using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    class WithdrawAddressVirtualPersistenceTests : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawAddressRepository _withdrawAddressRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IWithdrawAddressRepository WithdrawAddressRepository
        {
            set { _withdrawAddressRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveWithdrawAddressesAndRetreiveByAccountIdTest_SavesMultipleObjectsToDatabase_ChecksIfRetreivedOutputIsAsExpected()
        {
            WithdrawAddress withdrawAddress = new WithdrawAddress(new BitcoinAddress("iambitcoin123"), "Description is for dummies",
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(withdrawAddress);

            WithdrawAddress deposit2 = new WithdrawAddress(new BitcoinAddress("321nioctibmai"), "Description is for champs",
                new AccountId("123"));
            Thread.Sleep(500);

            _persistanceRepository.SaveOrUpdate(deposit2);

            List<WithdrawAddress> retrievedWithdrawAddressList = _withdrawAddressRepository.GetWithdrawAddressByAccountId(new AccountId("123"));
            Assert.IsNotNull(retrievedWithdrawAddressList);
            Assert.AreEqual(2, retrievedWithdrawAddressList.Count);

            Assert.AreEqual(withdrawAddress.BitcoinAddress.Value, retrievedWithdrawAddressList[0].BitcoinAddress.Value);
            Assert.AreEqual(withdrawAddress.Description, retrievedWithdrawAddressList[0].Description);
            Assert.AreEqual(withdrawAddress.AccountId.Value, retrievedWithdrawAddressList[0].AccountId.Value);

            Assert.AreEqual(deposit2.BitcoinAddress.Value, retrievedWithdrawAddressList[1].BitcoinAddress.Value);
            Assert.AreEqual(deposit2.Description, retrievedWithdrawAddressList[1].Description);
            Assert.AreEqual(deposit2.AccountId.Value, retrievedWithdrawAddressList[1].AccountId.Value);
        }
    }
}
