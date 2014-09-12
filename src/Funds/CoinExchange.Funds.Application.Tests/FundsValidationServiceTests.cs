using System;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Services;
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository, 
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(123), 
                new Currency("XBT", true), new Currency("USD"),  300, 101, "buy", "order123");
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 4000, 4000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD", false), new AccountId(123), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(123),
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);

            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 40, 40);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD", false), new AccountId(123), 2000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(123),
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 1000, 1000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD", false), new AccountId(123), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(123),
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT"), new AccountId(123), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId(123), 4000, 20005);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId(123),
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Withdraw withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                1.7m, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
            Assert.IsNotNull(withdrawalResponse);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailTest_TestsIfWithdrawValidationisReturnedTrueWhenSufficientBalanceIsNotAvailable_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockFeeCalculationService = new MockFeeCalculationService();
            var mockWithdrawFeesRepository = new MockWithdrawFeesRepository();
            var mockWithdrawIdGeneratorService = new MockWithdrawIdGeneratorService();
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Withdraw withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                100.0002M, new TransactionId("transaction123"), new BitcoinAddress("bitcoinid123"));
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Balance balance = new Balance(new Currency("XBT", true), new AccountId(123), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Deposit deposit = new Deposit(balance.Currency, mockDepositIdGeneratorService.GenerateId(), DateTime.Now,
                DepositType.Default, 1.5m, 0, TransactionStatus.Pending, balance.AccountId, new TransactionId("123"),
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
            var depositLimitEvaluationService = new DepositLimitEvaluationService();
            var mockLedgerRepository = new MockLedgerRepository();
            var mockDepositLimitRepository = new MockDepositLimitRepository();
            var mockWithdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            var mockWithdrawLimitRepository = new MockWithdrawLimitRepository();
            var mockTierLevelRetrievalService = new MockTierLevelRetrievalService();
            var mockBboRetrievalService = new MockBboRetrievalService();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var tierValidationService = new TierValidationService();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository, mockFeeCalculationService, mockWithdrawFeesRepository,
                mockWithdrawIdGeneratorService, mockLedgerRepository, depositLimitEvaluationService,
                mockDepositLimitRepository, mockWithdrawLimitEvaluationService, mockWithdrawLimitRepository,
                mockTierLevelRetrievalService, mockBboRetrievalService, mockWithdrawRepository, tierValidationService);
            Withdraw withdraw = new Withdraw(new Currency("XBT", true), "123", DateTime.Now, WithdrawType.Bitcoin, 0.4m, 
                0.001m, TransactionStatus.Pending, 
                new AccountId(123), new BitcoinAddress("bitcoin123"));

            Balance balance = new Balance(withdraw.Currency, withdraw.AccountId, 400, 800);
            mockBalanceRepository.AddBalance(balance);

            Withdraw withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(withdraw.AccountId, 
            withdraw.Currency, withdraw.Amount, withdraw.TransactionId, withdraw.BitcoinAddress);
            Assert.IsNotNull(withdrawalResponse);
            bool withdrawalExecuted = fundsValidationService.WithdrawalExecuted(withdrawalResponse);
            Assert.IsTrue(withdrawalExecuted);

            Assert.AreEqual(5, mockFundsRepository.GetNumberOfObjects());
        }

        #endregion Deposit Unit Tests
    }
}
