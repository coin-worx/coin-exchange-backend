using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class EndToEndScenarioTests
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

        #region Order Scenario Tests

        [Test]
        public void SendMultipleOrdersTest_TestsIfSendingMultipleOrdersIsHandledAsExpected_VerifiesThroughTheReturnesValue()
        {
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];

            AccountId accountId = new AccountId("accountid123");
            Currency baseCurrency = new Currency("XBT");
            Currency quoteCurrency = new Currency("USD");
            Balance baseCurrencyBalance = new Balance(baseCurrency, accountId, 400, 400);
            fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);

            Balance quoteCurrencyBalance = new Balance(quoteCurrency, accountId, 40000, 40000);
            fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(accountId, baseCurrency,
                quoteCurrency, 40, 100, "buy", "order123");
            Assert.IsTrue(validateFundsForOrder);

            // Get the fee corresponding to the current volume of the quote currency
            double usdFee = feeCalculationService.GetFee(baseCurrency, quoteCurrency, 40 * 100);
            Assert.Greater(usdFee, 0);

            baseCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, accountId);
            Assert.AreEqual(400, baseCurrencyBalance.CurrentBalance);
            Assert.AreEqual(400, baseCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, baseCurrencyBalance.PendingBalance);

            quoteCurrencyBalance = balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, accountId);
            Assert.AreEqual(40000 - (40 * 100), quoteCurrencyBalance.CurrentBalance);
            Assert.AreEqual(400, quoteCurrencyBalance.AvailableBalance);
            Assert.AreEqual(0, quoteCurrencyBalance.PendingBalance);
            
        }

        #endregion Order Scenario Tests

        #region Deposit and WIthdrawal Tests

        [Test]
        public void DepositAndWithdrawTest_TestsIfThingsGoAsExpectedWhenWithdrawIsMadeAfterDeposit_ChecksBalanceToVerify()
        {
            // Scenario: Confirmed Deposit --> Withdraw --> Check Balance            
            IFundsValidationService fundsValidationService = (IFundsValidationService)ContextRegistry.GetContext()["FundsValidationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];            
            IDepositIdGeneratorService depositIdGeneratorService = (IDepositIdGeneratorService)ContextRegistry.GetContext()["DepositIdGeneratorService"];
            IWithdrawFeesRepository withdrawFeesRepository = (IWithdrawFeesRepository)ContextRegistry.GetContext()["WithdrawFeesRepository"];

            AccountId accountId = new AccountId("accountid123");
            Currency currency = new Currency("XBT");

            // Deposit
            Deposit deposit = new Deposit(currency, depositIdGeneratorService.GenerateId(), DateTime.Now,
                                          DepositType.Default, 500, 0, TransactionStatus.Pending, accountId,
                                          new TransactionId("123"), new BitcoinAddress("bitcoin123"));
            deposit.IncrementConfirmations(7);
            fundsPersistenceRepository.SaveOrUpdate(deposit);

            // Retrieve balance
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
            bool depositResponse = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(depositResponse);

            // Check balance
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(balance.CurrentBalance, deposit.Amount);
            Assert.AreEqual(balance.AvailableBalance, deposit.Amount);
            Assert.AreEqual(balance.PendingBalance, 0);

            // Withdraw
            Withdraw validateFundsForWithdrawal = fundsValidationService.ValidateFundsForWithdrawal(accountId, currency, 400, new TransactionId("transaction123"), new BitcoinAddress("bitcoin123"));
            Assert.IsNotNull(validateFundsForWithdrawal);
            bool withdrawalExecuted = fundsValidationService.WithdrawalExecuted(validateFundsForWithdrawal);
            Assert.IsTrue(withdrawalExecuted);

            WithdrawFees withdrawFee = withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);

            // Check balance
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(2400 - 400 - withdrawFee.Fee, balance.CurrentBalance);
            Assert.AreEqual(2400 - 400 - withdrawFee.Fee, balance.AvailableBalance);
            Assert.AreEqual(0, balance.PendingBalance);
        }

        #endregion Deposit and WIthdrawal Tests
    }
}
