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

namespace CoinExchange.Funds.Application.LedgerServices.Representations
{
    /// <summary>
    /// Represents the Ledger
    /// </summary>
    public class LedgerRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LedgerRepresentation(string ledgerId, DateTime dateTime, string ledgerType, string currency, 
            decimal amount, decimal amountInUsd, decimal fee, decimal balance, string tradeId, string orderId,
            string withdrawId, string depositId, int accountId)
        {
            LedgerId = ledgerId;
            DateTime = dateTime;
            LedgerType = ledgerType;
            Currency = currency;
            Amount = amount;
            AmountInUsd = amountInUsd;
            Fee = fee;
            Balance = balance;
            TradeId = tradeId;
            OrderId = orderId;
            WithdrawId = withdrawId;
            DepositId = depositId;
            AccountId = accountId;
        }

        /// <summary>
        /// Ledger ID
        /// </summary>
        public string LedgerId { get; private set; }

        /// <summary>
        /// Datetime
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Type of Ledger
        /// </summary>
        public string LedgerType { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Amount in US Dollars
        /// </summary>
        public decimal AmountInUsd { get; private set; }

        /// <summary>
        /// Fee
        /// </summary>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// TradeId
        /// </summary>
        public string TradeId { get; private set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string OrderId { get; private set; }

        /// <summary>
        /// Withdraw ID
        /// </summary>
        public string WithdrawId { get; private set; }

        /// <summary>
        /// Deposit ID
        /// </summary>
        public string DepositId { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

    }
}
