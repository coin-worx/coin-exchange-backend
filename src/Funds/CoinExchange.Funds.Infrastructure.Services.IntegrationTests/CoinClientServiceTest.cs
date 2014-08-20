using System;
using System.Threading;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastructure.Services.IntegrationTests
{
    [TestFixture]
    class CoinClientServiceTest
    {
        // Care must be taken to decide this amount, as this will subtract the currency from the testnet version
        // of bitcoin
        private const decimal Amount = 0.0001m;

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
        public void ReceiveDepositTest_TestIfDepositIsReceivedProperlyAndDepositInstanceIsCreated_VerifiesThroughReturnedValue()
        {
            ICoinClientService coinClientService = (ICoinClientService)ContextRegistry.GetContext()["CoinClientService"];

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

            Assert.IsTrue(eventFired);
        }
    }
}
