using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests for actual persistence in database for Deposit address objects
    /// </summary>
    [TestFixture]
    class DepositAddressDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IDepositAddressRepository _depositAddressRepository;

        [SetUp]
        public void Setup()
        {
            _depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
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
        public void SaveDepositAddressesAndRetreiveByAccountIdTest_SavesObjectsToDatabase_ChecksIfTheyAreAsExpected()
        {
            DepositAddress deposit = new DepositAddress(new BitcoinAddress("iambitcoin123"), AddressStatus.New, DateTime.Now,
                new AccountId(1));

            _persistanceRepository.SaveOrUpdate(deposit);

            DepositAddress deposit2 = new DepositAddress(new BitcoinAddress("iambitcoin123"), AddressStatus.New, DateTime.Now,
                new AccountId(1));
            Thread.Sleep(500);

            _persistanceRepository.SaveOrUpdate(deposit2);

            List<DepositAddress> retrievedDepositAddressList = _depositAddressRepository.GetDepositAddressByAccountId(new AccountId(1));
            Assert.IsNotNull(retrievedDepositAddressList);
            Assert.AreEqual(2, retrievedDepositAddressList.Count);

            Assert.AreEqual(deposit.BitcoinAddress.Value, retrievedDepositAddressList[0].BitcoinAddress.Value);
            Assert.AreEqual(deposit.Status, retrievedDepositAddressList[0].Status);
            Assert.AreEqual(deposit.AccountId.Value, retrievedDepositAddressList[0].AccountId.Value);

            Assert.AreEqual(deposit.BitcoinAddress.Value, retrievedDepositAddressList[1].BitcoinAddress.Value);
            Assert.AreEqual(deposit2.Status, retrievedDepositAddressList[1].Status);
            Assert.AreEqual(deposit2.AccountId.Value, retrievedDepositAddressList[1].AccountId.Value);
        }
    }
}
