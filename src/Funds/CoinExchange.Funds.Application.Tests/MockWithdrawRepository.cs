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
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawRepository : IWithdrawRepository
    {
        private List<Withdraw> _withdrawawls = new List<Withdraw>();

        public Withdraw GetWithdrawById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByAccountId(AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByCurrencyName(string currency)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByCurrencyAndAccountId(string currency, AccountId accountId)
        {
            List<Withdraw> currentWithdrawals = new List<Withdraw>();
            foreach (var ledger in _withdrawawls)
            {
                if (ledger.Currency.Name == currency && ledger.AccountId.Value == accountId.Value)
                {
                    currentWithdrawals.Add(ledger);
                }
            }
            return currentWithdrawals;
        }

        public Withdraw GetWithdrawByWithdrawId(string withdrawId)
        {
            throw new NotImplementedException();
        }

        public Withdraw GetWithdrawByTransactionId(TransactionId transactionId)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        public void AddLedger(Withdraw withdraw)
        {
            _withdrawawls.Add(withdraw);
        }
    }
}
