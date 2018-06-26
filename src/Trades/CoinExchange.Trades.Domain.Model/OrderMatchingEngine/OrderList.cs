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

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the Orders for a specific side (Bids or Asks) as a collection
    /// </summary>
    [Serializable]
    public class OrderList : IEnumerable<Order>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Order> _orderList = new List<Order>();

        private string _currencyPair = null;
        private OrderSide _orderSide;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"> </param>
        public OrderList(string currencyPair, OrderSide orderSide)
        {
            _currencyPair = currencyPair;
            _orderSide = orderSide;
        }
  
        /// <summary>
        /// Add an Order to the List
        /// </summary>
        /// <returns></returns>
        internal bool Add(Order order)
        {
            // Check whether the incoming order is of the same CurrencyPair and Side for which this list was created
            if (order != null && order.CurrencyPair == _currencyPair && order.OrderSide == _orderSide)
            {
                // If yes, add the order, sort the list and log the details
                _orderList.Add(order);
                SortList();

                Log.Debug("Order added to the " + _orderSide.ToString() + " side list of th curreny pair: " + 
                    _currencyPair.ToString(CultureInfo.InvariantCulture) + ". Order = " + order.ToString());
                return true;
            }
            // Otherwise, log the error and return false
            Log.Debug("Order could not be added to " + _currencyPair + "'s " + _orderSide.ToString() + " list. Provided order" +
                      " does not belong to this list. Order: " + order);
            
            return false;
        }

        /// <summary>
        /// Updates the specified Order in the list
        /// </summary>
        /// <returns></returns>
        internal bool Update(OrderId orderId, string currencyPair, Volume volume, Price price)
        {
            if(_currencyPair == currencyPair)
            {
                Order selectedOrder = FindOrder(orderId);

                if (selectedOrder != null)
                {
                    selectedOrder.Volume = volume;
                    selectedOrder.Price = price;
                    return true;
                }
            }
            Log.Error("Order with OrderId = " + orderId + " not found in " + _currencyPair + " " + _orderSide.ToString() + 
                " list.");
            return false;
        }

        /// <summary>
        /// Cancels the specified Order in the list
        /// </summary>
        /// <returns></returns>
        internal bool Remove(Order order)
        {
            if (order != null)
            {
                _orderList.Remove(order);
                SortList();
                Log.Debug("Order Removed. Details: " + order);
                return true;
            }
            Log.Error("Order value null provided for Cancel.");
            return false;
        }

        /// <summary>
        /// Sorts the list in ascending order if the list is for Bids, and in descending order if the lsit is for Asks
        /// </summary>
        /// <returns></returns>
        private bool SortList()
        {
            switch (_orderSide)
            {
                // In case of sell, we need to sort the list in ascending order
                case OrderSide.Sell:
                    _orderList = _orderList.OrderBy(x => x.Price.Value).ToList();
                    return true;
                // In case of buy, we need to sort the list in Descending order
                case OrderSide.Buy:
                    _orderList = _orderList.OrderByDescending(x => x.Price.Value).ToList();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Selects an Order from the ORderList given the OrderID
        /// </summary>
        /// <returns></returns>
        public Order FindOrder(OrderId orderId)
        {
            if (orderId != null)
            {
                foreach (var order in _orderList)
                {
                    if (order.OrderId.Id == orderId.Id)
                    {
                        return order;
                    }
                }
            }
            return null;
        }

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

        #region Implementation of IEnumerable

        IEnumerator<Order> IEnumerable<Order>.GetEnumerator()
        {
            foreach (Order order in _orderList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (order == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return order;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (Order order in _orderList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (order == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return order;
            }
        }

        #endregion
    }
}
