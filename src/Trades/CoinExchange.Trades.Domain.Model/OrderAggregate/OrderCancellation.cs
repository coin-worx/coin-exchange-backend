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
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// VO that will represent cancelation request
    /// </summary>
    [Serializable]
    public class OrderCancellation
    {
        private TraderId _traderId;
        private OrderId _orderId;
        private string _currencyPair;

        public string CurrencyPair
        {
            get { return _currencyPair; }
            set
            {
                AssertionConcern.AssertEmptyString(value,"CurrencyPair cannot be null");
                _currencyPair = value;
            }
        }

        public OrderId OrderId
        {
            get { return _orderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value, "Order id cannot be null");
                _orderId = value;
            }
        }

        public TraderId TraderId
        {
            get { return _traderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value,"Trader id cannot be null");
                _traderId = value;
            }
        }

        public OrderCancellation()
        {
            
        }

        public OrderCancellation(OrderId orderId, TraderId traderId,string currencyPair)
        {
            OrderId = orderId;
            TraderId = traderId;
            CurrencyPair = currencyPair;
        }

       public override bool Equals(object obj)
        {
            OrderCancellation cancelOrder = obj as OrderCancellation;
            if (cancelOrder == null)
            {
                return false;
            }
            return (cancelOrder.OrderId.Id == this.OrderId.Id && cancelOrder.TraderId.Id == this.TraderId.Id&&cancelOrder.CurrencyPair==CurrencyPair);
        }

        /// <summary>
        /// Perform deep copy
        /// </summary>
        /// <param name="cancelOrder"></param>
        /// <returns></returns>
        public OrderCancellation MemberWiseClone(OrderCancellation cancelOrder)
        {
            cancelOrder.OrderId = new OrderId(OrderId.Id);
            cancelOrder.TraderId=new TraderId(TraderId.Id);
            cancelOrder.CurrencyPair = this.CurrencyPair;
            return cancelOrder;
        }
    }
}
