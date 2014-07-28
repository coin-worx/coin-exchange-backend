using System;
using System.Collections.Generic;
using System.Configuration;
using System.Management.Instrumentation;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class FundsValidationServiceIntegrationTests
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

        #region Orders Validation

        [Test]
        public void ValidateBuyForAccountIdOrderFailTest_TestsIfNotEnoughBalanceSituationIsHandledAsExpected_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            
            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // Since 40 * 101 > 4000, we dont have enough USD to buy XBT
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 40, 101, "buy", "order123");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidateBuyForAccountIdOrderPassTest_TestsIfEnoughBalanceSituationIsHandledAsExpected_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 40, 100, "buy", "order123");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidateBuyOrderInsufficientBalanceAndVerifyBalanceTest_TestsIfTheBalanceIsAsExpectedAfterOrderValidation_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // Since 40 * 101 > 4000, we dont have enough USD to buy XBT
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 40, 101, "buy", "order123");
            Assert.IsFalse(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.AreEqual(quoteCurrencyBalance.AvailableBalance, quoteAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(quoteCurrencyBalance.CurrentBalance, quoteAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(quoteCurrencyBalance.PendingBalance, quoteAfterValidationBalance.PendingBalance);
        }

        [Test]
        public void ValidateBuyOrderSufficientBalanceAndVerifyBalanceTest_TestsIfTheBalanceIsAsExpectedAfterOrderValidation_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 40, 90, "buy", "order123");
            Assert.IsTrue(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.AreEqual(400, quoteAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(quoteCurrencyBalance.CurrentBalance, quoteAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(3600, quoteAfterValidationBalance.PendingBalance);
        }

        [Test]
        public void ValidateSellForAccountIdOrderFailTest_TestsIfNotEnoughBalanceSituationIsHandledAsExpected_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // Since 401 > 400, we dont have enough XBT to sell
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 401, 101, "sell", "order123");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidateSellForAccountIdOrderPassTest_TestsIfEnoughBalanceSituationIsHandledAsExpected_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // We have enough XBT to sell
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 400, 101, "sell", "order123");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidateSellOrderInsufficientBalanceAndVerifyBalanceTest_TestsIfTheBalanceIsAsExpectedAfterOrderValidation_VerifiesThroughTheReturnedValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // Since 401 > 400, we dont have enough XBT to sell
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 401, 101, "sell", "order123");
            Assert.IsFalse(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.AreEqual(quoteCurrencyBalance.AvailableBalance, quoteAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(quoteCurrencyBalance.CurrentBalance, quoteAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(quoteCurrencyBalance.PendingBalance, quoteAfterValidationBalance.PendingBalance);
        }

        [Test]
        public void ValidateSellOrderSufficientBalanceAndVerifyBalanceTest_TestsIfTheBalanceIsAsExpectedAfterOrderValidation_VerifiesThroughTheReturnedValue()
        {
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            // We have enough XBT to sell
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 300, 101, "sell", "order123");
            Assert.IsTrue(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.AreEqual(100, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(300, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.AreEqual(quoteCurrencyBalance.AvailableBalance, quoteAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(quoteCurrencyBalance.CurrentBalance, quoteAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(quoteCurrencyBalance.PendingBalance, quoteAfterValidationBalance.PendingBalance);
        }

        #endregion Orders Validation

        #region Withdrawal Validation

        [Test]
        public void WithdrawalFailTest_TestsIfWithdraweValidationReturnsFalseIfBalanceIsInsufficient_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 500
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNull(validateFundsForOrder);
        }

        [Test]
        public void WithdrawalPassTest_TestsIfWithdrawValidationReturnsTrueIfBalanceIsSufficient_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 101
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(validateFundsForOrder);
        }

        [Test]
        public void WithdrawalFailAndVerificationTest_TestsIfWithdrawValidationReturnsFalseIfBalanceIsInsufficient_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");
            Balance balance = new Balance(currency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 500
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNull(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.AreEqual(balance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, baseAfterValidationBalance.PendingBalance);
        }

        [Test] 
        public void WithdrawalValidationPassAndBalanceVerificationTest_TestsIfWithdrawValidationReturnsTrueIfBalanceIsSufficient_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");
            Balance balance = new Balance(currency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 101
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.AreEqual(299, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(101, baseAfterValidationBalance.PendingBalance);
        }

        #endregion Withdrawal Validation

        #region Deposit Tests

        [Test]
        public void InitialDepositSuccessTest_TestsIfThingsProceedAsExpectedWhenADepositIsMade_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 500, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);
            fundsPersistenceRepository.SaveOrUpdate(deposit);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(balance.CurrentBalance, deposit.Amount);
            Assert.AreEqual(balance.AvailableBalance, deposit.Amount);
            Assert.AreEqual(balance.PendingBalance, 0);
        }

        [Test]
        public void InitialDepositFailTest_TestsIfTransactioNFailsDueToInsufficientConfirmations_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 500, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(5);
            fundsPersistenceRepository.SaveOrUpdate(deposit);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsFalse(depositResponse);
            deposit.IncrementConfirmations(2);

            depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(balance.CurrentBalance, deposit.Amount);
            Assert.AreEqual(balance.AvailableBalance, deposit.Amount);
            Assert.AreEqual(balance.PendingBalance, 0);
        }

        [Test]
        public void MultipleDepositsSuccessTest_TestsIfThingsProceedAsExpectedWhenMultipleDepositsAreMade_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default,7500, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(balance.CurrentBalance, deposit.Amount);
            Assert.AreEqual(balance.AvailableBalance, deposit.Amount);
            Assert.AreEqual(balance.PendingBalance, 0);

            Ledger deposit1Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit1Ledger);

            Deposit deposit2 = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 3400, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit2.IncrementConfirmations(7);
            depositResponse = fundsValidationService.DepositConfirmed(deposit2);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(10900, balance.AvailableBalance);
            Assert.AreEqual(10900, balance.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, 0);

            Ledger deposit2Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit2Ledger);            
        }

        [Test]
        public void BalancePresentAndMultipleDepositsSuccessTest_TestsIfThingsProceedAsExpectedWhenMultipleDepositsAreMadeWhenBalanceIsAlreadyPresent_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            Balance balance = new Balance(currency, accountId, 2500, 4000);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 7500, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(4000, balance.CurrentBalance);
            Assert.AreEqual(2500, balance.AvailableBalance);
            Assert.AreEqual(1500, balance.PendingBalance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(11500, balance.CurrentBalance);
            Assert.AreEqual(10000, balance.AvailableBalance);
            Assert.AreEqual(1500, balance.PendingBalance);

            Ledger deposit1Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit1Ledger);

            Deposit deposit2 = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 3400, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit2.IncrementConfirmations(7);
            depositResponse = fundsValidationService.DepositConfirmed(deposit2);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(14900, balance.CurrentBalance);
            Assert.AreEqual(13400, balance.AvailableBalance);
            Assert.AreEqual(1500, balance.PendingBalance);

            Ledger deposit2Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit2Ledger);
        }

        #endregion Deposit Tests

        #region Trade Tests

        [Test]
        public void TradeBalanceUpdatedSuccessTest_VerifiesThatTheBalanceIsUpdatedAsExpectedAfterATradeOccurs_ChecksThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];            
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            string baseCurrency = "XBT";
            string quoteCurrency = "USD";
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderId123";
            string sellOrderId = "sellorderid123";
            string buyAccountId = "buyaccountid123";
            string sellAccountId = "sellaccountid123";

            Balance buyAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountBaseCurrencyBalance);
            Balance buyAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountQuoteCurrencyBalance);

            Balance sellAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountBaseCurrencyBalance);
            Balance sellAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountQuoteCurrencyBalance);
            bool tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 400, 100,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
            Assert.IsTrue(tradeExecutedResponse);

            buyAccountBaseCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(baseCurrency), new AccountId(buyAccountId));
            Assert.IsNotNull(buyAccountBaseCurrencyBalance);
            Assert.AreEqual(400, buyAccountBaseCurrencyBalance.CurrentBalance);
            Assert.AreEqual(400, buyAccountBaseCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, buyAccountBaseCurrencyBalance.PendingBalance);

            buyAccountQuoteCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(quoteCurrency), new AccountId(buyAccountId));
            Assert.AreEqual(-40000, buyAccountQuoteCurrencyBalance.CurrentBalance);
            Assert.AreEqual(-40000, buyAccountQuoteCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, buyAccountQuoteCurrencyBalance.PendingBalance);

            sellAccountBaseCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(baseCurrency), new AccountId(sellAccountId));
            Assert.AreEqual(-400, sellAccountBaseCurrencyBalance.CurrentBalance);
            Assert.AreEqual(-400, sellAccountBaseCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, sellAccountBaseCurrencyBalance.PendingBalance);

            sellAccountQuoteCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(quoteCurrency), new AccountId(sellAccountId));
            Assert.AreEqual(40000, sellAccountQuoteCurrencyBalance.CurrentBalance);
            Assert.AreEqual(40000, sellAccountQuoteCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, sellAccountQuoteCurrencyBalance.PendingBalance);
        }

        [Test]
        public void TradeTransactionsAfterValidationTest_TEstsIfTheLedgerTransactionsWereCreatedAsExpectedAfterBalanceUpdate_VerifiesThroughDatabaseQUery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];

            string baseCurrency = "XBT";
            string quoteCurrency = "USD";
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderid123";
            string sellOrderId = "sellorderid123";
            string buyAccountId = "buyaccountid123";
            string sellAccountId = "sellaccountid123";

            Balance buyAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountBaseCurrencyBalance);
            Balance buyAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountQuoteCurrencyBalance);

            Balance sellAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountBaseCurrencyBalance);
            Balance sellAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountQuoteCurrencyBalance);
            bool tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 400, 100,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
            Assert.IsTrue(tradeExecutedResponse);

            List<Ledger> ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(new AccountId("buyaccountid123"));
            Assert.AreEqual(2, ledgerByAccountId.Count);

            // Any of the two ledgers for currencies can be fetched one after the other as they might be happening in the
            // same second, so we check which one is fetched first

            // First, buy side order's ledger will be verified
            if (ledgerByAccountId[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(400, ledgerByAccountId[0].Amount);
                Assert.AreEqual(400, ledgerByAccountId[0].Balance);
                // For USD
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[1].Amount);
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[1].Balance);

                // Get the fee corresponding to the currenct volume of the quote currency
                double fee = feeCalculationService.GetFee(new Currency("USD"), 400 * 100);
                Assert.AreEqual(fee, ledgerByAccountId[1].Fee);
            }
            else if (ledgerByAccountId[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[0].Amount);
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[0].Balance);
                // Get the fee corresponding to the currenct volume of the quote currency
                double fee = feeCalculationService.GetFee(new Currency("USD"), 400 * 100);
                Assert.AreEqual(fee, ledgerByAccountId[0].Fee);

                // For XBT
                Assert.AreEqual(400, ledgerByAccountId[1].Amount);
                Assert.AreEqual(400, ledgerByAccountId[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }
            Assert.AreEqual("tradeid123", ledgerByAccountId[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByAccountId[0].LedgerType);
            Assert.AreEqual("buyorderid123", ledgerByAccountId[0].OrderId);
            Assert.AreEqual("tradeid123", ledgerByAccountId[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByAccountId[1].LedgerType);
            Assert.AreEqual("buyorderid123", ledgerByAccountId[1].OrderId);


            ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(new AccountId("sellaccountid123"));
            Assert.AreEqual(2, ledgerByAccountId.Count);

            // Secondly, wee verify for the sell order side's ledgers
            if (ledgerByAccountId[0].Currency.Name == "XBT")
            {
                // For XBT
                Assert.AreEqual(-400, ledgerByAccountId[0].Amount);
                Assert.AreEqual(-400, ledgerByAccountId[0].Balance);
                // For USD
                Assert.AreEqual((400 * 100), ledgerByAccountId[1].Amount);
                Assert.AreEqual((400 * 100), ledgerByAccountId[1].Balance);
                // Get the fee corresponding to the currenct volume of the quote currency
                double fee = feeCalculationService.GetFee(new Currency("USD"), 400 * 100);
                Assert.AreEqual(fee, ledgerByAccountId[1].Fee);
            }
            else if (ledgerByAccountId[0].Currency.Name == "USD")
            {
                // For USD
                Assert.AreEqual((400 * 100), ledgerByAccountId[0].Amount);
                Assert.AreEqual((400 * 100), ledgerByAccountId[0].Balance);
                // Get the fee corresponding to the currenct volume of the quote currency
                double fee = feeCalculationService.GetFee(new Currency("USD"), 400 * 100);
                Assert.AreEqual(fee, ledgerByAccountId[0].Fee);
                // For XBT
                Assert.AreEqual(-400, ledgerByAccountId[1].Amount);
                Assert.AreEqual(-400, ledgerByAccountId[1].Balance);
            }
            else
            {
                throw new InstanceNotFoundException("No instance found for either of the two expected currencies.");
            }

            Assert.AreEqual("tradeid123", ledgerByAccountId[0].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByAccountId[0].LedgerType);
            Assert.AreEqual("sellorderid123", ledgerByAccountId[0].OrderId);
            Assert.AreEqual("tradeid123", ledgerByAccountId[1].TradeId);
            Assert.AreEqual(LedgerType.Trade, ledgerByAccountId[1].LedgerType);
            Assert.AreEqual("sellorderid123", ledgerByAccountId[1].OrderId);
        }

        [Test]
        public void MultipleTradesBalanceUpdateInLedgerTest_TestsIfBalanceIsBeingUpdatedInTHeLedgerTransactionsAsExpected_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];

            string baseCurrency = "XBT";
            string quoteCurrency = "USD";
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderid123";
            string sellOrderId = "sellorderid123";
            string buyAccountId = "buyaccountid123";
            string sellAccountId = "sellaccountid123";

            Balance buyXbtBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(buyXbtBalance);
            Balance buyUsdBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(buyUsdBalance);

            Balance sellXbtBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(sellXbtBalance);
            Balance sellUsdBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(sellUsdBalance);

            // ----- Trade 1-----
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(buyAccountId), 
                new Currency(baseCurrency), new Currency(quoteCurrency), 100, 100, "buy", buyOrderId + "1");
            Assert.IsTrue(validateFundsForOrder);

            validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(sellAccountId),
                new Currency(baseCurrency), new Currency(quoteCurrency), 100, 100, "sell", sellOrderId + "1");
            Assert.IsTrue(validateFundsForOrder);

            bool tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 100, 100,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId + "1", sellOrderId + "1");
            Assert.IsTrue(tradeExecutedResponse);

            buyXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(baseCurrency),
                                                                   new AccountId(buyAccountId));
            double buyerBaseAccountBalance = ledgerRepository.GetBalanceForCurrency(baseCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyXbtBalance.CurrentBalance, buyerBaseAccountBalance);
            Assert.AreEqual(4100, buyXbtBalance.CurrentBalance);
            Assert.AreEqual(4100, buyXbtBalance.AvailableBalance);

            buyUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                                                                   new AccountId(buyAccountId));
            double buyerQuoteAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyUsdBalance.CurrentBalance, buyerQuoteAccountBalance);
            Assert.AreEqual(40000, buyUsdBalance.CurrentBalance);
            Assert.AreEqual(40000, buyUsdBalance.AvailableBalance);

            // ----- Trade 1-----

            tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 300, 102,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId + "2", sellOrderId + "2");
            Assert.IsTrue(tradeExecutedResponse);

            tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 300, 102,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId + "3", sellOrderId + "3");
            Assert.IsTrue(tradeExecutedResponse);

            buyXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(baseCurrency),
                                                                   new AccountId(buyAccountId));
            buyerBaseAccountBalance = ledgerRepository.GetBalanceForCurrency(baseCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyXbtBalance.CurrentBalance, buyerBaseAccountBalance);

            buyUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                                                                   new AccountId(buyAccountId));
            buyerQuoteAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyUsdBalance.CurrentBalance, buyerQuoteAccountBalance);

            sellXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(baseCurrency),
                                                                   new AccountId(sellAccountId));
            double sellerBaseAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(sellAccountId));
            Assert.AreEqual(sellXbtBalance.CurrentBalance, sellerBaseAccountBalance);

            sellUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                                                                   new AccountId(sellAccountId));
            double sellerQuoteAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(sellAccountId));
            Assert.AreEqual(sellUsdBalance.CurrentBalance, sellerQuoteAccountBalance);
        }

        #endregion Trade Tests

        #region Withdrawal Confirmed Tests

        [Test]
        public void WithdrawalConfirmedPassTest_TestsIfTheWithdrawalOperationProceedsAsExpected_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IWithdrawIdGeneratorService withdrawIdGeneratorService = (IWithdrawIdGeneratorService)ContextRegistry.GetContext()["WithdrawIdGeneratorService"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            Balance balance = new Balance(currency, accountId, 2000, 2400);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Withdraw withdraw = new Withdraw(currency, withdrawIdGeneratorService.GenerateNewId(), DateTime.Now, 
                WithdrawType.Default, 400, 0.4, TransactionStatus.Confirmed, accountId, new TransactionId("transaction123"),
                new BitcoinAddress("bitcoin123"));

            bool withdrawalExecuted = fundsValidationService.WithdrawalExecuted(withdraw);
            Assert.IsTrue(withdrawalExecuted);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(2000, balance.CurrentBalance);
            Assert.AreEqual(2000, balance.AvailableBalance);
            Assert.AreEqual(0, balance.PendingBalance);
        }

        #endregion Withdrawal Confirmed Tests

        #region Order Cancelled Tests

        [Test]
        public void OrderCancelledFundsRestoreTest_TestsIfTheFundsAreRestoredAsExpectedInCaseOfAnOrderCancelled_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IWithdrawIdGeneratorService withdrawIdGeneratorService = (IWithdrawIdGeneratorService)ContextRegistry.GetContext()["WithdrawIdGeneratorService"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 5000, 5000);
            Balance quoteCurrencyBalance = new Balance(baseCurrency, accountId, 20000, 20000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 100, 90, "buy", "order123");
            Assert.IsTrue(validateFundsForOrder);
            fundsValidationService.OrderCancelled(baseCurrency, accountId, "orderid123", 500);

        }
        #endregion Order Cancelled Tests

        #region Transaction Jobs Tests

        [Test]
        public void ValidateOrderTransactionTest_TestsIfTransactioNIntegrityIsMantainedDuringSimultaneuosTransactions_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];            
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            IWithdrawIdGeneratorService withdrawIdGeneratorService = (IWithdrawIdGeneratorService)ContextRegistry.GetContext()["WithdrawIdGeneratorService"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 1000, 1000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 400, 101, "sell", "order123");
            fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 300, 101, "sell", "order123");

            // Here we create a service without using the spring dependency injection to create a new instance
            IFundsValidationService fundsValidationService2 = new FundsValidationService(transactionService,
                fundsPersistenceRepository, balanceRepository, feeCalculationService, 
                withdrawFeesRepository, withdrawIdGeneratorService);
            fundsValidationService2.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 100, 101, "sell", "order123");
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(1000, balance.CurrentBalance);
            Assert.AreEqual(200, balance.AvailableBalance);
        }

        [Test]
        public void ValidateWithdrawTransactionTest_TestsIfTransactioNIntegrityIsMantainedDuringSimultaneuosTransactions_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            IWithdrawIdGeneratorService withdrawIdGeneratorService = (IWithdrawIdGeneratorService)ContextRegistry.GetContext()["WithdrawIdGeneratorService"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 1000, 1000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 400, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));

            // Here we create a service without using the spring dependency injection to create a new instance
            IFundsValidationService fundsValidationService2 = new FundsValidationService(transactionService,
                fundsPersistenceRepository, balanceRepository, feeCalculationService, withdrawFeesRepository,
                withdrawIdGeneratorService);
            fundsValidationService2.ValidateFundsForWithdrawal(accountId, baseCurrency, 100, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(1000, balance.CurrentBalance);
            Assert.AreEqual(500, balance.AvailableBalance);
        }

        #endregion Transaction Jobs Tests
    }
}
