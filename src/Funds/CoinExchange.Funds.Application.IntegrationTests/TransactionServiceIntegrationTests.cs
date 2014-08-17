using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class TransactionServiceIntegrationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
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
        public void AddDepositAndCheckReturnedParameterTest_TestsIfTheReturnedLedgerFromDepositContainsSameValuesAsGivenDeposit_ComparesVariables()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            Deposit deposit = new Deposit(new Currency("LTC"), "depositid123", DateTime.Now, DepositType.Default, 300, 0, TransactionStatus.Pending,
                new AccountId(123), new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            bool response = transactionService.CreateDepositTransaction(deposit, 300);
            Assert.IsTrue(response);
            Ledger depositTransaction1 = ledgerRepository.GetLedgerByAccountId(new AccountId(123)).Single();
            Assert.IsNotNull(depositTransaction1);
            Assert.AreEqual(deposit.Currency.Name, depositTransaction1.Currency.Name);
            Assert.AreEqual(deposit.Amount, depositTransaction1.Amount);
            Assert.AreEqual(300, depositTransaction1.Balance);
            Assert.AreEqual(deposit.DepositId, depositTransaction1.DepositId);
            Assert.AreEqual(deposit.AccountId.Value, depositTransaction1.AccountId.Value);
        }

        [Test]
        public void AddDepositTest_TestsIfBalanceEndsUpAsExpected_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            
            Deposit deposit = new Deposit(new Currency("LTC"), "depositid123", DateTime.Now, DepositType.Default, 300, 0, TransactionStatus.Pending, 
                new AccountId(123), new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);

            bool response = transactionService.CreateDepositTransaction(deposit, 300);
            Assert.IsTrue(response);
            Ledger depositTransaction1 = ledgerRepository.GetLedgerByAccountId(new AccountId(123)).Single();
            Assert.IsNotNull(depositTransaction1);
            List<Ledger> ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(deposit.AccountId);
            // There will be no ledgers yet as the deposit did not had 7 confirmations
            Assert.AreEqual(1, ledgerByAccountId.Count);
            Assert.AreEqual("LTC", ledgerByAccountId.Single().Currency.Name);
            Assert.AreEqual(300, ledgerByAccountId.Single().Amount);
            Assert.AreEqual(300, ledgerByAccountId.Single().Balance);
            Assert.AreEqual("depositid123", ledgerByAccountId.Single().DepositId);
            Assert.AreEqual(123, ledgerByAccountId.Single().AccountId.Value);
        }
    }
}
