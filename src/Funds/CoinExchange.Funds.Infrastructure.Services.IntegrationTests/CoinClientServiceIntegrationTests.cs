using System;
using System.Configuration;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastructure.Services.IntegrationTests
{
    [TestFixture]
    class CoinClientServiceIntegrationTests
    {
        // Care must be taken to decide this amount, as this will subtract the currency from the testnet version
        // of bitcoin
        private const decimal Amount = 0.0001m;

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
        public void CreateNewAddressTest_TestsWhetherTheServiceCreatesNewAddressSuccessfullyOrNot_VerifiesByTheReturnedResult()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];

            string newAddress = coinClientService.CreateNewAddress("XBT");
            Assert.IsNotNull(newAddress);
            Assert.IsFalse(string.IsNullOrEmpty(newAddress));
        }

        [Test]
        public void CommitWithdrawTest_TestsIfTheWithdrawalIsMadeAsExpected_VerifiesTheReturnedResponseAndCheskTheBlockChainToConfirm()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];

            decimal fee = 0.0001m;
            AccountId accountId = new AccountId(1);
            string newAddress = coinClientService.CreateNewAddress("BTC");
            Withdraw withdraw = new Withdraw(new Currency("BTC", true), Guid.NewGuid().ToString(), DateTime.Now, 
                WithdrawType.Default, Amount, Amount * 585, fee, TransactionStatus.Pending, accountId, 
                new BitcoinAddress(newAddress));
            bool commitWithdraw = coinClientService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            Assert.IsNotNull(withdraw.TransactionId);
        }

        [Test]
        public void CheckBalanceTest_TestIfBalanceIsCheckedAndReturnedProperly_VerifiesThroughReturnedValue()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];

            decimal checkBalance = coinClientService.CheckBalance("BTC");

            Assert.AreNotEqual(0, checkBalance);
        }

        [Test]
        public void DepositTimerTest_TestIfDepositHandlingIsDoneAsExpected_VerifiesThroughReturnedValue()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            Currency currency = new Currency("BTC", true);
            AccountId accountId = new AccountId(1);
            string newAddress = coinClientService.CreateNewAddress("BTC");
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            int count = 0;
            bool eventFired = false;
            coinClientService.DepositArrived += delegate()
            {
                eventFired = true;
                count++;
                if (count == 3)
                {
                    manualResetEvent.Set();
                }
            };
            DepositAddress depositAddress = new DepositAddress(currency, new BitcoinAddress(newAddress), AddressStatus.New, 
                DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            manualResetEvent.WaitOne(new TimeSpan(0, 10, 0));

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void ReceiveDepositTest_TestIfDepositIsReceivedProperlyAndDepositInstanceIsCreated_VerifiesThroughReturnedValue()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            
            Currency currency = new Currency("BTC", true);
            AccountId accountId = new AccountId(1);
            
            decimal checkBalance = coinClientService.CheckBalance("BTC");
            Assert.AreNotEqual(0, checkBalance);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            int count = 0;
            bool eventFired = false;
            coinClientService.DepositArrived += delegate()
                                                    {
                                                        eventFired = true;
                                                        count++;
                                                        if (count == 3)
                                                        {
                                                            manualResetEvent.Set();
                                                        }
                                                    };
            string newAddress = coinClientService.CreateNewAddress(currency.Name);
            DepositAddress depositAddress = new DepositAddress(currency, new BitcoinAddress(newAddress), AddressStatus.New, 
                DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Withdraw withdraw = new Withdraw(currency, Guid.NewGuid().ToString(), DateTime.Now, 
                WithdrawType.Default, Amount, Amount * 585, 0.001m, TransactionStatus.Pending, accountId, 
                new BitcoinAddress(newAddress));
            // Create the balance and save it first, because only in that case will a BalanceId be generated by the
            // database, and that Id will be used by NHibernate to map pending transactions to this balance
            Balance balance = new Balance(currency, accountId, 0, 0);
            fundsPersistenceRepository.SaveOrUpdate(balance);
            // Add a new pending transaction to this balance, and then save it again.
            balance.AddPendingTransaction(withdraw.WithdrawId, PendingTransactionType.Withdraw, Amount);
            fundsPersistenceRepository.SaveOrUpdate(balance);
            bool commitWithdraw = coinClientService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne(new TimeSpan(0, 2, 0));

            Assert.IsTrue(eventFired);
        }
    }
}