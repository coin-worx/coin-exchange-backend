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
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockLedgerRepository : ILedgerRepository
    {
        private IList<Ledger> _ledgers = new List<Ledger>(); 
        public Ledger GetLedgerById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgerByAccountId(AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgerByCurrencyName(string currency)
        {
            throw new NotImplementedException();
        }

        public IList<Ledger> GetLedgerByAccountIdAndCurrency(string currency, AccountId accountId)
        {
            IList<Ledger> currentLedgers = new List<Ledger>();
            foreach (var ledger in _ledgers)
            {
                if (ledger.Currency.Name == currency && ledger.AccountId.Value == accountId.Value)
                {
                    currentLedgers.Add(ledger);
                }
            }
            return currentLedgers;
        }

        public Ledger GetLedgerByLedgerId(string ledgerId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByTradeId(string tradeId)
        {
            throw new NotImplementedException();
        }

        public Ledger GetLedgersByDepositId(string depositId)
        {
            throw new NotImplementedException();
        }

        public Ledger GetLedgersByWithdrawId(string withdrawId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByOrderId(string orderId)
        {
            throw new NotImplementedException();
        }

        public decimal GetBalanceForCurrency(string currency, AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public IList<Ledger> GetAllLedgers(int accountId)
        {
            throw new NotImplementedException();
        }

        public IList<Ledger> GetAllLedgers()
        {
            throw new NotImplementedException();
        }

        public void AddLedger(Ledger ledger)
        {
            _ledgers.Add(ledger);
        }
    }
}
