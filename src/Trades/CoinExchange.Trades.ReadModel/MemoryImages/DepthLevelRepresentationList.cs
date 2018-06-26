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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the Depth levels in a simplified format
    /// </summary>
    public class DepthLevelRepresentationList : IEnumerable<DepthTuple>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// 3. OrderCount
        /// </summary>
        private DepthTuple[] _depthLevelList = null;
        private int _size = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="size"></param>
        public DepthLevelRepresentationList(int size)
        {
            _depthLevelList = new DepthTuple[size];
        }

        /// <summary>
        /// Adds the depth level to the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="depthLevel"></param>
        /// <returns></returns>
        public bool AddDepthLevel(int index, DepthLevel depthLevel)
        {
            if (depthLevel.AggregatedVolume != null && depthLevel.Price != null)
            {
                _depthLevelList[index] = new DepthTuple(depthLevel.AggregatedVolume.Value,
                                              depthLevel.Price.Value, depthLevel.OrderCount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Flattens i.e., makes the specified slot null
        /// </summary>
        /// <returns></returns>
        public bool FlattenDepthLevel(int index)
        {
            _depthLevelList[index] = null;
            return true;
        }

        /// <summary>
        /// Size
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// DepthLevels representation
        /// </summary>
        public DepthTuple[] DepthLevels
        {
            get { return _depthLevelList; }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<DepthTuple> IEnumerable<DepthTuple>.GetEnumerator()
        {
            foreach (var orderStats in _depthLevelList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (DepthTuple orderStats in _depthLevelList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        #endregion
    }
}
