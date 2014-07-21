using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Domain.Model.IntegrationTests
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
        public void CreateTradeTransactionAndGetByAccountIdsTest_TestsIfTransacitonIsCreatedAsExpectedUponNewTrade_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IList<Ledger> allLedgers = ledgerRepository.GetAllLedgers();
            Assert.AreEqual(0, allLedgers.Count);
            bool tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 300, 498.98, DateTime.Now,
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            List<Ledger> ledgersByTradeId = ledgerRepository.GetLedgersByTradeId("tradeid123");
            Assert.AreEqual(4, ledgersByTradeId.Count);
        }

        [Test]
        public void CreateTradeTransactionAndGetByUsersAccountIdsTest_TestsIfTransacitonIsCreatedAsExpectedUponNewTrade_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IList<Ledger> allLedgers = ledgerRepository.GetAllLedgers();
            Assert.AreEqual(0, allLedgers.Count);
            bool tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 300, 498.98, DateTime.Now,
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            List<Ledger> ledgerByCurrencyName = ledgerRepository.GetLedgerByAccountId(new AccountId("buyaccountid123"));
            Assert.AreEqual(2, ledgerByCurrencyName.Count);

            // Any of the two ledgers for currencies can be fetched one after the other as they might be happening in the
            // same second, so we check which one is fetched first

            // First, buy side order's ledger will be verified
            if (ledgerByCurrencyName[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(300, ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(300, ledgerByCurrencyName[0].Balance);
                // For USD
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[1].Balance);
            }
            else if (ledgerByCurrencyName[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[0].Balance);
                // For XBT
                Assert.AreEqual(300, ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(300, ledgerByCurrencyName[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[0].LedgerType);
            Assert.AreEqual("buyorderid123", ledgerByCurrencyName[0].OrderId);
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[1].LedgerType);
            Assert.AreEqual("buyorderid123", ledgerByCurrencyName[1].OrderId);

            ledgerByCurrencyName = ledgerRepository.GetLedgerByAccountId(new AccountId("sellaccountid123"));
            Assert.AreEqual(2, ledgerByCurrencyName.Count);

            // Secondly, wee verify for the sell order side's ledgers
            if (ledgerByCurrencyName[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(-300, ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(-300, ledgerByCurrencyName[0].Balance);
                // For USD
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[1].Amount);
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[1].Balance);
            }
            else if (ledgerByCurrencyName[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[0].Amount);
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[0].Balance);
                // For XBT
                Assert.AreEqual(-300, ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(-300, ledgerByCurrencyName[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }

            Assert.AreEqual("tradeid123", ledgerByCurrencyName[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[0].LedgerType);
            Assert.AreEqual("sellorderid123", ledgerByCurrencyName[0].OrderId);
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[1].LedgerType);
            Assert.AreEqual("sellorderid123", ledgerByCurrencyName[1].OrderId);
        }

        [Test]
        public void CreateTradeTransactionAndGetByOrderIdTest_TestsIfTransacitonIsCreatedAsExpectedUponNewTrade_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IList<Ledger> allLedgers = ledgerRepository.GetAllLedgers();
            Assert.AreEqual(0, allLedgers.Count);
            bool tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 300, 498.98, DateTime.Now, 
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            List<Ledger> ledgerByCurrencyName = ledgerRepository.GetLedgersByOrderId("buyorderid123");
            Assert.AreEqual(2, ledgerByCurrencyName.Count);

            // Any of the two ledgers for currencies can be fetched one after the other as they might be happening in the
            // same second, so we check which one is fetched first

            // First, buy side order's ledger will be verified
            if (ledgerByCurrencyName[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(300, ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(300, ledgerByCurrencyName[0].Balance);
                // For USD
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[1].Balance);
            }
            else if (ledgerByCurrencyName[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(-(300 * 498.98), ledgerByCurrencyName[0].Balance);
                // For XBT
                Assert.AreEqual(300, ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(300, ledgerByCurrencyName[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[0].LedgerType);
            Assert.AreEqual("buyaccountid123", ledgerByCurrencyName[0].AccountId.Value);
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[1].LedgerType);
            Assert.AreEqual("buyaccountid123", ledgerByCurrencyName[1].AccountId.Value);

            ledgerByCurrencyName = ledgerRepository.GetLedgersByOrderId("sellorderid123");
            Assert.AreEqual(2, ledgerByCurrencyName.Count);

            // Secondly, wee verify for the sell order side's ledgers
            if (ledgerByCurrencyName[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(-300, ledgerByCurrencyName[0].Amount);
                Assert.AreEqual(-300, ledgerByCurrencyName[0].Balance);
                // For USD
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[1].Amount);
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[1].Balance);
            }
            else if (ledgerByCurrencyName[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[0].Amount);
                Assert.AreEqual((300 * 498.98), ledgerByCurrencyName[0].Balance);
                // For XBT
                Assert.AreEqual(-300, ledgerByCurrencyName[1].Amount);
                Assert.AreEqual(-300, ledgerByCurrencyName[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }

            Assert.AreEqual("tradeid123", ledgerByCurrencyName[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[0].LedgerType);
            Assert.AreEqual("sellaccountid123", ledgerByCurrencyName[0].AccountId.Value);
            Assert.AreEqual("tradeid123", ledgerByCurrencyName[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByCurrencyName[1].LedgerType);
            Assert.AreEqual("sellaccountid123", ledgerByCurrencyName[1].AccountId.Value);
        }

        [Test]
        public void AddTransactionsToIncrementBalanceTest_TestsIfBalanceEndsUpAsExpected_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IList<Ledger> allLedgers = ledgerRepository.GetAllLedgers();
            Assert.AreEqual(0, allLedgers.Count);
            bool tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 300, 498.98, DateTime.Now,
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 200, 491.81, DateTime.Now,
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            tradeTransaction = transactionService.CreateTradeTransaction("XBTUSD", 500, 497.77, DateTime.Now,
                "tradeid123", "buyaccountid123", "sellaccountid123", "buyorderid123", "sellorderid123");
            Assert.IsTrue(tradeTransaction);
            double buyerXbtAccountBalance = ledgerRepository.GetBalanceForCurrency("XBT", new AccountId("buyaccountid123"));
            Assert.AreEqual(1000, buyerXbtAccountBalance);
            double buyerUsdAccountBalance = ledgerRepository.GetBalanceForCurrency("USD", new AccountId("buyaccountid123"));
            Assert.AreEqual(-((300 * 498.98) + (200 * 491.81) + (500 * 497.77)), buyerUsdAccountBalance);

            double sellerXbtAccountBalance = ledgerRepository.GetBalanceForCurrency("XBT", new AccountId("sellaccountid123"));
            Assert.AreEqual(-1000, sellerXbtAccountBalance);
            double sellerUsdAccountBalance = ledgerRepository.GetBalanceForCurrency("USD", new AccountId("sellaccountid123"));
            Assert.AreEqual((300 * 498.98) + (200 * 491.81) + (500 * 497.77), sellerUsdAccountBalance);
        }

        [Test]
        public void AddDepositAndCheckReturnedParameterTest_TestsIfTheReturnedLedgerFromDepositContainsSameValuesAsGivenDeposit_ComparesVariables()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            
            Deposit deposit = new Deposit(new Currency("LTC"), "depositid123", DateTime.Now, "New", 300, 0, TransactionStatus.Pending,
                new AccountId("accountid123"), new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            Ledger depositTransaction1 = transactionService.CreateDepositTransaction(deposit);
            Assert.IsNotNull(depositTransaction1);
            Assert.AreEqual(deposit.Currency.Name, depositTransaction1.Currency.Name);
            Assert.AreEqual(deposit.Amount, depositTransaction1.Amount);
            Assert.AreEqual(300, depositTransaction1.Balance);
            Assert.AreEqual(deposit.DepositId, depositTransaction1.DepositId);
            Assert.AreEqual(deposit.AccountId, depositTransaction1.AccountId);
        }

        [Test]
        public void AddDepositTest_TestsIfBalanceEndsUpAsExpected_VerifiesThroughDatabaseQuery()
        {
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            
            Deposit deposit = new Deposit(new Currency("LTC"), "depositid123", DateTime.Now, "New", 300, 0, TransactionStatus.Pending, 
                new AccountId("accountid123"), new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations();

            Ledger depositTransaction1 = transactionService.CreateDepositTransaction(deposit);
            Assert.IsNull(depositTransaction1);
            List<Ledger> ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(deposit.AccountId);
            // There will be no ledgers yet as the deposit did not had 7 confirmations
            Assert.AreEqual(0, ledgerByAccountId.Count);

            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            depositTransaction1 = transactionService.CreateDepositTransaction(deposit);
            Assert.IsNotNull(depositTransaction1);
            ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(deposit.AccountId);
            // There will be no ledgers yet as the deposit did not had 7 confirmations
            Assert.AreEqual(1, ledgerByAccountId.Count);
            Assert.AreEqual("LTC", ledgerByAccountId.Single().Currency.Name);
            Assert.AreEqual(300, ledgerByAccountId.Single().Amount);
            Assert.AreEqual(300, ledgerByAccountId.Single().Balance);
            Assert.AreEqual("depositid123", ledgerByAccountId.Single().DepositId);
            Assert.AreEqual("accountid123", ledgerByAccountId.Single().AccountId.Value);
        }
    }
}
