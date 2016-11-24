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
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Service to evaluate the Maximum Withdraw 
    /// </summary>
    public interface IWithdrawLimitEvaluationService
    {
        /// <summary>
        /// Evaluates the Maximum Withdraw amount
        /// </summary>
        /// <returns></returns>
        bool EvaluateMaximumWithdrawLimit(decimal withdrawalAmount, IList<Withdraw> depositLedgers, WithdrawLimit 
                                 withdrawLimit, decimal balance, decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0);

        /// <summary>
        /// Assigns the threshold limits without comparing the current withdrawal amount
        /// </summary>
        /// <param name="depositLedgers"></param>
        /// <param name="withdrawLimit"></param>
        /// <param name="balance"></param>
        /// <param name="currentBalance"></param>
        /// <param name="bestBid"> </param>
        /// <param name="bestAsk"> </param>
        /// <returns></returns>
        bool AssignWithdrawLimits(IList<Withdraw> depositLedgers, WithdrawLimit withdrawLimit, decimal balance, 
                                    decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0);

        /// <summary>
        /// Daily Limit that the user can use in 24 hours
        /// </summary>
        decimal DailyLimit { get; }

        /// <summary>
        /// Daily limit that has been used by the user in the next 24 hours
        /// </summary>
        decimal DailyLimitUsed { get; }

        /// <summary>
        /// Monthly Limit that the user can use in 30 days
        /// </summary>
        decimal MonthlyLimit { get; }

        /// <summary>
        /// Amount that has been used in the last 30 days
        /// </summary>
        decimal MonthlyLimitUsed { get; }

        /// <summary>
        /// Withheld
        /// </summary>
        decimal WithheldAmount { get; }

        /// <summary>
        /// The maximum amount that can be withdrawn
        /// </summary>
        decimal MaximumWithdraw { get; }
    }
}
