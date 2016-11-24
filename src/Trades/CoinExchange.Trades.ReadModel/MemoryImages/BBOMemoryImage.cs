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
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing best Bid and best Ask
    /// </summary>
    public class BBOMemoryImage
    {
        private List<string> _currencyPairs = new List<string>(); 

        private BBORepresentationList _bboRepresentationList = null;
        private RatesList _ratesList = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBOMemoryImage()
        {
            _currencyPairs = new List<string>();
            InitializeCurrencyPairs();
            _bboRepresentationList = new BBORepresentationList();
            _ratesList = new RatesList();
            BBOEvent.BBOChanged += OnBBOArrived;
        }

        /// <summary>
        /// Event listener for listening BBO
        /// </summary>
        /// <param name="bbo"></param>
        private void OnBBOArrived(BBO bbo)
        {
            OnBBOArrived(bbo.CurrencyPair, bbo.BestBid, bbo.BestAsk);
        }

        /// <summary>
        /// Initialize the set of currency pairs that the application is to support.
        /// </summary>
        private void InitializeCurrencyPairs()
        {
            _currencyPairs.Add(CurrencyConstants.BtcLtc);
            _currencyPairs.Add(CurrencyConstants.BtcLtcSeparated);
            _currencyPairs.Add(CurrencyConstants.XbtLtc);
            _currencyPairs.Add(CurrencyConstants.XbtLtcSeparated);
        }

        /// <summary>
        /// Handles the Best bid and best offer
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="bestBid"></param>
        /// <param name="bestAsk"></param>
        public void OnBBOArrived(string currencyPair, DepthLevel bestBid, DepthLevel bestAsk)
        {
            _bboRepresentationList.AddBBO(currencyPair, bestBid, bestAsk);
            if (bestBid != null && bestAsk != null && bestBid.Price != null && bestAsk.Price != null)
            {
                _ratesList.AddRate(currencyPair, bestBid.Price.Value, bestAsk.Price.Value);
            }
        }

        /// <summary>
        /// Contains a BBO representation against each currency, each representation contains Volume, Price and order count 
        /// information for the best dask and bid depth of that currency
        /// </summary>
        public BBORepresentationList BBORepresentationList
        {
            get { return _bboRepresentationList; }
        }

        /// <summary>
        /// Contains the Rates for each and every currency pair. Rate is the midpoint between the best bid and the best ask
        /// </summary>
        public RatesList RatesList
        {
            get { return _ratesList; }
        }
    }
}
