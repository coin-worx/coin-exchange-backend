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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing depth levels
    /// </summary>
    public class DepthMemoryImage
    {
        private List<string> _currencyPairs = new List<string>(); 

        private DepthRepresentation _bidDepth = new DepthRepresentation();
        private DepthRepresentation _askDepth = new DepthRepresentation();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DepthMemoryImage()
        {
            _currencyPairs = new List<string>();
            InitializeCurrencyPairs();
            DepthEvent.DepthChanged += OnDepthArrived;
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
        /// Handles the event for the Depth
        /// </summary>
        public void OnDepthArrived(Depth depth)
        {
            UpdateBids(depth.CurrencyPair, depth.BidLevels);
            UpdateAsks(depth.CurrencyPair, depth.AskLevels);
        }

        /// <summary>
        /// Update the Bid depth levels
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="bidLevels"></param>
        private void UpdateBids(string currencyPair, DepthLevel[] bidLevels)
        {
            var depthLevelsRepresentations = CreateDepthLevels(bidLevels);
            if (depthLevelsRepresentations != null)
            {
                if (_bidDepth.ContainsKey(currencyPair))
                {
                    _bidDepth.SetValue(currencyPair, depthLevelsRepresentations);
                }
                else
                {
                    _bidDepth.AddDepth(currencyPair, depthLevelsRepresentations);
                }
            }
        }

        /// <summary>
        /// Update teh Ask depth levels
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="askLevels"></param>
        private void UpdateAsks(string currencyPair, DepthLevel[] askLevels)
        {
            var depthLevelsRepresentations = CreateDepthLevels(askLevels);
            if (depthLevelsRepresentations != null)
            {
                if (_askDepth.ContainsKey(currencyPair))
                {
                    _askDepth.SetValue(currencyPair, depthLevelsRepresentations);
                }
                else
                {
                    _askDepth.AddDepth(currencyPair, depthLevelsRepresentations);
                }
            }
        }

        /// <summary>
        /// Create Depth Levels
        /// </summary>
        /// <param name="depthLevels"></param>
        /// <returns></returns>
        private DepthLevelRepresentationList CreateDepthLevels(DepthLevel[] depthLevels)
        {
            DepthLevelRepresentationList depthLevelsRepresentations = new DepthLevelRepresentationList(depthLevels.Length);
            for (int i = 0; i < depthLevels.Length; i++)
            {
                depthLevelsRepresentations.AddDepthLevel(i, depthLevels[i]);
            }
            return depthLevelsRepresentations;
        }

        /// <summary>
        /// Contains Depth levels for bids for each currency in the order book, and each depth level contains
        /// Key = CurrencyPair,
        /// Value = Array of:
        /// Item1 = Aggregated Volume,
        /// Item2 = Price,
        /// Item3 = Number of orders present
        /// </summary>
        public DepthRepresentation BidDepths
        {
            get { return _bidDepth; }
        }

        /// <summary>
        /// Contains Depth levels for asks for each currency in the order book, and each depth level contains:
        /// Key = CurrencyPair,
        /// Value = Array of:
        /// Item1 = Aggregated Volume,
        /// Item2 = Price,
        /// Item3 = Number of orders present
        /// </summary>
        public DepthRepresentation AskDepths
        {
            get { return _askDepth; }
        }
    }
}
