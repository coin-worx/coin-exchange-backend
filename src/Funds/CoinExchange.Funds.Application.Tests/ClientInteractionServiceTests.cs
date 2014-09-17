using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.CrossBoundedContextsServices;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Application.Tests
{
    [TestFixture]
    class ClientInteractionServiceTests
    {
        [Test]
        public void CommitWithdrawSuccessfulTest_ChecksThatWithdrawEventIsSubmittedAsExpectedToTheCoinClientService_VerifiesThroughVariablesValues()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            Withdraw withdraw = new Withdraw(new Currency("BTC", true), "123", DateTime.Now, WithdrawType.Bitcoin, 0.4m, 
                0.001m, TransactionStatus.Pending, 
                new AccountId(123), new BitcoinAddress("bitcoin123"));

            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw withdraw2 = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw receivedWithdraw)
                                                             {
                                                                 eventFired = true;
                                                                 withdraw2 = receivedWithdraw;
                                                                 manualResetEvent.Set();
                                                             };
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.IsNotNull(withdraw2);
            Assert.AreEqual(withdraw.Currency.Name, withdraw2.Currency.Name);
            Assert.AreEqual(withdraw.AccountId.Value, withdraw2.AccountId.Value);
            Assert.AreEqual(withdraw.TransactionId.Value, withdraw2.TransactionId.Value);
            Assert.AreEqual(withdraw.Fee, withdraw2.Fee);
            Assert.AreEqual(withdraw.Status, withdraw2.Status);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void CommitWithdrawFailTest_ChecksThatWithdrawEventIsSubmittedAsExpectedToTheCoinClientService_VerifiesThroughVariablesValues()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            Withdraw withdraw = null;
            clientInteractionService.CommitWithdraw(withdraw);
        }

        [Test]
        public void CancelWithdrawTest_ChecksThatWithdrawEventIsSubmittedAsExpectedToTheCoinClientService_VerifiesThroughVariablesValues()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            Withdraw withdraw = new Withdraw(new Currency("BTC", true), "123", DateTime.Now, WithdrawType.Bitcoin, 0.4m,
                0.001m, TransactionStatus.Pending,
                new AccountId(123), new BitcoinAddress("bitcoin123"));

            bool eventFired = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Withdraw withdraw2 = null;
            clientInteractionService.WithdrawExecuted += delegate(Withdraw receivedWithdraw)
            {
                eventFired = true;
                withdraw2 = receivedWithdraw;
                manualResetEvent.Set();
            };
            bool commitWithdraw = clientInteractionService.CommitWithdraw(withdraw);
            Assert.IsTrue(commitWithdraw);
            clientInteractionService.CancelWithdraw(withdraw.WithdrawId);
            manualResetEvent.WaitOne(Convert.ToInt16(clientInteractionService.WithdrawSubmissionInterval));

            Assert.IsFalse(eventFired);
            Assert.IsNull(withdraw2);
        }

        [Test]
        public void DepositArrivalEventTest_ChecksThatDepositArrivedEventsAreRaisedHandledProperly_VerifiesThroughRaisedEvents()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            bool eventFired = false;
            int eventCounter = 0;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            clientInteractionService.DepositArrived +=
                                        delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
                                            {
                                                eventFired = true;
                                                eventCounter++;
                                            };

            // Raise the event twice from both services to see if duplicate invocations don't occur
            mockBitcoinClientService.RaiseDepositArrivedEvent();
            mockLitecoinClientService.RaiseDepositArrivedEvent();
            mockBitcoinClientService.RaiseDepositArrivedEvent();
            mockLitecoinClientService.RaiseDepositArrivedEvent();
            manualResetEvent.WaitOne(3000);
            Assert.IsTrue(eventFired);
            Assert.AreEqual(4, eventCounter);
        }

        [Test]
        public void DepositConfirmedEventTest_ChecksThatDepositConfirmedEventsAreRaisedHandledProperly_VerifiesThroughRaisedEvents()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            bool eventFired = false;
            int eventCounter = 0;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            clientInteractionService.DepositConfirmed +=
                                        delegate(string currency, int newTransactions)
                                        {
                                            eventFired = true;
                                            eventCounter++;
                                        };

            // Raise the event twice from both services to see if duplicate invocations don't occur
            mockBitcoinClientService.RaiseDepositConfirmedEvent();
            mockLitecoinClientService.RaiseDepositConfirmedEvent();
            mockBitcoinClientService.RaiseDepositConfirmedEvent();
            mockLitecoinClientService.RaiseDepositConfirmedEvent();
            manualResetEvent.WaitOne(3000);
            Assert.IsTrue(eventFired);
            Assert.AreEqual(4, eventCounter);
        }

        [Test]
        public void NewAddressForBtcTest_ChecksThatNewAddressIsCreatedAndReturnedProperly_VerifiesThroughRaisedEvents()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            string newAddress = clientInteractionService.GenerateNewAddress("BTC");
            Assert.IsTrue(!string.IsNullOrEmpty(newAddress));
        }

        [Test]
        public void NewAddressForLtcTest_ChecksThatNewAddressIsCreatedAndReturnedProperly_VerifiesThroughRaisedEvents()
        {
            var mockFundsRepository = new MockFundsRepository();
            var mockWithdrawRepository = new MockWithdrawRepository();
            var mockBitcoinClientService = new MockBitcoinClientService();
            var mockLitecoinClientService = new MockLitecoinClientService();
            ClientInteractionService clientInteractionService = new ClientInteractionService(mockFundsRepository, mockWithdrawRepository,
                mockBitcoinClientService, mockLitecoinClientService);

            string newAddress = clientInteractionService.GenerateNewAddress("LTC");
            Assert.IsTrue(!string.IsNullOrEmpty(newAddress));
        }
    }
}
