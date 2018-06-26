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

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// BBORepresentation
    /// </summary>
    public class BBORepresentation
    {
        private string _currencyPair = string.Empty;
        private decimal _bestBidPrice = 0;
        private decimal _bestBidVolume = 0;
        private int _bestBidOrderCount = 0;
        private decimal _bestAskPrice = 0;
        private decimal _bestAskVolume = 0;
        private int _bestAskOrderCount = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBORepresentation(string currencyPair, decimal bestBidPrice, decimal bestBidVolume, int bestBidOrderCount,
            decimal bestAskPrice, decimal bestAskVolume, int bestAskOrderCount)
        {
            _currencyPair = currencyPair;
            _bestBidPrice = bestBidPrice;
            _bestBidVolume = bestBidVolume;
            _bestBidOrderCount = bestBidOrderCount;
            _bestAskPrice = bestAskPrice;
            _bestAskVolume = bestAskVolume;
            _bestAskOrderCount = bestAskOrderCount;
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Best Bid Price
        /// </summary>
        public decimal BestBidPrice
        {
            get { return _bestBidPrice; }
        }

        /// <summary>
        /// Best Bid Volume
        /// </summary>
        public decimal BestBidVolume
        {
            get { return _bestBidVolume; }
        }

        /// <summary>
        /// Best Bid's Ordercount
        /// </summary>
        public decimal BestBidOrderCount
        {
            get { return _bestBidOrderCount; }
        }

        /// <summary>
        /// best Ask's Price
        /// </summary>
        public decimal BestAskPrice
        {
            get { return _bestAskPrice; }
        }

        /// <summary>
        /// Best Ask's Volume
        /// </summary>
        public decimal BestAskVolume
        {
            get { return _bestAskVolume; }
        }

        /// <summary>
        /// Best Ask's OrderCount
        /// </summary>
        public decimal BestAskOrderCount
        {
            get { return _bestAskOrderCount; }
        }
    }
}
