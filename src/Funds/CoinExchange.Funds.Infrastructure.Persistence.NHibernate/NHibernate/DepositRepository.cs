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
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for accessing Deposit objects
    /// </summary>
    [Repository]
    public class DepositRepository : NHibernateSessionFactory, IDepositRepository
    {
        [Transaction]
        public Deposit GetDepositById(int id)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<Deposit> GetDepositByCurrencyName(string currency)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.Currency.Name == currency)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public List<Deposit> GetDepositByCurrencyAndAccountId(string currency, AccountId accountId)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.Currency.Name == currency && x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public Deposit GetDepositByDepositId(string depositId)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.DepositId == depositId).SingleOrDefault();
        }

        [Transaction]
        public Deposit GetDepositByTransactionId(TransactionId transactionId)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.TransactionId.Value == transactionId.Value).SingleOrDefault();
        }

        [Transaction]
        public List<Deposit> GetDepositsByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.BitcoinAddress.Value == bitcoinAddress.Value)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public List<Deposit> GetAllDeposits()
        {
            return CurrentSession.Query<Deposit>()
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public List<Deposit> GetDepositByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }
    }
}
