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
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Service to evaluate the Maximum Deposit
    /// </summary>
    public class DepositLimitEvaluationService : IDepositLimitEvaluationService
    {
        private decimal _dailyLimit = 0;
        private decimal _dailyLimitUsed = 0;
        private decimal _monthlyLimit = 0;
        private decimal _monthlyLimitUsed = 0;
        private decimal _maximumDeposit = 0;

        /// <summary>
        /// Evaluates if the current deposit transaction is within the maximum deposit limit and is allowed to proceed
        /// </summary>
        /// <param name="currentDepositAmount"> </param>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBid"> </param>
        /// <param name="bestAsk"> </param>
        /// <returns></returns>
        public bool EvaluateDepositLimit(decimal currentDepositAmount, IList<Ledger> depositLedgers, DepositLimit depositLimit, 
            decimal bestBid = 0, decimal bestAsk = 0)
        {
            if (depositLimit.DailyLimit != 0 && depositLimit.MonthlyLimit != 0)
            {
                // Set Daily and Monthly Limit
                SetLimits(depositLimit);
                // Set the amount used in the Daily and Monthly limit
                SetUsedLimits(depositLedgers, bestBid, bestAsk);
                // Evaluate the Maximum Deposit, set it, and return response whether it went successfully or not
                if (EvaluateMaximumDeposit())
                {
                    return currentDepositAmount <= _maximumDeposit;
                }
            }
            _maximumDeposit = 0;
            _dailyLimit = 0;
            _dailyLimitUsed = 0;
            _monthlyLimit = 0;
            _monthlyLimitUsed = 0;
            return false;
        }

        /// <summary>
        /// Assigns the Threshold Limits without comparing them with a given value
        /// </summary>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBid"> </param>
        /// <param name="bestAsk"> </param>
        /// <returns></returns>
        public bool AssignDepositLimits(IList<Ledger> depositLedgers, DepositLimit depositLimit, decimal bestBid, decimal bestAsk)
        {
            if (depositLimit.DailyLimit != 0 && depositLimit.MonthlyLimit != 0)
            {
                // Set Daily and Monthly Limit
                SetLimits(depositLimit);
                // Set the amount used in the Daily and Monthly limit
                SetUsedLimits(depositLedgers, bestBid, bestAsk);
                // Evaluate the Maximum Deposit, set it, and return response whether it went successfully or not
                if (EvaluateMaximumDeposit())
                {
                    return true;
                }
            }
            _maximumDeposit = 0;
            _dailyLimit = 0;
            _dailyLimitUsed = 0;
            _monthlyLimit = 0;
            _monthlyLimitUsed = 0;

            return false;
        }

        /// <summary>
        /// Evaluates the value of Maximum Deposit
        /// </summary>
        /// <returns></returns>
        private bool EvaluateMaximumDeposit()
        {
            // If either the daily limit or monthly limit has been reached, no deposit can be made
            if (!(_monthlyLimit - _monthlyLimitUsed).Equals(0) || !(_dailyLimit - _dailyLimitUsed).Equals(0))
            {
                // If both the used limits have not been used
                if (_monthlyLimitUsed.Equals(0) && _dailyLimitUsed.Equals(0))
                {
                    return SetMaximumDeposit(_dailyLimit);
                }
                // If only the DailyLimitUsed is 0, we need to evaluate other conditions
                else if (_dailyLimitUsed.Equals(0))
                {
                    // E.g., if DailyLimit = 0/1000 & MonthlyLimit = 3000/5000
                    if (_monthlyLimit - _monthlyLimitUsed > _dailyLimit)
                    {
                        return SetMaximumDeposit(_dailyLimit);
                    }
                    // E.g., if DailyLimit = 0/1000, & MonthlyLimit = 4500/5000
                    else if (_monthlyLimit - _monthlyLimitUsed < _dailyLimit)
                    {
                        return SetMaximumDeposit(_monthlyLimit - _monthlyLimitUsed);
                    }
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 4000/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) > (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(_dailyLimit - _dailyLimitUsed);
                }
                // E.g., if DailyLimit = 400/1000 & MonthlyLimit = 4500/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) < (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(_monthlyLimit - _monthlyLimitUsed);
                }
                else if ((_monthlyLimit - _monthlyLimitUsed).Equals(_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(_monthlyLimit - _monthlyLimitUsed);
                }
            }
            return false;
        }

        /// <summary>
        /// Set the value for the Maximum Deposit in currency amount
        /// </summary>
        private bool SetMaximumDeposit(decimal maximumDeposit)
        {
            _maximumDeposit = maximumDeposit;
            return true;
        }

        /// <summary>
        /// Sets the limits of daily and monthly deposit limits
        /// </summary>
        private void SetLimits(DepositLimit depositLimit)
        {
            _dailyLimit = depositLimit.DailyLimit;
            _monthlyLimit = depositLimit.MonthlyLimit;
        }

        /// <summary>
        /// Sets the amount that has been used for daily and monthly deposit limits
        /// </summary>
        /// <returns></returns>
        private void SetUsedLimits(IList<Ledger> depositLedgers, decimal bestBid, decimal bestAsk)
        {
            decimal tempDailyLimitUsed = 0;
            decimal tempMonthlyLimitUsed = 0;

            if (depositLedgers != null)
            {
                foreach (var depositLedger in depositLedgers)
                {
                    decimal amount = 0;

                    // Best bid and best ask will be zero if the admin has decided to specify the limits in  Default Crypto currency
                    // and not FIAT currency
                    if (bestBid == 0 && bestAsk == 0)
                    {
                        amount = depositLedger.Amount;
                    }
                    else
                    {
                        amount = depositLedger.AmountInUsd;
                    }
                    // Set the amount used in the last 24 hours
                    if (depositLedger.DateTime >= DateTime.Now.AddHours(-24))
                    {
                        tempDailyLimitUsed += amount;
                        tempMonthlyLimitUsed += amount;
                    }
                    // Set the amount used in the last 30 days
                    if (depositLedger.DateTime >= DateTime.Now.AddDays(-30) &&
                        depositLedger.DateTime < DateTime.Now.AddHours(-24))
                    {
                        tempMonthlyLimitUsed += amount;
                    }
                }
            }

            _dailyLimitUsed = tempDailyLimitUsed;
            _monthlyLimitUsed = tempMonthlyLimitUsed;
        }

        #region Properties

        /// <summary>
        /// Daily limit
        /// </summary>
        public decimal DailyLimit
        {
            get { return _dailyLimit; }
            private set { value = _dailyLimit; }
        }

        /// <summary>
        /// Daily limit that has been used in the last 24 hours
        /// </summary>
        public decimal DailyLimitUsed
        {
            get { return _dailyLimitUsed; }
            private set { value = _dailyLimitUsed; }
        }

        /// <summary>
        /// Monthly limit
        /// </summary>
        public decimal MonthlyLimit
        {
            get { return _monthlyLimit; }
            private set { value = _monthlyLimit; }
        }

        /// <summary>
        /// Monthly Limit that has been used in the last 30 days
        /// </summary>
        public decimal MonthlyLimitUsed
        {
            get { return _monthlyLimitUsed; }
            private set { value = _monthlyLimitUsed; }
        }

        /// <summary>
        /// Maximum Deposit allowed to the user at this moment
        /// </summary>
        public decimal MaximumDeposit
        {
            get { return _maximumDeposit; }
            private set { value = _maximumDeposit; }
        }

        #endregion Properties
    }
}
