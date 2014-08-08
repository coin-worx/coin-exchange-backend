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
        public void DepositEvaluationScenario1DummyTest_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 0/5000
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            double bestBid = 580;
            double bestAsk = 590;
            double dailyLimit = 1000;
            double monthlyLimit = 5000;
            double originalMidpoint = ((dailyLimit / bestBid) + (dailyLimit / bestAsk)) / 2;
            double midpoint = (bestBid + bestAsk)/2;

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
            Ledger ledger = new Ledger("ledgeris1", DateTime.Now.AddMinutes(-1), LedgerType.Deposit, currency, 1.5,
                900, 0, 1.5, null, null, null, depositId, accountId);
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
        public void DepositEvaluationScenario1Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDOneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 0/5000
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            double bestBid = 580;
            double bestAsk = 590;
            double dailyLimit = 1000;
            double monthlyLimit = 5000;
            double originalMidpoint = ((dailyLimit/bestBid) + (dailyLimit/bestAsk))/2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger = new Ledger("ledgeris1", DateTime.Now.AddDays(-40), LedgerType.Deposit, currency, 1.5, 0, 
                1.5, null, null, null, depositId, accountId);
            ledgers.Add(ledger);
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(1.65, ledgers, depositLimit, 
                bestBid, bestAsk);
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitEvaluationService.MonthlyLimitUsed);
            Assert.AreEqual(originalMidpoint, depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario2Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDoneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 0/1000, MonthlyLimit = 4500/5000
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            double bestBid = 580;
            double bestAsk = 590;
            double dailyLimit = 1000;
            double monthlyLimit = 5000;
            double bboMidpoint = (bestBid + bestAsk) / 2;
            double originalDailyDepositLimit = ((dailyLimit / bestBid) + (dailyLimit / bestAsk) / 2);
            
            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger1 = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Deposit, currency, 6, 0,
                6.5, null, null, null, depositId, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddDays(-28), LedgerType.Deposit, currency, 2, 0,
                8, null, null, null, depositId, accountId);
            ledgers.Add(ledger1);
            ledgers.Add(ledger2);
            // Given the values, maximum DailyLimit(XBT) ~= 1.7
            // Max. MonthlyLimit(XBT) = 8.5470085
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(0.5, ledgers, depositLimit,
                bestBid, bestAsk);
            
            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            // Convert XBT to USD
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            double monthlyUsedInUsd = 8*bboMidpoint;           
            Assert.AreEqual(monthlyUsedInUsd, depositLimitEvaluationService.MonthlyLimitUsed);

            double monthlyDifference = monthlyLimit - monthlyUsedInUsd;
            double originalMonthlyDepositLimit = ((monthlyDifference/bestBid)+(monthlyDifference/bestAsk))/2;

            Assert.AreEqual(originalMonthlyDepositLimit, depositLimitEvaluationService.MaximumDeposit);
        }

        [Test]
        public void DepositEvaluationScenario3Test_VerifiesIfTheEvaluationOfTheLimitsIsBeingDoneProperly_VerifiesThourhgPropetyValues()
        {
            // Scenario: DailyLimit = 400/1000, MonthlyLimit = 4500/5000
            IDepositLimitEvaluationService depositLimitEvaluationService = new DepositLimitEvaluationService();
            Currency currency = new Currency("XBT");
            string depositId = "depositid123";
            AccountId accountId = new AccountId("accountid123");
            double bestBid = 580;
            double bestAsk = 590;
            double dailyLimit = 1000;
            double monthlyLimit = 5000;
            double bboMidpoint = (bestBid + bestAsk) / 2;

            List<Ledger> ledgers = new List<Ledger>();
            Ledger ledger1 = new Ledger("ledgerid1", DateTime.Now.AddDays(-29), LedgerType.Deposit, currency, 4, 0,
                4, null, null, null, depositId, accountId);
            Ledger ledger2 = new Ledger("ledgerid2", DateTime.Now.AddDays(-28), LedgerType.Deposit, currency, 4, 0,
                8, null, null, null, depositId, accountId);
            Ledger ledger3 = new Ledger("ledgerid2", DateTime.Now.AddHours(-2), LedgerType.Deposit, currency, 0.5, 0,
                8.5, null, null, null, depositId, accountId);
            ledgers.Add(ledger1);
            ledgers.Add(ledger2);
            ledgers.Add(ledger3);
            // Given the values, maximum DailyLimit(XBT) ~= 1.7
            // Max. MonthlyLimit(XBT) = 8.5470085
            DepositLimit depositLimit = new DepositLimit("Tier 0", dailyLimit, monthlyLimit);
            bool evaluationResponse = depositLimitEvaluationService.EvaluateDepositLimit(0.5, ledgers, depositLimit,
                bestBid, bestAsk);

            Assert.IsTrue(evaluationResponse);
            Assert.AreEqual(1000, depositLimitEvaluationService.DailyLimit);
            Assert.AreEqual(5000, depositLimitEvaluationService.MonthlyLimit);
            // Convert XBT to USD
            Assert.AreEqual(0, depositLimitEvaluationService.DailyLimitUsed);
            double monthlyUsedInUsd = 8 * bboMidpoint;
            Assert.AreEqual(monthlyUsedInUsd, depositLimitEvaluationService.MonthlyLimitUsed);

            double monthlyDifference = monthlyLimit - monthlyUsedInUsd;
            double originalMonthlyDepositLimit = ((monthlyDifference / bestBid) + (monthlyDifference / bestAsk)) / 2;

            Assert.AreEqual(originalMonthlyDepositLimit, depositLimitEvaluationService.MaximumDeposit);
        }
    }
}
