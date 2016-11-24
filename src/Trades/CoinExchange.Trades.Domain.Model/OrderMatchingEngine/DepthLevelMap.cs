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

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    [Serializable]
    public class DescendingOrderComparer : IComparer<decimal>
    {
        #region Implementation of IComparer<in decimal>

        public int Compare(decimal x, decimal y)
        {
            // Use the default comparer to do the original comparison for datetimes
            int ascendingResult = Comparer<decimal>.Default.Compare(x, y);

            // Turn the result around
            return 0 - ascendingResult;
        }

        #endregion
    }

    /// <summary>
    /// Contains the depth levels for each price in the book
    /// Key = Price
    /// Value = DepthLevel
    /// </summary>
    [Serializable]
    public class DepthLevelMap : IEnumerable<KeyValuePair<decimal, DepthLevel>>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _currencyPair = string.Empty;
        private OrderSide _orderSide;
        private SortedList<decimal, DepthLevel> _depthLevels = null;
 
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"></param>
        public DepthLevelMap(string currencyPair, OrderSide orderSide)
        {
            _currencyPair = currencyPair;
            _orderSide = orderSide;

            if (orderSide == OrderSide.Buy)
            {
                _depthLevels = new SortedList<decimal, DepthLevel>(new DescendingOrderComparer());
            }
            else
            {
                _depthLevels = new SortedList<decimal, DepthLevel>();
            }
        }

        /// <summary>
        /// Add to the depth Level
        /// </summary>
        /// <param name="price"></param>
        /// <param name="depthLevel"> </param>
        /// <returns></returns>
        internal bool AddLevel(Price price, DepthLevel depthLevel)
        {
            try
            {
                _depthLevels.Add(price.Value, depthLevel);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error("Could not add DepthLevel to DepthLevelMap. " + exception);
                return false;
            }
        }

        /// <summary>
        /// Add to the depth Level
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        internal bool RemoveLevel(Price price)
        {
            try
            {
                if (_depthLevels.ContainsKey(price.Value))
                {
                    _depthLevels.Remove(price.Value);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Could not add DepthLevel to DepthLevelMap. " + exception);
            }
            return false;
        }

        #region Properties

        /// <summary>
        /// The CurrencyPair for which this list specifies the OrderList
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// The CurrencyPair for which this list specifies the OrderList
        /// </summary>
        public OrderSide OrderSide
        {
            get { return _orderSide; }
        }

        #endregion Properties

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<decimal, DepthLevel>> GetEnumerator()
        {
            foreach (KeyValuePair<decimal, DepthLevel> keyValPair in _depthLevels)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (keyValPair.Value == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return keyValPair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
