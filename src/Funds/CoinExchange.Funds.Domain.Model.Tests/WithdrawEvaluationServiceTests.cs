using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Domain.Model.Tests
{
    [TestFixture]
    class WithdrawEvaluationServiceTests
    {
        [Test]
        public void WithdrawLimitScenario1Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // We have suficient balance for this case
            decimal availableBalance = 5000 + 100;
            decimal currentBalance = 5000 + 100;

            List<Withdraw> ledgers = new List<Withdraw>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(900, ledgers, 
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(1000, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario2NotEnoughBalanceTest_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Balance is less than the evaluated Maximum Withdrawal threshold
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;

            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 1000 - 1.09m;
            decimal currentBalance = 1000 - 1.09m;

            List<Withdraw> ledgers = new List<Withdraw>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(999, ledgers,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance,5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario3Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 0/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 1000 - 1.09m;
            decimal currentBalance = 1000 - 1.09m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-40), WithdrawType.Bitcoin, 500, 
                0.001m, TransactionStatus.Pending, accountId, 
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(999, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario4Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 1000;
            decimal currentBalance = 1000;
            decimal fee = 0.001m;

            List<Withdraw> ledgers = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            ledgers.Add(withdraw);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee, ledgers,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario5Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 1000 - 0.09m;
            decimal currentBalance = 1000 - 0.09m;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4100, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-5), WithdrawType.Bitcoin,
                400, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario6Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal availableBalance = 1000;
            decimal currentBalance = 1000;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4100, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-5), WithdrawType.Bitcoin,
                400, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario7Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4000/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;

            decimal availableBalance = 1000;
            decimal currentBalance = 1000;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                3500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-5), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario8Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 1000;
            decimal currentBalance = 1000;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-5), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600 - fee, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario9Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4500/5000 
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal availableBalance = 1000;
            decimal currentBalance = 1000;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-3), WithdrawType.Bitcoin,
                4000, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-5), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500 - fee*2, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 +fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(500 - fee*2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario10Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 500 - 0.09m;
            decimal currentBalance = 500 - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = availableBalance;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);           
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario11Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000, but Balance is less than withdraw amount
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = 500 - 0.09m;
            decimal currentBalance = 500 - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = availableBalance;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4100, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-29), WithdrawType.Bitcoin,
                400, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);            
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount-10, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario12Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4000/5000, but Balance is less than withdraw amount
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = 500 - 0.09m;
            decimal currentBalance = 500 - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = availableBalance;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                3500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-29), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount - 10, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario13Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 500/5000 with insufficient balance
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = 500 - 0.09m;
            decimal currentBalance = 500 - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = availableBalance;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-29), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario14Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4500/5000  with insufficient balance
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            AccountId accountId = new AccountId(123);
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = 500 - 0.09m;
            decimal currentBalance = 500 - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = availableBalance;
            decimal fee = 0.001m;

            List<Withdraw> withdraws = new List<Withdraw>();
            Withdraw withdraw = new Withdraw(currency, "withdrawid123", DateTime.Now.AddDays(-29), WithdrawType.Bitcoin,
                4000, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            Withdraw withdraw2 = new Withdraw(currency, "withdrawid123", DateTime.Now.AddMinutes(-29), WithdrawType.Bitcoin,
                500, fee, TransactionStatus.Pending, accountId,
                new BitcoinAddress("bitcoin123"));
            withdraws.Add(withdraw);
            withdraws.Add(withdraw2);

            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount - 10, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500 + fee, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500 + fee*2, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario15Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Check to see the withheld amount
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();

            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // We have suficient balance for this case
            decimal availableBalance = 1000 + 100;
            decimal currentBalance = 1000 + 110;

            List<Withdraw> withdraws = new List<Withdraw>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(900, withdraws,
                withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(1000, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(currentBalance - availableBalance, withdrawLimitEvaluationService.WithheldAmount);
        }

        [Test]
        public void WithdrawLimitScenario16Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Check to see the withheld amount
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();

            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // We have insufficient balance for this case
            decimal availableBalance = 1000 - 150;
            decimal currentBalance = 1000 - 100;

            List<Withdraw> withdrawals = new List<Withdraw>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);

            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(
                currentBalance, withdrawals, withdrawLimit, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(availableBalance, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(currentBalance - availableBalance, withdrawLimitEvaluationService.WithheldAmount);
            
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(
                availableBalance, withdrawals, withdrawLimit, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(availableBalance, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(currentBalance - availableBalance, withdrawLimitEvaluationService.WithheldAmount);
        }

        #region Private Methods

        private decimal ConvertCurrencyToUsd(decimal bestBid, decimal bestAsk, decimal currencyAmount)
        {
            decimal sum = (currencyAmount * bestBid) + (currencyAmount * bestAsk);
            decimal midPoint = sum / 2;
            return midPoint;
        }

        #endregion Private Methods
    }
}
