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
            
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // We have suficient balance for this case
            decimal availableBalance = (5000/midpoint) + 100;
            decimal currentBalance = (5000/midpoint) + 100;

            List<Ledger> ledgers = new List<Ledger>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(900, ledgers, 
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((1000/bestBid) + (1000/bestAsk))/2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario2NotEnoughBalanceTest_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Balance is less than the evaluated Maximum Withdrawal threshold
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;

            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000/bestAsk)) / 2 ) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(999, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance,5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario3Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 0/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-40), LedgerType.Withdrawal, currency, 1.5m,
                900, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(999, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario4Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4500 * midpoint,
                4500, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario5Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4100 * midpoint,
                4100, 0, currentBalance, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid1", DateTime.Now.AddMinutes(-5), LedgerType.Withdrawal, currency, 400 * midpoint,
                400, 0, currentBalance, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario6Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4100 * midpoint,
                4100, 0, currentBalance, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid12", DateTime.Now.AddMinutes(-5), LedgerType.Withdrawal, currency, 400 * midpoint,
                400, 0, currentBalance, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario7Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4000/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 3500 * midpoint,
                3500, 0, currentBalance, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-5), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, currentBalance, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario8Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddMinutes(-5), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, currentBalance, null, null, withdrawId, null, accountId);
            
            ledgers.Add(ledger);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario9Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4500/5000 
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((1000 / bestBid) + (1000 / bestAsk)) / 2) - 0.09m;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-3), LedgerType.Withdrawal, currency, 4000 * midpoint,
                4000, 0, currentBalance, null, null, withdrawId, null, accountId);            
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-5), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, currentBalance, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(600, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((500 / bestBid) + (500 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario10Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance);

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4500 * midpoint,
                4500, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5), 
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario11Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold
            decimal availableBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance);

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4100 * midpoint,
                4100, 0, 1.5m, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-29), LedgerType.Withdrawal, currency, 400 * midpoint,
                400, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount-10, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario12Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4000/5000 with insufficient balance
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance);

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 3500 * midpoint,
                3500, 0, 1.5m, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-29), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount - 10, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario13Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 500/5000 with insufficient balance
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance);

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 0,
                0, 0, 1.5m, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-29), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount - 10, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario14Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4500/5000  with insufficient balance
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string withdrawId = "withdrawid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // Balance is less than the calculated maximum threshold foor the remaining quantity from the daily limit used
            decimal availableBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            decimal currentBalance = (((500 / bestBid) + (500 / bestAsk)) / 2) - 0.09m;
            // Amount that can be safely withdrawn
            decimal safeAmount = ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance);

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Withdrawal, currency, 4000 * midpoint,
                4000, 0, 1.5m, null, null, withdrawId, null, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-29), LedgerType.Withdrawal, currency, 500 * midpoint,
                500, 0, 1.5m, null, null, withdrawId, null, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(500, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);

            // Withdraw below the threshold limit
            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(safeAmount - 10, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), 5),
                withdrawLimitEvaluationService.MaximumWithdrawUsd);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(0, withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario15Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Check to see the withheld and withheld converted values
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();

            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // We have suficient balance for this case
            decimal availableBalance = (1000 / midpoint) + 100;
            decimal currentBalance = (1000 / midpoint) + 110;

            List<Ledger> ledgers = new List<Ledger>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);
            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(900, ledgers,
                withdrawLimit, bestBid, bestAsk, availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(((1000 / bestBid) + (1000 / bestAsk)) / 2, withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(currentBalance - availableBalance, 5), withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(ConvertCurrencyToUsd(bestBid, bestAsk, Math.Round(currentBalance - availableBalance, 5)), 
                withdrawLimitEvaluationService.WithheldConverted);
        }

        [Test]
        public void WithdrawLimitScenario16Test_ChecksIfTheScenarioProceedsAsExpected_VerifiesThroughReturnedValue()
        {
            // Check to see the withheld and withheld converted values
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService = new WithdrawLimitEvaluationService();

            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;
            // We have insufficient balance for this case
            decimal availableBalance = (1000 / midpoint) - 1.5m;
            decimal currentBalance = (1000 / midpoint) - 1;

            List<Ledger> ledgers = new List<Ledger>();
            WithdrawLimit withdrawLimit = new WithdrawLimit("Tier0", dailyLimit, monthlyLimit);

            bool evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(
                ConvertCurrencyToUsd(bestBid, bestAsk, currentBalance), ledgers, withdrawLimit, bestBid, bestAsk, 
                availableBalance, currentBalance);
            Assert.IsFalse(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(currentBalance - availableBalance, 5), withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(ConvertCurrencyToUsd(bestBid, bestAsk, Math.Round(currentBalance - availableBalance, 5)),
                withdrawLimitEvaluationService.WithheldConverted);

            evaluationResponse = withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(
                ConvertCurrencyToUsd(bestBid, bestAsk, availableBalance), ledgers, withdrawLimit, bestBid, bestAsk, 
                availableBalance, currentBalance);
            Assert.IsTrue(evaluationResponse);

            Assert.AreEqual(1000, withdrawLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, withdrawLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, withdrawLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, withdrawLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(availableBalance, 5), withdrawLimitEvaluationService.MaximumWithdraw);
            Assert.AreEqual(Math.Round(currentBalance - availableBalance, 5), withdrawLimitEvaluationService.WithheldAmount);
            Assert.AreEqual(ConvertCurrencyToUsd(bestBid, bestAsk, Math.Round(currentBalance - availableBalance, 5)),
                withdrawLimitEvaluationService.WithheldConverted);
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
