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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Handles the change in the state of any order and publishes to the output disruptor
    /// </summary>
    [Serializable]
    public class OrderListener : IOrderListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Handles the OrderBook's event in case the state of an Order changes
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderChanged(Order order)
        {
            OutputDisruptor.Publish(order);
            Log.Debug("Order change received and published. Order: " + order.ToString());
        }

        /// <summary>
        /// When Order gets accepted
        /// </summary>
        /// <param name="order"></param>
        /// <param name="matchedPrice"></param>
        /// <param name="matchedVolume"></param>
        public void OnOrderAccepted(Order order, Price matchedPrice, Volume matchedVolume)
        {
            /*OutputDisruptor.Publish(order);
            Log.Debug("Order Accepted: " + order.ToString() + " | Published to output Disruptor");*/
        }

        /// <summary>
        /// When Order gets filled
        /// </summary>
        /// <param name="inboundOrder"></param>
        /// <param name="matchedOrder"></param>
        /// <param name="fillFlags"></param>
        /// <param name="filledPrice"></param>
        /// <param name="filledVolume"></param>
        public void OnOrderFilled(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price filledPrice, Volume filledVolume)
        {
            /*// Publish the order that was just received from the user and got filled
            OutputDisruptor.Publish(inboundOrder);
            Log.Debug("Order Filled: " + inboundOrder.ToString() + " | Published to output Disruptor.");

            // Publish the order that was on the order book and got matched with the incoming order
            OutputDisruptor.Publish(matchedOrder);
            Log.Debug("Order Filled: " + matchedOrder.ToString() + " | Published to output Disruptor.");*/
        }

        /// <summary>
        /// handles the event in case an order gets cancelled
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderCancelled(Order order)
        {
            /*OutputDisruptor.Publish(order);
            Log.Debug("Order Cancelled: " + order.ToString() + " | Published to output Disruptor.");*/
        }
    }
}
