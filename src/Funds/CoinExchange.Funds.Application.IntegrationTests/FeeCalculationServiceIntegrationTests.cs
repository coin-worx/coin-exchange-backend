using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Common.Utility;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class FeeCalculationServiceIntegrationTests
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

        [Test]
        public void GetFeeTest_TestsIfTheFeeIsasReturnedAsExpectedWhenFinalElementInTheTableIsReached_VerifiesThroughDatabaseQueries()
        {
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            
            AccountId accountId = new AccountId(1);
            List<Fee> allFees = feeRepository.GetAllFees();
            Assert.IsNotNull(allFees);
            string currencyPair = allFees[0].CurrencyPair;
            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(currencyPair);

            // As amount, the value is so large that it is always greater than any fee in the table
            Ledger ledger = new Ledger("1", DateTime.Now, LedgerType.Trade, new Currency(splittedCurrencyPair.Item2), 
                2000000, 0, 0, 0, "1", "1", false, accountId);
            fundsPersistenceRepository.SaveOrUpdate(ledger);
            List<Fee> feeByCurrencyPair = feeRepository.GetFeeByCurrencyPair(currencyPair);
            Assert.IsNotNull(feeByCurrencyPair);
            decimal lastElementFee = (feeByCurrencyPair.Last().PercentageFee / 100) * (1000 * 2000);

            decimal feeFromService = feeCalculationService.GetFee(new Currency(splittedCurrencyPair.Item1),
                                         new Currency(splittedCurrencyPair.Item2),
                                         accountId, 1000, 2000);

            Assert.AreEqual(lastElementFee, feeFromService);
        }

        [Test]
        public void GetFeeTest_TestsIfTheFeeIsasReturnedAsExpectedWhenMiddleElementsAreReached_VerifiesThroughDatabaseQueries()
        {
            IFeeCalculationService feeCalculationService = (IFeeCalculationService)ContextRegistry.GetContext()["FeeCalculationService"];
            IFeeRepository feeRepository = (IFeeRepository)ContextRegistry.GetContext()["FeeRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];


            AccountId accountId = new AccountId(1);
            List<Fee> allFees = feeRepository.GetAllFees();
            Assert.IsNotNull(allFees);
            string currencyPair = allFees[0].CurrencyPair;
            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(currencyPair);

            List<Fee> feeByCurrencyPair = feeRepository.GetFeeByCurrencyPair(currencyPair);
            Assert.IsNotNull(feeByCurrencyPair);

            if (feeByCurrencyPair.Count > 4)
            {
                decimal amount = feeByCurrencyPair[feeByCurrencyPair.Count - 2].Amount;
                Ledger ledger = new Ledger("1", DateTime.Now, LedgerType.Trade, new Currency(splittedCurrencyPair.Item2),
                                           amount, 0, 0, 0, "1", "1", false, accountId);
                fundsPersistenceRepository.SaveOrUpdate(ledger);

                decimal lastElementFee = (feeByCurrencyPair[feeByCurrencyPair.Count - 2].PercentageFee/100)*(10*20);

                decimal feeFromService = feeCalculationService.GetFee(new Currency(splittedCurrencyPair.Item1),
                                                                      new Currency(splittedCurrencyPair.Item2),
                                                                      accountId, 10, 20);
                
                Assert.AreEqual(lastElementFee, feeFromService);
            }
        }

        [Test]
        public void GetFeeTest_TestsIfTheFeeIsasReturnedAsExpectedWhenFirstElementisGreaterThanAmount_VerifiesThroughDatabaseQueries()
        {
            IFeeCalculationService feeCalculationService =
                (IFeeCalculationService) ContextRegistry.GetContext()["FeeCalculationService"];
            IFeeRepository feeRepository = (IFeeRepository) ContextRegistry.GetContext()["FeeRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository =
                (IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];


            AccountId accountId = new AccountId(1);
            List<Fee> allFees = feeRepository.GetAllFees();
            Assert.IsNotNull(allFees);
            string currencyPair = allFees[0].CurrencyPair;
            Tuple<string, string> splittedCurrencyPair = CurrencySplitterService.SplitCurrencyPair(currencyPair);

            List<Fee> feeByCurrencyPair = feeRepository.GetFeeByCurrencyPair(currencyPair);
            Assert.IsNotNull(feeByCurrencyPair);

            decimal amount = feeByCurrencyPair.First().Amount - 1;
            Ledger ledger = new Ledger("1", DateTime.Now, LedgerType.Trade, new Currency(splittedCurrencyPair.Item2),
                                       amount, 0, 0, 0, "1", "1", false, accountId);
            fundsPersistenceRepository.SaveOrUpdate(ledger);

            decimal lastElementFee = (feeByCurrencyPair.First().PercentageFee/100)*(10*20);

            decimal feeFromService = feeCalculationService.GetFee(new Currency(splittedCurrencyPair.Item1),
                                                                  new Currency(splittedCurrencyPair.Item2),
                                                                  accountId, 10, 20);

            Assert.AreEqual(lastElementFee, feeFromService);
        }
    }
}
