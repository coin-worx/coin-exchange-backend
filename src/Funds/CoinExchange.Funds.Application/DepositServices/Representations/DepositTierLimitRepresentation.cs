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

namespace CoinExchange.Funds.Application.DepositServices.Representations
{
    /// <summary>
    /// Representation for the daily and monthly deposit limits
    /// </summary>
    public class DepositTierLimitRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositTierLimitRepresentation(decimal tier0DailyLimit, decimal tier0MonthlyLimit, decimal tier1DailyLimit, decimal tier1MonthlyLimit, decimal tier2DailyLimit, decimal tier2MonthlyLimit, decimal tier3DailyLimit, decimal tier3MonthlyLimit, decimal tier4DailyLimit, decimal tier4MonthlyLimit)
        {
            Tier0DailyLimit = tier0DailyLimit;
            Tier0MonthlyLimit = tier0MonthlyLimit;
            Tier1DailyLimit = tier1DailyLimit;
            Tier1MonthlyLimit = tier1MonthlyLimit;
            Tier2DailyLimit = tier2DailyLimit;
            Tier2MonthlyLimit = tier2MonthlyLimit;
            Tier3DailyLimit = tier3DailyLimit;
            Tier3MonthlyLimit = tier3MonthlyLimit;
            Tier4DailyLimit = tier4DailyLimit;
            Tier4MonthlyLimit = tier4MonthlyLimit;
        }

        /// <summary>
        /// Tier 0 Daily Limit
        /// </summary>
        public decimal Tier0DailyLimit { get; private set; }

        /// <summary>
        /// Tier 0 Monthly Limit
        /// </summary>
        public decimal Tier0MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 1 Daily Limit
        /// </summary>
        public decimal Tier1DailyLimit { get; private set; }

        /// <summary>
        /// Tier 1 Monthly Limit
        /// </summary>
        public decimal Tier1MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 2 Daily Limit
        /// </summary>
        public decimal Tier2DailyLimit { get; private set; }

        /// <summary>
        /// Tier 2 Monthly Limit
        /// </summary>
        public decimal Tier2MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 3 Daily Limit
        /// </summary>
        public decimal Tier3DailyLimit { get; private set; }

        /// <summary>
        /// Tier 3 Monthly Limit
        /// </summary>
        public decimal Tier3MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 4 Daily Limit
        /// </summary>
        public decimal Tier4DailyLimit { get; private set; }

        /// <summary>
        /// Tier 4 Monthly Limit
        /// </summary>
        public decimal Tier4MonthlyLimit { get; private set; }
    }
}
