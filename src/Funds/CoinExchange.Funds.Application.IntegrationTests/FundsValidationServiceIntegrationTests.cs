using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 40, 101, "buy");
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 40, 100, "buy");
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 40, 101, "buy");
            Assert.IsFalse(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(quoteCurrency, accountId);
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 40, 90, "buy");
            Assert.IsTrue(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(quoteCurrency, accountId);
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 401, 101, "sell");
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 400, 101, "sell");
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 401, 101, "sell");
            Assert.IsFalse(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(baseCurrency, accountId);
            Assert.AreEqual(baseCurrencyBalance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(baseCurrencyBalance.PendingBalance, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(quoteCurrency, accountId);
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
            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 300, 101, "sell");
            Assert.IsTrue(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(baseCurrency, accountId);
            Assert.AreEqual(100, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(baseCurrencyBalance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(300, baseAfterValidationBalance.PendingBalance);
            Balance quoteAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(quoteCurrency, accountId);
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

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 500);
            Assert.IsFalse(validateFundsForOrder);
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

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, baseCurrency, 101);
            Assert.IsTrue(validateFundsForOrder);
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

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 500);
            Assert.IsFalse(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(currency, accountId);
            Assert.AreEqual(balance.AvailableBalance, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, baseAfterValidationBalance.PendingBalance);
        }

        [Test] public void WithdrawalPassAndBalanceVerificationTest_TestsIfWithdrawValidationReturnsTrueIfBalanceIsSufficient_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");
            Balance balance = new Balance(currency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 101);
            Assert.IsTrue(validateFundsForOrder);

            Balance baseAfterValidationBalance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(currency, accountId);
            Assert.AreEqual(299, baseAfterValidationBalance.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, baseAfterValidationBalance.CurrentBalance);
            Assert.AreEqual(101, baseAfterValidationBalance.PendingBalance);
        }

        #endregion Withdrawal Validation

        #region Transaction Jobs Tests

        [Test]
        public void ValidateOrderTransactionTest_TestsIfTransactioNIntegrityIsMantainedDuringSimultaneuosTransactions_VerifiesThroughDatabaseQuery()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            ITransactionService transactionService = (ITransactionService)ContextRegistry.GetContext()["TransactionService"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 1000, 1000);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 4000, 4000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
            fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 400, 101, "sell");
            fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 300, 101, "sell");

            // Here we create a service without using the spring dependency injection to create a new instance
            IFundsValidationService fundsValidationService2 = new FundsValidationService(transactionService,
                fundsPersistenceRepository, balanceRepository);            
            fundsValidationService2.ValidateFundsForOrder(accountId, baseCurrency, quoteCurrency, 100, 101, "sell");
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccoutnId(baseCurrency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(1000, balance.CurrentBalance);
            Assert.AreEqual(200, balance.AvailableBalance);
        }

        #endregion Transaction Jobs Tests
    }
}
