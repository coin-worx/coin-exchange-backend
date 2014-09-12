using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.Common.Utility;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Application.WithdrawServices;
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
            
            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawalFailTest_TestsIfWithdraweValidationReturnsFalseIfBalanceIsInsufficient_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only withdraw 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day, approx. = 1.7
            // Likewise, monthly limit for Tier 1 is $5000 and the threshold for a particular currency is measured the 
            // same way as above

            // This test will fail due to over the llimit withdrawal amount requested
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC", true);
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 3
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNull(validateFundsForOrder);
        }

        [Test]
        public void WithdrawalPassTest_TestsIfWithdrawValidationReturnsTrueIfBalanceIsSufficient_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only withdraw 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day, approx = 1.7
            // Likewise, monthly limit for Tier 1 is $5000 and the threshold for a particular currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC", true);
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 1.7m
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(validateFundsForOrder);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawalFailAndVerificationTest_TestsIfWithdrawValidationReturnsFalseIfBalanceIsInsufficient_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only withdraw 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day. 
            // Likewise, monthly limit for Tier 1 is $5000 and the threshold for a particular currency is measured the 
            // same way as above

            // This test will fail due to over the llimit withdrawal amount requested
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            Balance balance = new Balance(currency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(balance);
            WithdrawFees withdrawFee = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 2.5m
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
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only withdraw 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day, approx = 1.7
            // Likewise, monthly limit for Tier 1 is $5000 and the threshold for a particular currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            Balance balance = new Balance(currency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Withdraw validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 1.4m
                , new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(validateFundsForOrder);

            WithdrawFees withdrawFee = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.AreEqual(20 - 1.4m - withdrawFee.Fee, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(1.4m + withdrawFee.Fee, baseAfterValidationBalance.PendingBalance);
        }

        #endregion Withdrawal Validation

        #region Deposit Tests

        [Test]
        public void InitialDepositSuccessTest_TestsIfThingsProceedAsExpectedWhenADepositIsMade_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only submit 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day. 
            // Likewise, monthlyl limit for Tier 1 is $5000 and the threshold for a particualr currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 1.5m, 0, TransactionStatus.Pending, accountId,
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
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only submit 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day. 
            // Likewise, monthlyl limit for Tier 1 is $5000 and the threshold for a particualr currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
           
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 1.2m, 0, TransactionStatus.Pending, accountId,
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

            deposit = depositRepository.GetDepositByTransactionId(deposit.TransactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(TransactionStatus.Confirmed, deposit.Status);
        }

        [Test]
        public void MultipleDepositsSuccessTest_TestsIfThingsProceedAsExpectedWhenMultipleDepositsAreMade_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only submit 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day. 
            // Likewise, monthlyl limit for Tier 1 is $5000 and the threshold for a particualr currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 1.5m, 0, TransactionStatus.Pending, accountId,
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
                                          DepositType.Default, 0.001m, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit2.IncrementConfirmations(7);
            depositResponse = fundsValidationService.DepositConfirmed(deposit2);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(1.501, balance.AvailableBalance);
            Assert.AreEqual(1.501, balance.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, 0);

            Ledger deposit2Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit2Ledger);            
        }

        [Test]
        public void BalancePresentAndMultipleDepositsSuccessTest_TestsIfThingsProceedAsExpectedWhenMultipleDepositsAreMadeWhenBalanceIsAlreadyPresent_VerifiesThroughDatabaseQuery()
        {
            // According to the TierLevel 1, the Daily limit is $1000 dollars. We can only submit 
            // (1000/BestBid(XBT/USD) + (1000/BestAsk(XBT/USD)/2)) XBT for one day. 
            // Likewise, monthlyl limit for Tier 1 is $5000 and the threshold for a particualr currency is measured the 
            // same way as above
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            Balance balance = new Balance(currency, accountId, 2.5m, 4);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 1.5m, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(4, balance.CurrentBalance);
            Assert.AreEqual(2.5m, balance.AvailableBalance);
            Assert.AreEqual(1.5m, balance.PendingBalance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(5.5, balance.CurrentBalance);
            Assert.AreEqual(4, balance.AvailableBalance);
            Assert.AreEqual(1.5, balance.PendingBalance);

            Ledger deposit1Ledger = ledgerRepository.GetLedgersByDepositId(deposit.DepositId);
            Assert.IsNotNull(deposit1Ledger);

            Deposit deposit2 = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 0.2m, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit2.IncrementConfirmations(7);
            depositResponse = fundsValidationService.DepositConfirmed(deposit2);
            Assert.IsTrue(depositResponse);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(5.7, balance.CurrentBalance);
            Assert.AreEqual(4.2, balance.AvailableBalance);
            Assert.AreEqual(1.5, balance.PendingBalance);

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
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];

            string baseCurrency = "BTC";
            string quoteCurrency = "USD";
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderid123";
            string sellOrderId = "sellorderid123";
            int buyAccountId = 1;
            int sellAccountId = 2;

            Balance buyAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountBaseCurrencyBalance);
            Balance buyAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountQuoteCurrencyBalance);

            // Get the fee corresponding to the currenct volume of the quote currency
            decimal buySideFee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                new AccountId(buyAccountId), 400, 100);
            decimal sellSideFee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                new AccountId(sellAccountId), 400, 100);

            Balance sellAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 6000, 6000);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountBaseCurrencyBalance);
            Balance sellAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 60000, 60000);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountQuoteCurrencyBalance);
            bool orderValidation = fundsValidationService.ValidateFundsForOrder(new AccountId(buyAccountId), new Currency(baseCurrency),
                                                         new Currency(quoteCurrency), 400, 100, "buy", buyOrderId);
            Assert.IsTrue(orderValidation);
            orderValidation = fundsValidationService.ValidateFundsForOrder(new AccountId(sellAccountId),
                new Currency(baseCurrency), new Currency(quoteCurrency), 400, 100, "sell", sellOrderId);
            Assert.IsTrue(orderValidation);
            bool tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 400, 100,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
            Assert.IsTrue(tradeExecutedResponse);

            buyAccountBaseCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(baseCurrency), new AccountId(buyAccountId));
            Assert.IsNotNull(buyAccountBaseCurrencyBalance);
            Assert.AreEqual(4400, buyAccountBaseCurrencyBalance.CurrentBalance);
            Assert.AreEqual(4400, buyAccountBaseCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, buyAccountBaseCurrencyBalance.PendingBalance);

            buyAccountQuoteCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(quoteCurrency), new AccountId(buyAccountId));
            Assert.AreEqual(50000-40000-buySideFee, buyAccountQuoteCurrencyBalance.CurrentBalance);
            Assert.AreEqual(50000-40000-buySideFee, buyAccountQuoteCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, buyAccountQuoteCurrencyBalance.PendingBalance);

            sellAccountBaseCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(baseCurrency), new AccountId(sellAccountId));
            Assert.AreEqual(6000 - 400, sellAccountBaseCurrencyBalance.CurrentBalance);
            Assert.AreEqual(6000 - 400, sellAccountBaseCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, sellAccountBaseCurrencyBalance.PendingBalance);

            sellAccountQuoteCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(
                new Currency(quoteCurrency), new AccountId(sellAccountId));
            Assert.AreEqual(60000 + (400 * 100) - sellSideFee, sellAccountQuoteCurrencyBalance.CurrentBalance);
            Assert.AreEqual(60000 + (400 * 100) - sellSideFee, sellAccountQuoteCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, sellAccountQuoteCurrencyBalance.PendingBalance);
        }

        [Test]
        public void TradeTransactionsAfterValidationTest_TEstsIfTheLedgerTransactionsWereCreatedAsExpectedAfterBalanceUpdate_VerifiesThroughDatabaseQUery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];

            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(feeRepository.GetAllFees().First().CurrencyPair);
            string baseCurrency = splittedCurrencyPair.Item1;
            string quoteCurrency = splittedCurrencyPair.Item2;
            AccountId accountId = new AccountId(1);
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderid123";
            string sellOrderId = "sellorderid123";
            int buyAccountId = 1;
            int sellAccountId = 2;

            Balance buyAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountBaseCurrencyBalance);
            Balance buyAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(buyAccountQuoteCurrencyBalance);

            // Get the fee corresponding to the currenct volume of the quote currency
            decimal buySideFee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                new AccountId(buyAccountId), 400, 100);
            decimal sellSideFee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                new AccountId(sellAccountId), 400, 100);
            Assert.Greater(buySideFee, 0);

            Balance sellAccountBaseCurrencyBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 6000, 6000);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountBaseCurrencyBalance);
            Balance sellAccountQuoteCurrencyBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 60000, 60000);
            fundsPersistenceRepository.SaveOrUpdate(sellAccountQuoteCurrencyBalance);
            bool orderValidation = fundsValidationService.ValidateFundsForOrder(new AccountId(buyAccountId), new Currency(baseCurrency),
                                                         new Currency(quoteCurrency), 400, 100, "buy", buyOrderId);
            Assert.IsTrue(orderValidation);
            orderValidation = fundsValidationService.ValidateFundsForOrder(new AccountId(sellAccountId),
                new Currency(baseCurrency), new Currency(quoteCurrency), 400, 100, "sell", sellOrderId);
            Assert.IsTrue(orderValidation);
            bool tradeExecutedResponse = fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, 400, 100,
                DateTime.Now, tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
            Assert.IsTrue(tradeExecutedResponse);

            Balance buyXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(baseCurrency), new AccountId(buyAccountId));
            Assert.AreEqual(4000 + 400, buyXbtBalance.AvailableBalance);
            Assert.AreEqual(4000 + 400, buyXbtBalance.CurrentBalance);
            Balance buyUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency), new AccountId(buyAccountId));
            Assert.AreEqual(50000 + (-400 * 100) - buySideFee, buyUsdBalance.AvailableBalance);
            Assert.AreEqual(50000 + (-400 * 100) - buySideFee, buyUsdBalance.CurrentBalance);
            List<Ledger> ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(new AccountId(1));
            
            Assert.AreEqual(2, ledgerByAccountId.Count);

            // Any of the two ledgers for currencies can be fetched one after the other as they might be happening in the
            // same second, so we check which one is fetched first

            // First, buy side order's ledger will be verified
            if (ledgerByAccountId[0].Currency.Name == splittedCurrencyPair.Item1)
            {
                // For Base Currency
                Assert.AreEqual(400, ledgerByAccountId[0].Amount);
                Assert.AreEqual(4000 + 400, ledgerByAccountId[0].Balance);
                // For Quote CUrrency
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[1].Amount);
                Assert.AreEqual(50000 -(400 * 100) - buySideFee, ledgerByAccountId[1].Balance);

                Assert.AreEqual(buySideFee, ledgerByAccountId[1].Fee);
            }
            else if (ledgerByAccountId[0].Currency.Name == splittedCurrencyPair.Item2)
            {
                // For Quote Currency
                Assert.AreEqual(-(400 * 100), ledgerByAccountId[0].Amount);
                Assert.AreEqual(50000 -(400 * 100) - buySideFee, ledgerByAccountId[0].Balance);
                Assert.AreEqual(buySideFee, ledgerByAccountId[0].Fee);

                // For Base Currency
                Assert.AreEqual(400, ledgerByAccountId[1].Amount);
                Assert.AreEqual(4000 + 400, ledgerByAccountId[1].Balance);
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

            Balance sellXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(baseCurrency),
                new AccountId(sellAccountId));
            Assert.AreEqual(6000 - 400, sellXbtBalance.AvailableBalance);
            Assert.AreEqual(6000 - 400, sellXbtBalance.CurrentBalance);

            Balance sellUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                new AccountId(sellAccountId));
            Assert.AreEqual(60000 + (400 * 100) - sellSideFee, sellUsdBalance.AvailableBalance);
            Assert.AreEqual(60000 + (400 * 100) - sellSideFee, sellUsdBalance.CurrentBalance);

            ledgerByAccountId = ledgerRepository.GetLedgerByAccountId(new AccountId(2));            
            Assert.AreEqual(2, ledgerByAccountId.Count);

            // Secondly, wee verify for the sell order side's ledgers
            if (ledgerByAccountId[0].Currency.Name == splittedCurrencyPair.Item1)
            {
                // For Base Currency
                Assert.AreEqual(-400, ledgerByAccountId[0].Amount);
                Assert.AreEqual(6000 - 400, ledgerByAccountId[0].Balance);
                // For Quote Currency
                Assert.AreEqual((400 * 100), ledgerByAccountId[1].Amount);
                Assert.AreEqual(60000 + (400 * 100) - sellSideFee, ledgerByAccountId[1].Balance);
                Assert.AreEqual(sellSideFee, ledgerByAccountId[1].Fee);
            }
            else if (ledgerByAccountId[0].Currency.Name == splittedCurrencyPair.Item2)
            {
                // For Quote Currency
                Assert.AreEqual((400 * 100), ledgerByAccountId[0].Amount);
                Assert.AreEqual(60000 + (400 * 100) - sellSideFee, ledgerByAccountId[0].Balance);
                // Get the fee corresponding to the currenct volume of the quote currency
                decimal fee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                    new AccountId(buyAccountId), 400, 100);
                Assert.AreEqual(fee, ledgerByAccountId[0].Fee);
                // For Base Currency
                Assert.AreEqual(-400, ledgerByAccountId[1].Amount);
                Assert.AreEqual(6000 - 400, ledgerByAccountId[1].Balance);
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
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];

            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(feeRepository.GetAllFees().First().CurrencyPair);
            string baseCurrency = splittedCurrencyPair.Item1;
            string quoteCurrency = splittedCurrencyPair.Item2;
            string tradeId = "tradeid123";
            string buyOrderId = "buyorderid123";
            string sellOrderId = "sellorderid123";
            int buyAccountId = 1;
            int sellAccountId = 2;

            Balance buyXbtBalance = new Balance(new Currency(baseCurrency), new AccountId(buyAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(buyXbtBalance);
            Balance buyUsdBalance = new Balance(new Currency(quoteCurrency), new AccountId(buyAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(buyUsdBalance);

            Balance sellXbtBalance = new Balance(new Currency(baseCurrency), new AccountId(sellAccountId), 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(sellXbtBalance);
            Balance sellUsdBalance = new Balance(new Currency(quoteCurrency), new AccountId(sellAccountId), 50000, 50000);
            fundsPersistenceRepository.SaveOrUpdate(sellUsdBalance);

            // Get the fee corresponding to the currenct volume of the quote currency
            decimal usdFee = feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                new AccountId(buyAccountId), 100, 100);
            Assert.Greater(usdFee, 0);

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
            decimal buyerBaseAccountBalance = ledgerRepository.GetBalanceForCurrency(baseCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyXbtBalance.CurrentBalance, buyerBaseAccountBalance);
            Assert.AreEqual(4000 + 100, buyXbtBalance.CurrentBalance);
            Assert.AreEqual(4000 + 100, buyXbtBalance.AvailableBalance);

            buyUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                                                                   new AccountId(buyAccountId));
            decimal buyerQuoteAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(buyAccountId));
            Assert.AreEqual(buyUsdBalance.CurrentBalance, buyerQuoteAccountBalance);
            Assert.AreEqual(50000 - (100 * 100) - usdFee, buyUsdBalance.CurrentBalance);
            Assert.AreEqual(50000 - (100 * 100) - usdFee, buyUsdBalance.AvailableBalance);

            // ----- Trade 2-----

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
            decimal sellerBaseAccountBalance = ledgerRepository.GetBalanceForCurrency(baseCurrency, new AccountId(sellAccountId));
            Assert.AreEqual(sellXbtBalance.CurrentBalance, sellerBaseAccountBalance);

            sellUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(quoteCurrency),
                                                                   new AccountId(sellAccountId));
            decimal sellerQuoteAccountBalance = ledgerRepository.GetBalanceForCurrency(quoteCurrency, new AccountId(sellAccountId));
            Assert.AreEqual(sellUsdBalance.CurrentBalance, sellerQuoteAccountBalance);
        }

        #endregion Trade Tests

        #region Withdrawal Confirmed Tests

        [Test]
        public void WithdrawalConfirmedPassTest_TestsIfTheWithdrawalOperationProceedsAsExpected_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];            
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];           
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            Balance balance = new Balance(currency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            Withdraw validateFundsForWithdrawal = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency,
                0.4m, new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            Assert.IsNotNull(validateFundsForWithdrawal);
            bool withdrawalExecuted = fundsValidationService.WithdrawalExecuted(validateFundsForWithdrawal);
            Assert.IsTrue(withdrawalExecuted);

            WithdrawFees withdrawFee = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(20 - 0.4m - withdrawFee.Fee, balance.CurrentBalance);
            Assert.AreEqual(20 - 0.4m - withdrawFee.Fee, balance.AvailableBalance);
            Assert.AreEqual(0, balance.PendingBalance);
        }

        #endregion Withdrawal Confirmed Tests

        #region Order Cancelled Tests

        [Test]
        public void BuyOrderCancelledFundsRestoreTest_TestsIfTheFundsAreRestoredAsExpectedInCaseOfAnOrderCancelled_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];            
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];            
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];

            AccountId accountId = new AccountId(123);

            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(feeRepository.GetAllFees().First().CurrencyPair);
            Currency baseCurrency = new Currency(splittedCurrencyPair.Item1);
            Currency quoteCurrency = new Currency(splittedCurrencyPair.Item2);
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 5000, 5000);
            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 20000, 20000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 100, 90, "buy", "orderid123");
            Assert.IsTrue(validateFundsForOrder);

            Balance retrievedXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(retrievedXbtBalance);

            Assert.AreEqual(5000, retrievedXbtBalance.AvailableBalance);
            Assert.AreEqual(5000, retrievedXbtBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedXbtBalance.PendingBalance);

            Balance retrievedUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.IsNotNull(retrievedUsdBalance);

            Assert.AreEqual(20000 - 100 * 90, retrievedUsdBalance.AvailableBalance);
            Assert.AreEqual(20000, retrievedUsdBalance.CurrentBalance);
            Assert.AreEqual(100*90, retrievedUsdBalance.PendingBalance);

            bool orderCancelled = fundsValidationService.OrderCancelled(baseCurrency, quoteCurrency, accountId, 
                "buy", "orderid123", 100, 90);
            Assert.IsTrue(orderCancelled);

            // Get the fee corresponding to the current volume of the quote currency
            decimal usdFee = feeCalculationService.GetFee(baseCurrency, quoteCurrency, accountId, 100, 90);
            Assert.Greater(usdFee, 0);

            retrievedXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(retrievedXbtBalance);

            Assert.AreEqual(5000, retrievedXbtBalance.AvailableBalance);
            Assert.AreEqual(5000, retrievedXbtBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedXbtBalance.PendingBalance);

            retrievedUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.IsNotNull(retrievedUsdBalance);

            Assert.AreEqual(20000, retrievedUsdBalance.AvailableBalance);
            Assert.AreEqual(20000, retrievedUsdBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedUsdBalance.PendingBalance);
        }

        [Test]
        public void SellOrderCancelledFundsRestoreTest_TestsIfTheFundsAreRestoredAsExpectedInCaseOfAnOrderCancelled_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];

            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(feeRepository.GetAllFees().First().CurrencyPair);
            Currency baseCurrency = new Currency(splittedCurrencyPair.Item1);
            Currency quoteCurrency = new Currency(splittedCurrencyPair.Item2);
            AccountId accountId = new AccountId(1);

            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 5000, 5000);
            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 20000, 20000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 100, 90, "sell", "orderid123");
            Assert.IsTrue(validateFundsForOrder);

            Balance retrievedXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(retrievedXbtBalance);

            Assert.AreEqual(5000 - 100, retrievedXbtBalance.AvailableBalance);
            Assert.AreEqual(5000, retrievedXbtBalance.CurrentBalance);
            Assert.AreEqual(100, retrievedXbtBalance.PendingBalance);

            Balance retrievedUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.IsNotNull(retrievedUsdBalance);

            Assert.AreEqual(20000, retrievedUsdBalance.AvailableBalance);
            Assert.AreEqual(20000, retrievedUsdBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedUsdBalance.PendingBalance);

            bool orderCancelled = fundsValidationService.OrderCancelled(baseCurrency, quoteCurrency, accountId, "sell", 
                "orderid123", 100, 90);
            Assert.IsTrue(orderCancelled);

            // Get the fee corresponding to the current volume of the quote currency
            decimal usdFee = feeCalculationService.GetFee(baseCurrency, quoteCurrency, accountId, 100, 90);
            Assert.Greater(usdFee, 0);

            retrievedXbtBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(retrievedXbtBalance);

            Assert.AreEqual(5000, retrievedXbtBalance.AvailableBalance);
            Assert.AreEqual(5000, retrievedXbtBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedXbtBalance.PendingBalance);

            retrievedUsdBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.IsNotNull(retrievedUsdBalance);

            Assert.AreEqual(20000, retrievedUsdBalance.AvailableBalance);
            Assert.AreEqual(20000, retrievedUsdBalance.CurrentBalance);
            Assert.AreEqual(0, retrievedUsdBalance.PendingBalance);
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
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositLimitEvaluationService depositLimitEvaluationService = (IDepositLimitEvaluationService)ContextRegistry.GetContext()["DepositLimitEvaluationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = (IWithdrawLimitEvaluationService)ContextRegistry.GetContext()["WithdrawLimitEvaluationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            ITierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];
            IBboCrossContextService bboRetrievalService = (IBboCrossContextService)ContextRegistry.GetContext()["BboCrossContextService"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            ITierValidationService tierValidationService = (ITierValidationService)ContextRegistry.GetContext()["TierValidationService"];

            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC");
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
                withdrawFeesRepository, withdrawIdGeneratorService, ledgerRepository, depositLimitEvaluationService,
                depositLimitRepository, withdrawLimitEvaluationService, withdrawLimitRepository, tierLevelRetrievalService,
                bboRetrievalService, withdrawRepository, tierValidationService);
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
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];
            IWithdrawIdGeneratorService withdrawIdGeneratorService = (IWithdrawIdGeneratorService)ContextRegistry.GetContext()["WithdrawIdGeneratorService"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositLimitEvaluationService depositLimitEvaluationService = (IDepositLimitEvaluationService)ContextRegistry.GetContext()["DepositLimitEvaluationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = (IWithdrawLimitEvaluationService)ContextRegistry.GetContext()["WithdrawLimitEvaluationService"];
            IWithdrawLimitRepository withdrawLimitRepository = (IWithdrawLimitRepository)ContextRegistry.GetContext()["WithdrawLimitRepository"];
            ITierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];
            IBboCrossContextService bboRetrievalService = (IBboCrossContextService)ContextRegistry.GetContext()["BboCrossContextService"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            ITierValidationService tierValidationService = (ITierValidationService)ContextRegistry.GetContext()["TierValidationService"];
            
            AccountId accountId = new AccountId(123);
            Currency baseCurrency = new Currency("BTC", true);
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 20, 20);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 0.9m, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));

            // Here we create a service without using the spring dependency injection to create a new instance
            IFundsValidationService fundsValidationService2 = new FundsValidationService(transactionService,
                fundsPersistenceRepository, balanceRepository, feeCalculationService, withdrawFeesRepository,
                withdrawIdGeneratorService, ledgerRepository, depositLimitEvaluationService, depositLimitRepository, 
                withdrawLimitEvaluationService, withdrawLimitRepository, tierLevelRetrievalService, bboRetrievalService,
                withdrawRepository, tierValidationService);
            fundsValidationService2.ValidateFundsForWithdrawal(accountId, baseCurrency, 0.1m, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));

            WithdrawFees withdrawFees = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(baseCurrency.Name);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(20, balance.CurrentBalance);
            Assert.AreEqual(19 - (withdrawFees.Fee * 2), balance.AvailableBalance);
        }

        #endregion Transaction Jobs Tests
    }
}
