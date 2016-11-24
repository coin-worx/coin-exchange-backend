/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
    class DogecoinClientUnitTests
    {
        [Test]
        public void NewTransactionTest_ChecksIfNewTransactionsAreProperlyHandledAndSavedToList_VerifiesThroughVariablesValues()
        {
            // New transaction is sent manually and checked that the event is raised and the values are the same
            DogecoinClientService dogecoinClientService = new DogecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid123";
            transaction.Address = "dogecoinaddress123";
            transaction.Amount = 10;
            transaction.BlockHash = "blockhash123";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

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
            DogecoinClientService dogecoinClientService = new DogecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid123";
            transaction.Address = "dogecoinaddress123";
            transaction.Amount = 10;
            transaction.BlockHash = "blockhash123";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

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
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
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
            List<Tuple<string, int>> depositList = new List<Tuple<string, int>>();
            MethodInfo methodInfo = dogecoinClientService.GetType().GetMethod("AddNewConfirmation", BindingFlags.NonPublic |
                BindingFlags.Instance);
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, depositList });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
        }

        [Test]
        public void MultipleTransactionsTest_CheckIfMultipleTransactionsAreAddedAsExpectedToTheList_VerifyThroughCariablesValues()
        {
            // 4 Transactions are sent one after the other, and every time DepositArrived event is raised, its values are checked.
            // When all 4 transactions have been sent, we check the private _pendingTransactions field that it contains exactly
            // the elements theat we sent and in the same order
            DogecoinClientService dogecoinClientService = new DogecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid1";
            transaction.Address = "bitcoinaddress1";
            transaction.Amount = 1;
            transaction.BlockHash = "blockhash1";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[0].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[0].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[0].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[0].Category, transactionListReceived.Single().Item4, "Category Check");

            // Second new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid2";
            transaction.Address = "bitcoinaddress2";
            transaction.Amount = 2;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[1].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[1].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[1].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[1].Category, transactionListReceived.Single().Item4, "Category Check");

            // Third new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid3";
            transaction.Address = "bitcoinaddress3";
            transaction.Amount = 3;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[2].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[2].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[2].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[2].Category, transactionListReceived.Single().Item4, "Category Check");

            // Fourth new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid4";
            transaction.Address = "bitcoinaddress4";
            transaction.Amount = 4;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[3].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[3].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[3].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[3].Category, transactionListReceived.Single().Item4, "Category Check");

            FieldInfo fieldInfo = dogecoinClientService.GetType().GetField("_pendingTransactions",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(fieldInfo);
            List<Tuple<string, int>> pendingTransactions = fieldInfo.GetValue(dogecoinClientService) as List<Tuple<string, int>>;
            Assert.IsNotNull(pendingTransactions);
            Assert.AreEqual(transactionsList[0].TxId, pendingTransactions[0].Item1, "Address Check");
            Assert.AreEqual(transactionsList[0].Confirmations, pendingTransactions[0].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[1].TxId, pendingTransactions[1].Item1, "Address Check");
            Assert.AreEqual(transactionsList[1].Confirmations, pendingTransactions[1].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[2].TxId, pendingTransactions[2].Item1, "Address Check");
            Assert.AreEqual(transactionsList[2].Confirmations, pendingTransactions[2].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[3].TxId, pendingTransactions[3].Item1, "Address Check");
            Assert.AreEqual(transactionsList[3].Confirmations, pendingTransactions[3].Item2, "TxId Check");
        }

        [Test]
        public void MultipleTransactionsAndConfirmationsTest_CheckIfMultipleTransactionsAndConfirmationsAreAddedAsExpectedToTheList_VerifyThroughCariablesValues()
        {
            // 4 Transactions are sent one after the other, and every time DepositArrived event is raised, its values are checked.
            // When all 4 transactions have been sent, we check the private _pendingTransactions field that it contains exactly
            // the elements that we sent and in the same order
            // All transactions are updated to 7 confirmations
            DogecoinClientService dogecoinClientService = new DogecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid1";
            transaction.Address = "bitcoinaddress1";
            transaction.Amount = 1;
            transaction.BlockHash = "blockhash1";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[0].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[0].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[0].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[0].Category, transactionListReceived.Single().Item4, "Category Check");

            // Second new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid2";
            transaction.Address = "bitcoinaddress2";
            transaction.Amount = 2;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[1].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[1].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[1].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[1].Category, transactionListReceived.Single().Item4, "Category Check");

            // Third new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid3";
            transaction.Address = "bitcoinaddress3";
            transaction.Amount = 3;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[2].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[2].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[2].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[2].Category, transactionListReceived.Single().Item4, "Category Check");

            // Fourth new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid4";
            transaction.Address = "bitcoinaddress4";
            transaction.Amount = 4;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[3].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[3].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[3].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[3].Category, transactionListReceived.Single().Item4, "Category Check");

            FieldInfo fieldInfo = dogecoinClientService.GetType().GetField("_pendingTransactions",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(fieldInfo);
            List<Tuple<string, int>> pendingTransactions = fieldInfo.GetValue(dogecoinClientService) as List<Tuple<string, int>>;
            Assert.IsNotNull(pendingTransactions);
            Assert.AreEqual(transactionsList[0].TxId, pendingTransactions[0].Item1, "Address Check");
            Assert.AreEqual(transactionsList[0].Confirmations, pendingTransactions[0].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[1].TxId, pendingTransactions[1].Item1, "Address Check");
            Assert.AreEqual(transactionsList[1].Confirmations, pendingTransactions[1].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[2].TxId, pendingTransactions[2].Item1, "Address Check");
            Assert.AreEqual(transactionsList[2].Confirmations, pendingTransactions[2].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[3].TxId, pendingTransactions[3].Item1, "Address Check");
            Assert.AreEqual(transactionsList[3].Confirmations, pendingTransactions[3].Item2, "TxId Check");

            //----------------Confirmations Start----------------------

            // Confirmation on index 0
            eventFired = false;
            manualResetEvent.Reset();
            string txId = null;
            int receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            GetTransactionResponse getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid1";
            getTransactionResponse.Confirmations = 7;
            int index = 0;
            List<Tuple<string, int>> confirmedDeposits = new List<Tuple<string, int>>();
            MethodInfo methodInfo = dogecoinClientService.GetType().GetMethod("AddNewConfirmation", BindingFlags.NonPublic |
                BindingFlags.Instance);
            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(1, confirmedDeposits.Count);

            // Confirmation on index 1
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid2";
            getTransactionResponse.Confirmations = 7;
            index = 1;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(2, confirmedDeposits.Count);

            // Confirmation on index 2
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid3";
            getTransactionResponse.Confirmations = 7;
            index = 2;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(3, confirmedDeposits.Count);

            // Confirmation on index 3
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid4";
            getTransactionResponse.Confirmations = 7;
            index = 3;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(4, confirmedDeposits.Count);

            Assert.AreEqual(transactionsList[0].TxId, pendingTransactions[0].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[0].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[1].TxId, pendingTransactions[1].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[1].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[2].TxId, pendingTransactions[2].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[2].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[3].TxId, pendingTransactions[3].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[3].Item2, "TxId Check");
        }

        [Test]
        public void MultipleTransactionsAndPartialConfirmationsTest_CheckIfMultipleTransactionsAndConfirmationsAreAddedAsExpectedToTheList_VerifyThroughCariablesValues()
        {
            // 4 Transactions are sent one after the other, and every time DepositArrived event is raised, its values are checked.
            // When all 4 transactions have been sent, we check the private _pendingTransactions field that it contains exactly
            // the elements that we sent and in the same order
            // Not all transactions are updated to 7 confirmations
            DogecoinClientService dogecoinClientService = new DogecoinClientService();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool eventFired = false;
            List<Tuple<string, string, decimal, string>> transactionListReceived = new List<Tuple<string, string, decimal, string>>();
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            List<TransactionSinceBlock> transactionsList = new List<TransactionSinceBlock>();
            TransactionSinceBlock transaction = new TransactionSinceBlock();
            transaction.TxId = "txid1";
            transaction.Address = "bitcoinaddress1";
            transaction.Amount = 1;
            transaction.BlockHash = "blockhash1";
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[0].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[0].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[0].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[0].Category, transactionListReceived.Single().Item4, "Category Check");

            // Second new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid2";
            transaction.Address = "bitcoinaddress2";
            transaction.Amount = 2;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[1].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[1].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[1].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[1].Category, transactionListReceived.Single().Item4, "Category Check");

            // Third new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid3";
            transaction.Address = "bitcoinaddress3";
            transaction.Amount = 3;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[2].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[2].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[2].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[2].Category, transactionListReceived.Single().Item4, "Category Check");

            // Fourth new transaction
            manualResetEvent.Reset();
            eventFired = false;
            dogecoinClientService.DepositArrived += delegate(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
            {
                eventFired = true;
                transactionListReceived = newTransactions;
                manualResetEvent.Set();
            };
            transaction = new TransactionSinceBlock();
            transaction.TxId = "txid4";
            transaction.Address = "bitcoinaddress4";
            transaction.Amount = 4;
            transaction.Category = "receive";
            transaction.Confirmations = 0;
            transactionsList.Add(transaction);
            dogecoinClientService.CheckNewTransactions(transactionsList);

            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.AreEqual(transactionsList[3].Address, transactionListReceived.Single().Item1, "Address Check");
            Assert.AreEqual(transactionsList[3].TxId, transactionListReceived.Single().Item2, "TxId Check");
            Assert.AreEqual(transactionsList[3].Amount, transactionListReceived.Single().Item3, "Amount Check");
            Assert.AreEqual(transactionsList[3].Category, transactionListReceived.Single().Item4, "Category Check");

            FieldInfo fieldInfo = dogecoinClientService.GetType().GetField("_pendingTransactions",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(fieldInfo);
            List<Tuple<string, int>> pendingTransactions = fieldInfo.GetValue(dogecoinClientService) as List<Tuple<string, int>>;
            Assert.IsNotNull(pendingTransactions);
            Assert.AreEqual(transactionsList[0].TxId, pendingTransactions[0].Item1, "TxId Check");
            Assert.AreEqual(transactionsList[0].Confirmations, pendingTransactions[0].Item2, "Confirmations Check");

            Assert.AreEqual(transactionsList[1].TxId, pendingTransactions[1].Item1, "TxId Check");
            Assert.AreEqual(transactionsList[1].Confirmations, pendingTransactions[1].Item2, "Confirmations Check");

            Assert.AreEqual(transactionsList[2].TxId, pendingTransactions[2].Item1, "TxId Check");
            Assert.AreEqual(transactionsList[2].Confirmations, pendingTransactions[2].Item2, "Confirmations Check");

            Assert.AreEqual(transactionsList[3].TxId, pendingTransactions[3].Item1, "TxId Check");
            Assert.AreEqual(transactionsList[3].Confirmations, pendingTransactions[3].Item2, "Confirmations Check");

            //----------------Confirmations Start----------------------

            // Confirmation on index 0
            eventFired = false;
            manualResetEvent.Reset();
            string txId = null;
            int receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            GetTransactionResponse getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid1";
            getTransactionResponse.Confirmations = 7;
            int index = 0;
            List<Tuple<string, int>> confirmedDeposits = new List<Tuple<string, int>>();
            MethodInfo methodInfo = dogecoinClientService.GetType().GetMethod("AddNewConfirmation", BindingFlags.NonPublic |
                BindingFlags.Instance);
            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(1, confirmedDeposits.Count);

            // Confirmation on index 1
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid2";
            getTransactionResponse.Confirmations = 2;
            index = 1;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(1, confirmedDeposits.Count);

            // Confirmation on index 2
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid3";
            getTransactionResponse.Confirmations = 3;
            index = 2;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(1, confirmedDeposits.Count);

            // Confirmation on index 3
            eventFired = false;
            manualResetEvent.Reset();
            txId = null;
            receivedConfirmations = 0;
            // Handler for event which is raised when enough confirmations are available
            dogecoinClientService.DepositConfirmed += delegate(string transactionId, int confirmations)
            {
                eventFired = true;
                txId = transactionId;
                receivedConfirmations = confirmations;
                manualResetEvent.Set();
            };

            // Create a new TransactionResponse and send to AddNEwConfrimation method
            getTransactionResponse = new GetTransactionResponse();
            getTransactionResponse.TxId = "txid4";
            getTransactionResponse.Confirmations = 7;
            index = 3;

            // Invoke private method AddNewConfirmations and send TransactionResponse, the index of _pendingTransactions list 
            // to which to add the confirmation to and the depositList instance that shows confirmedDeposits
            methodInfo.Invoke(dogecoinClientService, new object[] { getTransactionResponse, index, confirmedDeposits });
            manualResetEvent.WaitOne();

            Assert.IsTrue(eventFired);
            Assert.AreEqual(getTransactionResponse.TxId, txId);
            Assert.AreEqual(getTransactionResponse.Confirmations, receivedConfirmations);
            Assert.AreEqual(2, confirmedDeposits.Count);

            Assert.AreEqual(transactionsList[0].TxId, pendingTransactions[0].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[0].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[1].TxId, pendingTransactions[1].Item1, "Address Check");
            Assert.AreEqual(2, pendingTransactions[1].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[2].TxId, pendingTransactions[2].Item1, "Address Check");
            Assert.AreEqual(3, pendingTransactions[2].Item2, "TxId Check");

            Assert.AreEqual(transactionsList[3].TxId, pendingTransactions[3].Item1, "Address Check");
            Assert.AreEqual(7, pendingTransactions[3].Item2, "TxId Check");
        }
    }
}
