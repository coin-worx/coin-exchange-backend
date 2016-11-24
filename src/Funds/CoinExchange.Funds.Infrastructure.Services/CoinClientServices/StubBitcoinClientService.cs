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
using System.Threading;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Stub Implementation for the Bitcoin Client Service
    /// </summary>
    public class StubBitcoinClientService : ICoinClientService
    {
        private List<Tuple<string, string, decimal, string>> _transactionList = new List<Tuple<string, string, decimal, string>>();
        public event Action<string, int> DepositConfirmed;

        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;

        public string CreateNewAddress()
        {
            return Guid.NewGuid().ToString();
        }

        public void CheckNewTransactions()
        {
            throw new NotImplementedException();
        }

        public void PollConfirmations()
        {
            throw new NotImplementedException();
        }

        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            string transactionId = "transactionId1";
            _transactionList = new List<Tuple<string, string, decimal, string>>();
            _transactionList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress,transactionId, amount, 
                BitcoinConstants.ReceiveCategory));
            if (DepositArrived != null)
            {
                DepositArrived(CurrencyConstants.Btc, _transactionList);
            }
            Thread.Sleep(200);
            if (DepositConfirmed != null)
            {
                DepositConfirmed(transactionId, 7);
            }
            return transactionId;
        }

        public void PopulateCurrencies()
        {
            throw new NotImplementedException();
        }

        public void PopulateServices()
        {
            throw new NotImplementedException();
        }

        public decimal CheckBalance(string currency)
        {
            return 0;
        }

        public double PollingInterval { get; private set; }
        public string Currency { get; private set; }
        public double NewTransactionsInterval { get; private set; }
    }
}
