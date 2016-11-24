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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains a list of LimitOrderBooks
    /// </summary>
    [Serializable]
    public class ExchangeEssentialsList : IEnumerable<ExchangeEssentials>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ExchangeEssentials> _orderBooksList = new List<ExchangeEssentials>();
        public DateTime LastSnapshotDateTime = DateTime.Now;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ExchangeEssentialsList()
        {
            
        }

        #region Methods

        /// <summary>
        /// Addds a LimitOrderBook to the Order books list
        /// </summary>
        /// <param name="exchangeEssentials"></param>
        internal bool AddEssentials(ExchangeEssentials exchangeEssentials)
        {
            if (!_orderBooksList.Contains(exchangeEssentials))
            {
                _orderBooksList.Add(exchangeEssentials);
                return true;
            }
            else
            {
                Log.Debug("Exchange essentials List already contains order book for: " + exchangeEssentials.LimitOrderBook.CurrencyPair);
            }
            return false;
        }

        /// <summary>
        /// Removes the order book from the list
        /// </summary>
        /// <returns></returns>
        internal bool RemoveEssentials(ExchangeEssentials exchangeEssentials)
        {
            if (_orderBooksList.Contains(exchangeEssentials))
            {
                _orderBooksList.Remove(exchangeEssentials);
                return true;
            }
            else
            {
                Log.Debug("Order book not present in ExchangeEssentialsList for: " + exchangeEssentials.LimitOrderBook.CurrencyPair);
            }
            return false;
        }

        #endregion Methods

        #region Implementation of IEnumerable

        public IEnumerator<ExchangeEssentials> GetEnumerator()
        {
            foreach (ExchangeEssentials order in _orderBooksList)
            {
                if (order == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return order;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
