using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Domain.Model.Tests
{
    [TestFixture]
    class DepositEvaluationServiceTests
    {
        [Test]
        public void DepositEvaluationScenario1Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal originalMidpoint = ((dailyLimit / bestBid) + (dailyLimit / bestAsk)) / 2;
            decimal midpoint = (bestBid + bestAsk)/2;

            List<Ledger> ledgers = new List<Ledger>();
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(900, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(originalMidpoint, depositLimitEvaluationService.MaximumDeposit);
            Ledger ledger = new Ledger("ledgeris1", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 1.5m,
                900, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(200, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(900, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(900, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(100/midpoint, 3), Math.Round(depositLimitEvaluationService.MaximumDeposit, 3));
        }

        [Test]
        public void DepositEvaluationScenario2Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 0/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal originalMidpoint = ((dailyLimit / bestBid) + (dailyLimit / bestAsk)) / 2;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgeris1", DateTime.Now.AddDays(-40), LedgerType.Deposit, currency, 1.5m,
                900, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger);
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(900, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(originalMidpoint, depositLimitEvaluationService.MaximumDeposit);
            Ledger ledger2 = new Ledger("ledgeris1", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 1.5m,
                900, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger2);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(200, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(900, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(900, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(100 / midpoint, 3), Math.Round(depositLimitEvaluationService.MaximumDeposit, 3));
        }

        [Test]
        public void DepositEvaluationScenario3Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal originalMidpoint = ((dailyLimit / bestBid) + (dailyLimit / bestAsk)) / 2;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Deposit, currency, 4500/midpoint,
                4500, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger);

            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(500, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0.0, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(Math.Round(500/midpoint), Math.Round(depositLimitEvaluationService.MaximumDeposit));
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 500/midpoint,
                500, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger2);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(200, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(0, Math.Round(depositLimitEvaluationService.MaximumDeposit, 3));
        }

        [Test]
        public void DepositEvaluationScenario4Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Deposit, currency, 4100 / midpoint,
                4100, 0, 1.5m, null, null, null, depositId, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 400 / midpoint,
                400, 0, 1.5m, null, null, null, depositId, accountId);            
            ledgers.Add(ledger);
            ledgers.Add(ledger2);

            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(400, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);
            
            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(600, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(500, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);

            Ledger ledger3 = new Ledger("ledgerid3", DateTime.Now.AddHours(-30), LedgerType.Deposit, currency, 500 / midpoint,
                500, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger3);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(1, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(400, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario5Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Monthly Limit Reached, no more deposits
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            
            Ledger ledger3 = new Ledger("ledgerid3", DateTime.Now.AddHours(-30), LedgerType.Deposit, currency, 5000 / midpoint,
                5000, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger3);

            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(1, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario6Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4000/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Deposit, currency, 3500 / midpoint,
                3500, 0, 1.5m, null, null, null, depositId, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 500 / midpoint,
                500, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);

            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(999, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);

            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(500, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4000, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario7Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 500/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 500 / midpoint,
                500, 0, 1.5m, null, null, null, depositId, accountId);           
            ledgers.Add(ledger);

            DepositLimit depositLimit = new DepositLimit("Tier 1", dailyLimit, monthlyLimit);
            // Fail Condition
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(501, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);

            // Pass condition
            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(500, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario8Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 500/1000, MonthlyLimit = 4500/5000 with older deposits present
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT", true);
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            decimal bestBid = 580;
            decimal bestAsk = 590;
            decimal dailyLimit = 1000;
            decimal monthlyLimit = 5000;
            decimal midpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgerid1", DateTime.Now.AddHours(-25), LedgerType.Deposit, currency, 4000 / midpoint,
                4000, 0, 1.5m, null, null, null, depositId, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 500 / midpoint,
                500, 0, 1.5m, null, null, null, depositId, accountId);
            ledgers.Add(ledger);
            ledgers.Add(ledger2);

            DepositLimit depositLimit = new DepositLimit("Tier 1", dailyLimit, monthlyLimit);
            // Fail Condition
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(501, ledgers, depositLimit,
                bestBid, bestAsk);
            Assert.IsFalse(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);

            // Pass condition
            evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(500, ledgers, depositLimit, bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(500, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(4500, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(ConvertUsdToCurrency(bestBid, bestAsk, 500), depositLimitEvaluationService.MaximumDeposit);
        }

        private decimal ConvertUsdToCurrency(decimal bestBid, decimal bestAsk, decimal usdAmount)
        {
            decimal sum = (usdAmount / bestBid) + (usdAmount / bestAsk);
            decimal midPoint = sum / 2;
            return midPoint;
        }
    }
}
