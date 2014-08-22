using System;
using System.Collections.Generic;
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

        // As these tests need bitcoins to run from the test account, or may be even real bitcoins, this flag is for 
        // safety and must be set to true to run tests related to deposits and withdrawals
        private bool _shouldRunTests = false;

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
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["CoinClientService"];

                decimal fee = 0.0001m;
                AccountId accountId = new AccountId(1);
                string newAddress = coinClientService.CreateNewAddress("BTC");
                Withdraw withdraw = new Withdraw(new Currency("BTC", true), Guid.NewGuid().ToString(), DateTime.Now,
                                                 WithdrawType.Default, Amount, Amount*585, fee,
                                                 TransactionStatus.Pending, accountId,
                                                 new BitcoinAddress(newAddress));
                bool commitWithdraw = coinClientService.CommitWithdraw(withdraw);
                Assert.IsTrue(commitWithdraw);
                Assert.IsNotNull(withdraw.TransactionId);
            }
        }

        [Test]
        public void CheckBalanceTest_TestIfBalanceIsCheckedAndReturnedProperly_VerifiesThroughReturnedValue()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];

            decimal checkBalance = coinClientService.CheckBalance("BTC");

            Assert.AreNotEqual(0, checkBalance);
        }

        [Test]
        public void DepositPollingTest_TestIfDepositHandlingIsDoneAsExpected_VerifiesThroughReturnedValue()
        {
            if (_shouldRunTests)
            {
                // Checks for confirmations of a transaction that was made way back, and sees whether confirmations are 
                // updated in the deposit in database.
                // Use of a confirmed transaction ID, to make sure we are checking a real transaction. 

                // The Client Service will itself check for polling when the polling interval elapses
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["CoinClientService"];
                IFundsPersistenceRepository fundsPersistenceRepository =
                    (IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];
                IDepositRepository depositRepository =
                    (IDepositRepository) ContextRegistry.GetContext()["DepositRepository"];

                string newAddress = coinClientService.CreateNewAddress("BTC");
                Currency currency = new Currency("BTC", true);
                AccountId accountId = new AccountId(1);
                BitcoinAddress bitcoinAddress = new BitcoinAddress(newAddress);
                TransactionId transactionId =
                    new TransactionId("9c67cd6b30b1f5a83d640877ea454f72db9c86837ce857d1591fc401796c3011");

                DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New,
                                                                   DateTime.Now,
                                                                   accountId);
                fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                Deposit deposit = new Deposit(currency, "123",
                                              DateTime.Now, DepositType.Default, 0.0008m, 0, TransactionStatus.Pending,
                                              accountId, transactionId,
                                              new BitcoinAddress("123"));
                fundsPersistenceRepository.SaveOrUpdate(deposit);

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                // Poll checks for confirmations in the second interval
                manualResetEvent.WaitOne(Convert.ToInt32((coinClientService.PollingInterval)*2) + 10000);
                Deposit depositByTransactionId = depositRepository.GetDepositByTransactionId(transactionId);
                Assert.IsNotNull(depositByTransactionId);
                Assert.Greater(depositByTransactionId.Confirmations, 7);
            }
        }

        [Test]
        public void DepositCheckNewTransactionsTest_TestIfDepositHandlingIsDoneAsExpected_VerifiesThroughReturnedValue()
        {
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["CoinClientService"];
                IFundsPersistenceRepository fundsPersistenceRepository =
                    (IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];
                IDepositAddressRepository depositAddressRepository =
                    (IDepositAddressRepository) ContextRegistry.GetContext()["DepositAddressRepository"];
                IDepositRepository depositRepository =
                    (IDepositRepository) ContextRegistry.GetContext()["DepositRepository"];

                Currency currency = new Currency("BTC", true);
                AccountId accountId = new AccountId(1);
                string newAddress = coinClientService.CreateNewAddress("BTC");
                BitcoinAddress bitcoinAddress = new BitcoinAddress(newAddress);
                DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress,
                                                                   AddressStatus.New, DateTime.Now, accountId);
                fundsPersistenceRepository.SaveOrUpdate(depositAddress);

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                // Wait for hte first interval to elapse, and then withdraw, because ony then we will be able to figure if a 
                // new transaction has been received
                manualResetEvent.WaitOne(Convert.ToInt32(coinClientService.PollingInterval + 3000));
                Withdraw withdraw = new Withdraw(currency, Guid.NewGuid().ToString(), DateTime.Now, WithdrawType.Default,
                                                 Amount, Amount*585, 0.001m, TransactionStatus.Pending, accountId,
                                                 new BitcoinAddress(newAddress));
                bool commitWithdraw = coinClientService.CommitWithdraw(withdraw);
                Assert.IsTrue(commitWithdraw);

                manualResetEvent.Reset();
                bool eventFired = false;
                coinClientService.DepositArrived += delegate()
                                                        {
                                                            eventFired = true;
                                                            manualResetEvent.Set();
                                                        };
                // Wait for a safe span of time taking the polling intervazl into consideration
                manualResetEvent.WaitOne();

                Assert.IsTrue(eventFired);

                Assert.IsFalse(string.IsNullOrEmpty(withdraw.TransactionId.Value));
                depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
                Assert.IsNotNull(depositAddress);
                Assert.AreEqual(AddressStatus.Used, depositAddress.Status);

                Deposit deposit = depositRepository.GetDepositsByBitcoinAddress(bitcoinAddress);
                Assert.IsNotNull(deposit);
                Assert.AreEqual(Amount, deposit.Amount);
                Assert.AreEqual(currency.Name, deposit.Currency.Name);
                Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            }
        }

        [Test]
        public void CompleteReceiveDepositTest_TestIfDepositIsReceivedProperlyAndDepositInstanceIsCreated_VerifiesThroughReturnedValue()
        {
            if (_shouldRunTests)
            {
                ICoinClientService coinClientService =
                    (ICoinClientService) ContextRegistry.GetContext()["CoinClientService"];
                IFundsPersistenceRepository fundsPersistenceRepository =
                    (IFundsPersistenceRepository) ContextRegistry.GetContext()["FundsPersistenceRepository"];
                IDepositAddressRepository depositAddressRepository =
                    (IDepositAddressRepository) ContextRegistry.GetContext()["DepositAddressRepository"];
                IDepositRepository depositRepository =
                    (IDepositRepository) ContextRegistry.GetContext()["DepositRepository"];

                Currency currency = new Currency("BTC", true);
                AccountId accountId = new AccountId(1);
                string newAddress = coinClientService.CreateNewAddress("BTC");
                BitcoinAddress bitcoinAddress = new BitcoinAddress(newAddress);
                DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress,
                                                                   AddressStatus.New, DateTime.Now, accountId);
                fundsPersistenceRepository.SaveOrUpdate(depositAddress);

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                // Wait for the first interval to elapse, and then withdraw, because ony then we will be able to figure if a 
                // new transaction has been received
                manualResetEvent.WaitOne(Convert.ToInt32(coinClientService.PollingInterval + 3000));
                Withdraw withdraw = new Withdraw(currency, Guid.NewGuid().ToString(), DateTime.Now, WithdrawType.Default,
                                                 Amount, Amount*585, 0.001m, TransactionStatus.Pending, accountId,
                                                 new BitcoinAddress(newAddress));
                bool commitWithdraw = coinClientService.CommitWithdraw(withdraw);
                Assert.IsTrue(commitWithdraw);

                manualResetEvent.Reset();
                bool eventFired = false;
                coinClientService.DepositArrived += delegate()
                                                        {
                                                            eventFired = true;
                                                            manualResetEvent.Set();
                                                        };
                // Wait for a safe span of time taking the polling intervazl into consideration
                manualResetEvent.WaitOne();

                Assert.IsTrue(eventFired);

                Assert.IsFalse(string.IsNullOrEmpty(withdraw.TransactionId.Value));
                depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
                Assert.IsNotNull(depositAddress);
                Assert.AreEqual(AddressStatus.Used, depositAddress.Status);

                Deposit deposit = depositRepository.GetDepositsByBitcoinAddress(bitcoinAddress);
                Assert.IsNotNull(deposit);
                Assert.AreEqual(Amount, deposit.Amount);
                Assert.AreEqual(currency.Name, deposit.Currency.Name);
                Assert.AreEqual(TransactionStatus.Pending, deposit.Status);

                manualResetEvent.Reset();
                TransactionId transactionId =
                    new TransactionId("9c67cd6b30b1f5a83d640877ea454f72db9c86837ce857d1591fc401796c3011");

                DepositAddress depositAddress2 = new DepositAddress(currency, new BitcoinAddress("bitcoinaddress1"),
                                                                    AddressStatus.New, DateTime.Now,
                                                                    accountId);
                fundsPersistenceRepository.SaveOrUpdate(depositAddress2);
                deposit = new Deposit(currency, "123",
                                      DateTime.Now, DepositType.Default, 0.0008m, 0, TransactionStatus.Pending,
                                      accountId, transactionId,
                                      new BitcoinAddress("123"));
                fundsPersistenceRepository.SaveOrUpdate(deposit);

                manualResetEvent.WaitOne(Convert.ToInt32(coinClientService.PollingInterval + 3000));


                depositAddress2 = depositAddressRepository.GetDepositAddressByAddress(depositAddress2.BitcoinAddress);
                Assert.IsNotNull(depositAddress2);
                Deposit depositByTransactionId = depositRepository.GetDepositByTransactionId(transactionId);
                Assert.IsNotNull(depositByTransactionId);
                Assert.Greater(depositByTransactionId.Confirmations, 7);
            }
        }
    }
}