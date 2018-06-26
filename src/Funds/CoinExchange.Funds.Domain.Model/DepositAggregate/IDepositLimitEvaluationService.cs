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


﻿using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for determining the Daily and Monthly limits for Deposit
    /// </summary>
    public interface IDepositLimitEvaluationService
    {
        /*/// <summary>
        /// Evaluate the limit for deposit and signify if current transaction is within the deposit limits by comapring 
        /// it with the threshold limits
        /// </summary>
        /// <returns></returns>
        bool EvaluateDepositLimit(decimal amountInUsd, IList<Ledger> depositLedgers, DepositLimit depositLimit);

        /// <summary>
        /// Assigns the deposit limits without comparing them to a given deposit value
        /// </summary>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <returns></returns>
        bool AssignDepositLimits(IList<Ledger> depositLedgers, DepositLimit depositLimit);*/

        /// <summary>
        /// Evaluate the limit for deposit in the FIAT currency specified by the user, to see if the deposit is within the limits
        /// </summary>
        /// <param name="amountInUsd"></param>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBid"></param>
        /// <param name="bestAsk"></param>
        /// <returns></returns>
        bool EvaluateDepositLimit(decimal amountInUsd, IList<Ledger> depositLedgers, DepositLimit depositLimit, 
            decimal bestBid = 0, decimal bestAsk = 0);

        /// <summary>
        /// Assigns the deposit limits without comparing them to a given deposit value
        /// </summary>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBid"> </param>
        /// <param name="bestAsk"> </param>
        /// <returns></returns>
        bool AssignDepositLimits(IList<Ledger> depositLedgers, DepositLimit depositLimit, decimal bestBid = 0, decimal bestAsk = 0);

        /// <summary>
        /// Daily Deposit Limit
        /// </summary>
        decimal DailyLimit { get; }

        /// <summary>
        /// Daily Deposit Limit that has been used in the last 24 hours
        /// </summary>
        decimal DailyLimitUsed { get; }

        /// <summary>
        /// Monthly Deposit Limit
        /// </summary>
        decimal MonthlyLimit { get; }

        /// <summary>
        /// Monthly Deposit Limit that has been used in the last 30 days
        /// </summary>
        decimal MonthlyLimitUsed { get; }

        /// <summary>
        /// Maximum Deposit amount allowed to the user at this moment.. Deposit should be kept 5-10% lower than this limit
        /// </summary>
        decimal MaximumDeposit { get; }
    }
}
