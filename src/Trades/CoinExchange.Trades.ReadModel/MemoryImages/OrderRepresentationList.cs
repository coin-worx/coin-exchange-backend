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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the OrderBook in a simplified form to be shown on the UI. Contains
    /// 1. Volume
    /// 2. Price
    /// in every slot
    /// </summary>
    public class OrderRepresentationList : IEnumerable<OrderRecord>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// </summary>
        private List<OrderRecord> _orderRecordList = new List<OrderRecord>();

        private string _currencyPair = null;
        private OrderSide _orderSide;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"> </param>
        public OrderRepresentationList(string currencyPair, OrderSide orderSide)
        {
            _currencyPair = currencyPair;
            _orderSide = orderSide;
        }
  
        /// <summary>
        /// Add an Order to the List
        /// </summary>
        /// <returns></returns>
        public bool AddRecord(/*string currencyPair, OrderSide orderSide, */decimal volume, decimal price,DateTime dateTime)
        {
            /*if (volume != 0 && currencyPair == _currencyPair && orderSide == _orderSide)
            {*/
                _orderRecordList.Add(new OrderRecord(price,volume,dateTime));

                Log.Debug("New Order record added: CurrencyPair = " + _currencyPair + " | Volume = " + volume + " | " +
                          "Price = " + price);
                return true;
            //}
            // Otherwise, log the error and return false
            Log.Debug("Wrong/non-matching Currencypair or OrderSide provided.");
            
            return false;
        }

        /// <summary>
        /// Updates the price and volume at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool UpdateAtIndex(int index, decimal volume, decimal price,DateTime dateTime)
        {
            if (index < _orderRecordList.Count)
            {
                _orderRecordList[index] = new OrderRecord(price, volume,dateTime);
                return true;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
            return false;
        }

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Orderside
        /// </summary>
        public OrderSide OrderSide
        {
            get { return _orderSide; }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<OrderRecord> IEnumerable<OrderRecord>.GetEnumerator()
        {
            foreach (var orderStats in _orderRecordList)
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
            foreach (OrderRecord orderStats in _orderRecordList)
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
