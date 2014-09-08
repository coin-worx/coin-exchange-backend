using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Application.WithdrawServices.Commands;
using CoinExchange.Funds.Application.WithdrawServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class WithdrawApplicationServiceIntegrationTests
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailedTest_TestsIfWithdrawFailsWhenTirLevelIsNotHighEnough_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            decimal amount = 1.02m;

            withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name,
                                                                                  currency.IsCryptoCurrency,
                                                                                  bitcoinAddress.Value, amount));
        }

        [Test]
        public void WithdrawSuccessfulTest_TestsIfWithdrawIsSuccessfulWhenTierLevelIsHighEnough_VerifiesThroughDatabaseQueries()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            IWithdrawRepository withdrawRepository = (IWithdrawRepository)ContextRegistry.GetContext()["WithdrawRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            decimal amount = 1.02m;

            Balance balance = new Balance(currency, accountId, amount + 1, amount + 1);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            CommitWithdrawResponse commitWithdrawResponse = withdrawApplicationService.CommitWithdrawal(new CommitWithdrawCommand(accountId.Value, currency.Name, currency.IsCryptoCurrency, bitcoinAddress.Value, amount));
            Assert.IsNotNull(commitWithdrawResponse);
            Assert.IsTrue(commitWithdrawResponse.CommitSuccessful);

            Withdraw withdraw = withdrawRepository.GetWithdrawByWithdrawId(commitWithdrawResponse.WithdrawId);
            Assert.IsNotNull(withdraw);
            Assert.AreEqual(accountId.Value, withdraw.AccountId.Value);
            Assert.AreEqual(currency.Name, withdraw.Currency.Name);
            Assert.AreEqual(currency.IsCryptoCurrency, withdraw.Currency.IsCryptoCurrency);
            Assert.AreEqual(amount, withdraw.Amount);
            Assert.AreEqual(TransactionStatus.Pending, withdraw.Status);
        }
    }
}
