using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.DatabasePersistenceTests
{
    /// <summary>
    /// Tests for the actual Ledger objects persistence in the database
    /// </summary>
    [TestFixture]
    class LedgerDatabasePersistenceTests
    {
        private DatabaseUtility _databaseUtility;
        private IFundsPersistenceRepository _persistanceRepository;
        private ILedgerRepository _ledgerRepository;

        [SetUp]
        public void Setup()
        {
            _ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
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
        public void SaveLedgerAndRetreiveByIdTest_SavesAnObjectToDatabaseAndGetsThePrimaryKeyIdForDatabase_ChecksIfTheOutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, "trade123", "order123", null, null, new AccountId(1));

            _persistanceRepository.SaveOrUpdate(ledger);

            Ledger retrievedLedger = _ledgerRepository.GetLedgerByLedgerId("1234");
            Assert.IsNotNull(retrievedLedger);
            int id = retrievedLedger.Id;
            _persistanceRepository.SaveOrUpdate(retrievedLedger);

            retrievedLedger = _ledgerRepository.GetLedgerById(id);
            Assert.AreEqual(ledger.Currency.Name, retrievedLedger.Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedger.LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedger.LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedger.Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedger.Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedger.Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedger.TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedger.OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedger.AccountId.Value);

            Assert.IsNull(retrievedLedger.WithdrawId);
            Assert.IsNull(retrievedLedger.DepositId);
        }

        [Test]
        public void SaveLedgersAndRetreiveByCurrencyNameTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, "trade123", "order123", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, "trade1234", "order1234", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);

            // Ledger 1 Details
            List<Ledger> retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedgers[0].AccountId.Value);

            // Ledger 2 Details
            retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger2.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedgers[0].AccountId.Value);
        }

        [Test]
        public void SaveLedgersAndRetreiveByTradeIdTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, "trade123", "order123", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, "trade123", "order1234", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);

            // Ledger 1 Details
            List<Ledger> retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedgers[0].AccountId.Value);

            // Ledger 2 Details
            retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger2.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedgers[0].AccountId.Value);
        }

        [Test]
        public void SaveLedgersAndRetreiveByOrderIdTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, "trade1234", "order123", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, "trade123", "order1234", null, null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);

            // Ledger 1 Details
            List<Ledger> retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedgers[0].AccountId.Value);

            // Ledger 2 Details
            retrievedLedgers = _ledgerRepository.GetLedgersByOrderId(ledger2.OrderId);
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(1, retrievedLedgers.Count);

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedgers[0].AccountId.Value);
        }

        [Test]
        public void SaveLedgerAndRetreiveByWithdrawIdTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, null, null, "withdraw123", null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, null, null, "withdraw1234", null, new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);

            Ledger retrievedLedger1 = _ledgerRepository.GetLedgersByWithdrawId("withdraw123");
            Assert.IsNotNull(retrievedLedger1);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedger1.Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedger1.LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedger1.LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedger1.Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedger1.Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedger1.Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedger1.TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedger1.OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedger1.AccountId.Value);

            Ledger retrievedLedger2 = _ledgerRepository.GetLedgersByWithdrawId("withdraw1234");

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedger2.Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedger2.LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedger2.LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedger2.Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedger2.Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedger2.Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedger2.TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedger2.OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedger2.AccountId.Value);
        }

        [Test]
        public void SaveLedgersAndRetreiveByDepositIdTest_SavesMultipleObjectInDatabase_ChecksIfTheoutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 1000.543m,
                0.005m, 23000, null, null, null, "deposit123", new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, null, null, null, "deposit1234", new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);

            Ledger retrievedLedger1 = _ledgerRepository.GetLedgersByDepositId("deposit123");
            Assert.IsNotNull(retrievedLedger1);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedger1.Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedger1.LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedger1.LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedger1.Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedger1.Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedger1.Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedger1.TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedger1.OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedger1.AccountId.Value);

            Ledger retrievedLedger2 = _ledgerRepository.GetLedgersByDepositId("deposit1234");
            Assert.IsNotNull(retrievedLedger2);

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedger2.Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedger2.LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedger2.LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedger2.Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedger2.Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedger2.Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedger2.TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedger2.OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedger2.AccountId.Value);
        }

        [Test]
        public void SaveLedgersAndGetAllLedgersTest_SavesMultipleObjectInDatabase_ChecksIfTheOutputIsAsExpected()
        {
            Ledger ledger = new Ledger("1234", DateTime.Now, LedgerType.Trade, new Currency("XBT", true), 1000.543m,
                0.005m, 23000, null, null, null, "deposit123", new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger);
            Thread.Sleep(1000);
            Ledger ledger2 = new Ledger("12345", DateTime.Now, LedgerType.Trade, new Currency("LTC", true), 2000.543m,
                0.005m, 3000, null, null, null, "deposit123", new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger2);
            Thread.Sleep(1000);
            Ledger ledger3 = new Ledger("123456", DateTime.Now, LedgerType.Trade, new Currency("BTC", true), 3000.543m,
                0.005m, 3000, null, null, null, "deposit123", new AccountId(1));
            _persistanceRepository.SaveOrUpdate(ledger3);

            // Retreives the list in descending order of time
            IList<Ledger> retrievedLedgers = _ledgerRepository.GetAllLedgers();
            Assert.IsNotNull(retrievedLedgers);
            Assert.AreEqual(3, retrievedLedgers.Count);

            Assert.AreEqual(ledger.Currency.Name, retrievedLedgers[0].Currency.Name);
            Assert.AreEqual(ledger.LedgerId, retrievedLedgers[0].LedgerId);
            Assert.AreEqual(ledger.LedgerType, retrievedLedgers[0].LedgerType);
            Assert.AreEqual(ledger.Amount, retrievedLedgers[0].Amount);
            Assert.AreEqual(ledger.Fee, retrievedLedgers[0].Fee);
            Assert.AreEqual(ledger.Balance, retrievedLedgers[0].Balance);
            Assert.AreEqual(ledger.TradeId, retrievedLedgers[0].TradeId);
            Assert.AreEqual(ledger.OrderId, retrievedLedgers[0].OrderId);
            Assert.AreEqual(ledger.AccountId.Value, retrievedLedgers[0].AccountId.Value);

            Assert.AreEqual(ledger2.Currency.Name, retrievedLedgers[1].Currency.Name);
            Assert.AreEqual(ledger2.LedgerId, retrievedLedgers[1].LedgerId);
            Assert.AreEqual(ledger2.LedgerType, retrievedLedgers[1].LedgerType);
            Assert.AreEqual(ledger2.Amount, retrievedLedgers[1].Amount);
            Assert.AreEqual(ledger2.Fee, retrievedLedgers[1].Fee);
            Assert.AreEqual(ledger2.Balance, retrievedLedgers[1].Balance);
            Assert.AreEqual(ledger2.TradeId, retrievedLedgers[1].TradeId);
            Assert.AreEqual(ledger2.OrderId, retrievedLedgers[1].OrderId);
            Assert.AreEqual(ledger2.AccountId.Value, retrievedLedgers[1].AccountId.Value);

            Assert.AreEqual(ledger3.Currency.Name, retrievedLedgers[2].Currency.Name);
            Assert.AreEqual(ledger3.LedgerId, retrievedLedgers[2].LedgerId);
            Assert.AreEqual(ledger3.LedgerType, retrievedLedgers[2].LedgerType);
            Assert.AreEqual(ledger3.Amount, retrievedLedgers[2].Amount);
            Assert.AreEqual(ledger3.Fee, retrievedLedgers[2].Fee);
            Assert.AreEqual(ledger3.Balance, retrievedLedgers[2].Balance);
            Assert.AreEqual(ledger3.TradeId, retrievedLedgers[2].TradeId);
            Assert.AreEqual(ledger3.OrderId, retrievedLedgers[2].OrderId);
            Assert.AreEqual(ledger3.AccountId.Value, retrievedLedgers[2].AccountId.Value);
        }
    }
}
