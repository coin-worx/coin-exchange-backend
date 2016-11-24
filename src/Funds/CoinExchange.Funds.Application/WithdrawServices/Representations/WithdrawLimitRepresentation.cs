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


﻿namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Represents the Withdrawawl limit thresholds and balance information
    /// </summary>
    public class WithdrawLimitRepresentation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"> </param>
        /// <param name="dailyLimit"></param>
        /// <param name="dailyLimitUsed"></param>
        /// <param name="monthlyLimit"></param>
        /// <param name="monthlyLimitUsed"></param>
        /// <param name="currentBalance"></param>
        /// <param name="maximumWithdraw"></param>
        /// <param name="maximumWithdrawInUsd"> </param>
        /// <param name="withheld"> </param>
        /// <param name="withheldInUsd"> </param>
        /// <param name="fee"> </param>
        /// <param name="minimumAmount"> </param>
        public WithdrawLimitRepresentation(string currency, int accountId, decimal dailyLimit, decimal dailyLimitUsed,
            decimal monthlyLimit, decimal monthlyLimitUsed, decimal currentBalance, decimal maximumWithdraw,
            decimal maximumWithdrawInUsd, decimal withheld, decimal withheldInUsd, decimal fee, decimal minimumAmount)
        {
            Currency = currency;
            AccountId = accountId;
            DailyLimit = dailyLimit;
            DailyLimitUsed = dailyLimitUsed;
            MonthlyLimit = monthlyLimit;
            MonthlyLimitUsed = monthlyLimitUsed;
            CurrentBalance = currentBalance;
            MaximumWithdraw = maximumWithdraw;
            MaximumWithdrawInUsd = maximumWithdrawInUsd;
            Withheld = withheld;
            WithheldInUsd = withheldInUsd;
            Fee = fee;
            MinimumAmount = minimumAmount;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// DailyLimit
        /// </summary>
        public decimal DailyLimit { get; private set; }

        /// <summary>
        /// DailyLimit that has been Used
        /// </summary>
        public decimal DailyLimitUsed { get; private set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public decimal MonthlyLimit { get; private set; }

        /// <summary>
        /// Monthly limit that has already been used
        /// </summary>
        public decimal MonthlyLimitUsed { get; private set; }

        /// <summary>
        /// Current balance for this currency of this user
        /// </summary>
        public decimal CurrentBalance { get; private set; }

        /// <summary>
        /// Maximum Withdraw threshold that the user can commit at the moment
        /// </summary>
        public decimal MaximumWithdraw { get; private set; }

        /// <summary>
        /// Maximum Withdraw amount in US Dollars
        /// </summary>
        public decimal MaximumWithdrawInUsd { get; private set; }

        /// <summary>
        /// Withheld Amount
        /// </summary>
        public decimal Withheld { get; private set; }

        /// <summary>
        /// Withheld amount in US Dollars
        /// </summary>
        public decimal WithheldInUsd { get; private set; }

        /// <summary>
        /// Fee for the withdrawal
        /// </summary>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Minimum Amount
        /// </summary>
        public decimal MinimumAmount { get; private set; }
    }
}
