using System;
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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

            Balance balance = new Balance(new Currency("XBT"), new AccountId("accountid123"), 100, 100);
            mockBalanceRepository.AddBalance(balance);

            bool withdrawalResponse = fundsValidationService.ValidateFundsForWithdrawal(balance.AccountId, balance.Currency,
                1000);
            Assert.IsFalse(withdrawalResponse);
        }

        #endregion Withdraw Unit Tests

        #region Deposit Unit Tests

        [Test]
        public void DepositAmountTest_TestsIfDepositTransactionProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockBalanceRepository = new MockBalanceRepository();
            var mockDepositIdGeneratorService = new MockDepositIdGeneratorService();
            var mockDepositRepository = new MockDepositRepository();
            TransactionService transactionService = new TransactionService(mockFundsRepository, new
                MockLedgerGeneratorService(), new MockLedgerRepository(), new MockFeeCalculationService(), new MockBalanceRepository());
            FundsValidationService fundsValidationService = new FundsValidationService(transactionService,
                mockFundsRepository, new MockBalanceRepository(), new MockDepositRepository(), new MockFeeCalculationService());

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

        #endregion Deposit Unit Tests
    }
}
