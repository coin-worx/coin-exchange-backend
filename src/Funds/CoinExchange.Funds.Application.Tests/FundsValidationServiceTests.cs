using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
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
            TransactionService transactionService = new TransactionService(mockFundsRepository, new 
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService, 
                mockFundsRepository, new MockBalanceRepository());

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"), 
                new Currency("XBT"), new Currency("USD"),  300, 101, "buy");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidationForBuyOrderPassTest_TestsIfValidationOfFundsPassesAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 30, 101, "buy");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidationForBuyOrderFailDueToinSufficientBalanceTest_TestsIfValidationOfFundsFailsAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 2000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 30, 100, "buy");
            Assert.IsFalse(validateFundsForOrder);
        }

        [Test]
        public void ValidationForSellOrderPassTest_TestsIfValidationOfFundsPassesAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 1000, 1000);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 4000);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 999, 101, "sell");
            Assert.IsTrue(validateFundsForOrder);
        }

        [Test]
        public void ValidationForSellOrderFailDueToinSufficientBalanceTest_TestsIfValidationOfFundsFailsAsExpected_VerifiesThroughTheResponseReturned()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            Balance usdBalance = new Balance(new Currency("USD"), new AccountId("accountid123"), 4000, 20005);
            mockBalanceRepository.AddBalance(usdBalance);

            bool validateFundsForOrder = fundsValidationService.ValidateFundsForOrder(new AccountId("accountid123"),
                new Currency("XBT"), new Currency("USD"), 101, 100, "sell");
            Assert.IsFalse(validateFundsForOrder);
        }

        #region Withdraw Unit Tests

        [Test]
        public void WithdrawSuccessTest_TestsIfWithdrawValidationisReturnedTrueWhenSufficientBalanceIsAvailable_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            bool withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                75);
            Assert.IsTrue(withdrawalResponse);
        }

        [Test]
        public void WithdrawFailTest_TestsIfWithdrawValidationisReturnedTrueWhenSufficientBalanceIsNotAvailable_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, mockBalanceRepository);

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            bool withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                1000);
            Assert.IsFalse(withdrawalResponse);
        }

        #endregion Withdraw Unit Tests
    }
}
