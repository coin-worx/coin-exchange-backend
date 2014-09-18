using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitcoinLib.Responses;
using CoinExchange.Funds.Infrastructure.Services.CoinClientServices;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastructure.Services.Tests
{
    [TestFixture]
    class LitecoinClientServiceTests
    {
        [Test]
        public void NewTransactionTest_ChecksIfNewTransactionsAreProperlyHandledAndSavedToList_VerifiesThroughVariablesValues()
        {
            // New transaction is sent manually and checked that the event is raised and the values are the same
            LitecoinClientService litecoinClientService = new LitecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            litecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
                                                        {
                                                            eventFired = true;
                                                            transactionListReceived = newTransactions;
                                                            manualResetEvent.Set();
                                                        };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid123";
            transaction.Address = "bitcoinaddress123";
            transaction.Amount = 10;
            transaction.BlockHash = "blockhash123";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            litecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList.Single().Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList.Single().TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList.Single().Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList.Single().Category, transactionListReceived.Single().Item4, "Category Check");
        }

        [Test]
        public void PollCnfirmationTest_ChecksIfNewConfirmationsAreProperlyAddedAndEventsAreRaised_VerifiesThroughVariablesValues()
        {
            // New transaction is sent manually and checked that the event is raised and the values are the same
            // Then confirmations are checked and it is made sure that events are being raised properly
            LitecoinClientService litecoinClientService = new LitecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            litecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
                                                        {
                                                            eventFired = true;
                                                            transactionListReceived = newTransactions;
                                                            manualResetEvent.Set();
                                                        };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid123";
            transaction.Address = "bitcoinaddress123";
            transaction.Amount = 10;
            transaction.BlockHash = "blockhash123";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            litecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList.Single().Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList.Single().TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList.Single().Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList.Single().Category, transactionListReceived.Single().Item4, "Category Check");

            string txId = null;
            int receivedConfirmations = 0;
            
            eventFired = false;
            manualResetEvent.Reset();
            // Handler for event which is raised when enough confirmations are available
            litecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            GetTransactionResponse getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid123";
            getTransactionResponse.Confirmations = 7;
            int index = 0;
            List<Tuple<string,int>> depositList = new List<Tuple<string, int>>();
            MethodInfo methodInfo = litecoinClientService.GetType().GetMethod("AddNewConfirmation", BindingFlags.NonPublic | 
                BindingFlags.Instance);
            methodInfo.Invoke(litecoinClientService, new object[]{getTransactionResponse,index,depositList});
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
        }
    }
}
