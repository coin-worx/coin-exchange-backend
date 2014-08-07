using System;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Application.Tests
{
    [TestFixture]
    class FundsValidationServiceTests
    {
        [Test]
        public void ValidationForOrderFailTest_TestsIfValidationOfFundsFailsAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new 
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository, 
                mockTierLevelRetrievalService, mockBboRetrievalService);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"), 
                new Currency("XBT"), new Currency("USD"),  300, 101, "buy", "order123");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidationForBuyOrderPassTest_TestsIfValidationOfFundsPassesAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 30, 101, "buy", "order123");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidationForBuyOrderFailDueToinSufficientBalanceTest_TestsIfValidationOfFundsFailsAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 2000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 30, 100, "buy", "order123");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidationForSellOrderPassTest_TestsIfValidationOfFundsPassesAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 1000, 1000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 999, 101, "sell", "order123");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidationForSellOrderFailDueToinSufficientBalanceTest_TestsIfValidationOfFundsFailsAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 20005);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 101, 100, "sell", "order123");
            Assert.IsFalse(validateFundsForOrder);
        }

        #region Withdraw Unit Tests

        [Test]
        public void WithdrawSuccessTest_TestsIfWithdrawValidationisReturnedTrueWhenSufficientBalanceIsAvailable_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Withdraw withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                75, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(withdrawalResponse);
        }

        [Test]
        public void WithdrawFailTest_TestsIfWithdrawValidationisReturnedTrueWhenSufficientBalanceIsNotAvailable_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Withdraw withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                1000, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNull(withdrawalResponse);
        }

        #endregion Withdraw Unit Tests

        #region Deposit Unit Tests

        [Test]
        public void DepositAmountTest_TestsIfDepositTransactionProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            var mockDepositIdGeneratorService = new MockDepositIdGeneratorService();
            var mockDepositRepository = new MockDepositRepository();
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Deposit deposit = new Deposit(balance.Currency, mockDepositIdGeneratorService.GenerateId(), DateTime.Now,
                DepositType.Default, 500, 0, TransactionStatus.Pending, balance.AccountId, new TransactionId("123"),
                new BitcoinAddress("123"));
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            deposit.IncrementConfirmations();
            mockDepositRepository.Save(deposit);
            bool response = fundsValidationService.DepositConfirmed(deposit);
            Assert.IsTrue(response);
            // 3 Object: 1 = Balance, 2 = Deposit, 3 = Ledger
            Assert.AreEqual(3, mockFundsRepository.GetNumberOfObjects());
        }

        [Test]
        public void WithdrawConfirmedTest_TestIfWithdrawalConfirmationExecutesAsExpected_TestsThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var mockDepositLimitEvaluationService = new MockDepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new MockWithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, mockDepositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService);
            Withdraw withdraw = new Withdraw(new Currency("XBT"), "123", DateTime.Now, WithdrawType.Default, 400, 0.4, TransactionStatus.Pending, 
                new AccountId("accountid123"), new TransactionId("123"), new BitcoinAddress("bitcoin123"));

            Balance balance = new Balance(withdraw.Currency, withdraw.AccountId, 400, 800);
            mockBalanceRepository.AddBalance(balance);
            bool withdrawalExecuted = fundsValidationService.WithdrawalExecuted(withdraw);
            Assert.IsTrue(withdrawalExecuted);

            balance = mockBalanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.AreEqual(400, balance.CurrentBalance);
            Assert.AreEqual(400, balance.AvailableBalance);
            Assert.AreEqual(0, balance.PendingBalance);
        }

        #endregion Deposit Unit Tests
    }
}
