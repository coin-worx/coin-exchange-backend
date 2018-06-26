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
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Represents the Withdrawal object - Entity
    /// </summary>
    public class Withdraw
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Withdraw()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Withdraw(Currency currency, string withdrawId, DateTime date, WithdrawType type, decimal amount, decimal fee, 
            TransactionStatus status, AccountId accountId, BitcoinAddress bitcoinAddress)
        {
            Currency = currency;
            WithdrawId = withdrawId;
            DateTime = date;
            Type = type;
            Amount = amount;
            Fee = fee;
            Status = status;
            AccountId = accountId;
            BitcoinAddress = bitcoinAddress;
        }

        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; private set; }

        /// <summary>
        /// DepositId
        /// </summary>
        public string WithdrawId { get; private set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Type
        /// </summary>
        public WithdrawType Type { get; private set; }

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
        /// Bank Account
        /// </summary>
        public BankAccount BankAccount { get; private set; }

        /// <summary>
        /// Status of the Deposit
        /// </summary>
        public TransactionStatus Status { get; private set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; private set; }

        /// <summary>
        /// Transaction ID for the Withdrawaal
        /// </summary>
        public TransactionId TransactionId { get; private set; }

        /// <summary>
        /// Bitcoin Address associated with this withdrawal
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; private set; }

        /// <summary>
        /// Assigns the withdraw fee to this instance
        /// </summary>
        /// <param name="fee"></param>
        /// <returns></returns>
        public bool AssignFee(decimal fee)
        {
            Fee = fee;
            return true;
        }

        /// <summary>
        /// Sets the status to Confirmed
        /// </summary>
        public void StatusConfirmed()
        {
            if (Status == TransactionStatus.Pending)
            {
                Status = TransactionStatus.Confirmed;
            }
        }

        /// <summary>
        /// Sets the status to Cancelled
        /// </summary>
        public void StatusCancelled()
        {
            if (Status == TransactionStatus.Pending)
            {
                Status = TransactionStatus.Cancelled;
            }
        }

        /// <summary>
        /// Sets the amount of the withdrawal
        /// </summary>
        public void SetAmount(decimal amount)
        {
            Amount = amount;
        }

        /// <summary>
        /// Sets the amount of withdrawal in US Dollars
        /// </summary>
        public void SetAmountInUsd(decimal amountInUsd)
        {
            AmountInUsd = amountInUsd;
        }

        /// <summary>
        /// Assigns the Transaction ID to this withdraw instance
        /// </summary>
        public void SetTransactionId(string transactionId)
        {
            this.TransactionId = new TransactionId(transactionId);
        }
    }
}
