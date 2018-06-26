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
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.BalanceAggregate
{
    /// <summary>
    /// Represents an entity that has pending balance
    /// </summary>
    public class PendingTransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PendingTransaction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PendingTransaction(Currency currency, string instanceId, PendingTransactionType pendingEntityType, 
            decimal amount, int balanceId)
        {
            Currency = currency;
            InstanceId = instanceId;
            PendingTransactionType = pendingEntityType;
            Amount = amount;
            BalanceId = balanceId;
        }

        /// <summary>
        /// Database Primary Key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Primary key ID of the balance database record
        /// </summary>
        public int BalanceId { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public virtual Currency Currency { get; private set; }

        /// <summary>
        /// ID of Order or Withdrawal instance
        /// </summary>
        public virtual string InstanceId { get; private set; }

        /// <summary>
        /// Type of the Pending Transaction i.e., Order or Withdraw
        /// </summary>
        public virtual PendingTransactionType PendingTransactionType { get; private set; }

        /// <summary>
        /// Amount of the pending transaction
        /// </summary>
        public virtual decimal Amount { get; private set; }
    }
}
