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

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the list of OrderRepresentations as Volume and Price
    /// </summary>
    public class OrderRepresentationBookList : IEnumerable<OrderRepresentationList>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// </summary>
        private List<OrderRepresentationList> _orderRepresentationLists = new List<OrderRepresentationList>();

        /// <summary>
        /// Add an Order to the List
        /// </summary>
        /// <returns></returns>
        internal bool AddRecord(OrderRepresentationList orderRepresentationList)
        {
            _orderRepresentationLists.Add(orderRepresentationList);
            return true;
        }

        /// <summary>
        /// Removes an OrderRepresentation given the currencyPair and OrderSide
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        internal bool Remove(string currencyPair)
        {
            foreach (var orderRepresentationList in _orderRepresentationLists)
            {
                if (orderRepresentationList.CurrencyPair == currencyPair)
                {
                    _orderRepresentationLists.Remove(orderRepresentationList);
                    return true;
                }
            }
            return false;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<OrderRepresentationList> IEnumerable<OrderRepresentationList>.GetEnumerator()
        {
            foreach (OrderRepresentationList orderRepresentationList in _orderRepresentationLists)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderRepresentationList == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderRepresentationList;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (OrderRepresentationList orderRepresentations in _orderRepresentationLists)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderRepresentations == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderRepresentations;
            }
        }

        #endregion
    }
}
