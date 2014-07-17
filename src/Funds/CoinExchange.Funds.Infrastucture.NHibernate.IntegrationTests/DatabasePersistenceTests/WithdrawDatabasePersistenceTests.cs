using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests for Withdrawal persistence by actually saving in the database
    /// </summary>
    [TestFixture]
    class WithdrawDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private IWithdrawRepository _withdrawRepository;

        [SetUp]
        public void Setup()
        {
            _withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
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
        public void SaveWithdrawalAndRetreiveByIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Withdraw withdraw = new Withdraw(new Currency("LTC", true), "1234", DateTime.Now, "New", 2000, 0.005, TransactionStatus.Pending,
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(withdraw);

            Withdraw retrievedDeposit = _withdrawRepository.GetWithdrawByWithdrawId("1234");
            Assert.IsNotNull(retrievedDeposit);
            int id = retrievedDeposit.Id;
            retrievedDeposit.Amount = 777;
            _persistanceRepository.SaveOrUpdate(retrievedDeposit);

            retrievedDeposit = _withdrawRepository.GetWithdrawById(id);
            Assert.AreEqual(withdraw.Currency.Name, retrievedDeposit.Currency.Name);
            Assert.AreEqual(withdraw.WithdrawId, retrievedDeposit.WithdrawId);
            Assert.AreEqual(withdraw.Type, retrievedDeposit.Type);
            Assert.AreEqual(777, retrievedDeposit.Amount);
            Assert.AreEqual(withdraw.Fee, retrievedDeposit.Fee);
            Assert.AreEqual(withdraw.Status, retrievedDeposit.Status);
            Assert.AreEqual(withdraw.AccountId.Value, retrievedDeposit.AccountId.Value);
        }

        [Test]
        public void SaveWithdrawalAndRetreiveByWithdrawIdTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Withdraw withdraw = new Withdraw(new Currency("LTC", true), "1234", DateTime.Now, "New", 2000, 0.005, TransactionStatus.Pending,
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(withdraw);

            Withdraw retrievedDeposit = _withdrawRepository.GetWithdrawByWithdrawId("1234");
            Assert.IsNotNull(retrievedDeposit);
            retrievedDeposit.Amount = 777;
            _persistanceRepository.SaveOrUpdate(retrievedDeposit);

            retrievedDeposit = _withdrawRepository.GetWithdrawByWithdrawId("1234");
            Assert.AreEqual(withdraw.Currency.Name, retrievedDeposit.Currency.Name);
            Assert.AreEqual(withdraw.WithdrawId, retrievedDeposit.WithdrawId);
            Assert.AreEqual(withdraw.Type, retrievedDeposit.Type);
            Assert.AreEqual(777, retrievedDeposit.Amount);
            Assert.AreEqual(withdraw.Fee, retrievedDeposit.Fee);
            Assert.AreEqual(withdraw.Status, retrievedDeposit.Status);
            Assert.AreEqual(withdraw.AccountId.Value, retrievedDeposit.AccountId.Value);
        }

        [Test]
        public void SaveWithdrawalAndRetreiveByCurrencyNameTest_SavesAnObjectToDatabaseAndManipulatesIt_ChecksIfItIsUpdatedAsExpected()
        {
            Withdraw withdraw = new Withdraw(new Currency("LTC", true), "1234", DateTime.Now, "New", 2000, 0.005, TransactionStatus.Pending,
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(withdraw);

            List<Withdraw> retrievedDeposits = _withdrawRepository.GetWithdrawByCurrencyName("LTC");
            Assert.IsNotNull(retrievedDeposits);
            retrievedDeposits[0].Amount = 777;
            _persistanceRepository.SaveOrUpdate(retrievedDeposits[0]);

            retrievedDeposits = _withdrawRepository.GetWithdrawByCurrencyName("LTC");
            Assert.AreEqual(withdraw.Currency.Name, retrievedDeposits[0].Currency.Name);
            Assert.AreEqual(withdraw.WithdrawId, retrievedDeposits[0].WithdrawId);
            Assert.AreEqual(withdraw.Type, retrievedDeposits[0].Type);
            Assert.AreEqual(777, retrievedDeposits[0].Amount);
            Assert.AreEqual(withdraw.Fee, retrievedDeposits[0].Fee);
            Assert.AreEqual(withdraw.Status, retrievedDeposits[0].Status);
            Assert.AreEqual(withdraw.AccountId.Value, retrievedDeposits[0].AccountId.Value);
        }

        [Test]
        public void SaveWithdrawalsAndRetreiveByAccountIdTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Withdraw withdraw = new Withdraw(new Currency("LTC", true), "1234", DateTime.Now, "New", 2000, 0.005, TransactionStatus.Pending,
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(withdraw);

            Withdraw withdraw2 = new Withdraw(new Currency("BTC", true), "123", DateTime.Now, "New", 1000, 0.010, TransactionStatus.Pending,
                new AccountId("123"));
            Thread.Sleep(500);

            _persistanceRepository.SaveOrUpdate(withdraw2);

            List<Withdraw> retrievedDepositList = _withdrawRepository.GetWithdrawByAccountId(new AccountId("123"));
            Assert.IsNotNull(retrievedDepositList);
            Assert.AreEqual(2, retrievedDepositList.Count);

            Assert.AreEqual(withdraw.Currency.Name, retrievedDepositList[0].Currency.Name);
            Assert.AreEqual(withdraw.WithdrawId, retrievedDepositList[0].WithdrawId);
            Assert.AreEqual(withdraw.Type, retrievedDepositList[0].Type);
            Assert.AreEqual(withdraw.Amount, retrievedDepositList[0].Amount);
            Assert.AreEqual(withdraw.Fee, retrievedDepositList[0].Fee);
            Assert.AreEqual(withdraw.Status, retrievedDepositList[0].Status);
            Assert.AreEqual(withdraw.AccountId.Value, retrievedDepositList[0].AccountId.Value);

            Assert.AreEqual(withdraw2.Currency.Name, retrievedDepositList[1].Currency.Name);
            Assert.AreEqual(withdraw2.WithdrawId, retrievedDepositList[1].WithdrawId);
            Assert.AreEqual(withdraw2.Type, retrievedDepositList[1].Type);
            Assert.AreEqual(withdraw2.Amount, retrievedDepositList[1].Amount);
            Assert.AreEqual(withdraw2.Fee, retrievedDepositList[1].Fee);
            Assert.AreEqual(withdraw2.Status, retrievedDepositList[1].Status);
            Assert.AreEqual(withdraw2.AccountId.Value, retrievedDepositList[1].AccountId.Value);
        }
    }
}
