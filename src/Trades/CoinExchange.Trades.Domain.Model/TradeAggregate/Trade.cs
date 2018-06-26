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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using Spring.Validation;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Result of a bid and ask crossing
    /// </summary>
    [Serializable]
    public class Trade
    {
        private readonly string _aggregateId;
        private string _currencyPair = string.Empty;
        private Price _executionPrice = null;
        private Volume _executedQuantity = null;
        private DateTime _executionTime = DateTime.MinValue;
        private Order _buyOrder = null;
        private Order _sellOrder = null;
        private TradeId _tradeId;

        /// <summary>
        /// Default Constructor
        /// </summary>
        //public Trade(string currencyPair, Price executionPrice, Volume executedQuantity, DateTime executionTime,
        //    Order matchedOrder, Order inboundOrder)
        //{
        //    _currencyPair = currencyPair;
        //    _executionPrice = executionPrice;
        //    _executedQuantity = executedQuantity;
        //    _executionTime = executionTime;
        //    if (matchedOrder.OrderSide == OrderSide.Buy)
        //    {
        //        _buyOrder = matchedOrder;
        //        _sellOrder = inboundOrder;
        //    }
        //    else
        //    {
        //        _buyOrder = inboundOrder;
        //        _sellOrder = matchedOrder;
        //    }

        //    // ToDo: Need to implement auto incremental aggregate Id generator 
        //}

        public Trade()
        {
            
        }

        /// <summary>
        /// Factory Constructor
        /// </summary>
        public Trade(TradeId tradeId,string currencyPair, Price executionPrice, Volume executedQuantity, DateTime executionTime,
            Order matchedOrder, Order inboundOrder)
        {
            TradeId = tradeId;
            CurrencyPair = currencyPair;
            ExecutionPrice = executionPrice;
            _executedQuantity = executedQuantity;
            ExecutionTime = executionTime;
            if (matchedOrder.OrderSide == OrderSide.Buy)
            {
                _buyOrder = matchedOrder;
                _sellOrder = inboundOrder;
            }
            else
            {
                _buyOrder = inboundOrder;
                _sellOrder = matchedOrder;
            }
        }

        /// <summary>
        /// Raise the TradeExecutedEvent
        /// </summary>
        public TradeExecutedEvent RaiseEvent()
        {
            TradeExecutedEvent tradeExecutedEvent = new TradeExecutedEvent(_aggregateId, this);
            return tradeExecutedEvent;
        }

        /// <summary>
        /// TradeId
        /// </summary>
        public TradeId TradeId
        {
            get { return _tradeId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value,"TradeId cannot be null");
                _tradeId = value;
            }
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
            private set
            {
                _currencyPair = value;
            }
        }

        /// <summary>
        /// Execution Price
        /// </summary>
        public Price ExecutionPrice
        {
            get { return _executionPrice; }
            private set
            {
                _executionPrice = value;
            }

        }

        /// <summary>
        /// Executed Quantity
        /// </summary>
        public Volume ExecutedVolume
        {
            get { return _executedQuantity; }
            private set
            {
                _executedQuantity = value;
            }
        }

        /// <summary>
        /// ExecutionTime
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return _executionTime; }
            private set
            {
                _executionTime = value;
            }
        }

        /// <summary>
        /// Buy Order Reference
        /// </summary>
        public Order BuyOrder
        {
            get { return _buyOrder; }
            private set { _buyOrder = value; }
        }

        /// <summary>
        /// Sell Order Reference
        /// </summary>
        public Order SellOrder
        {
            get { return _sellOrder; }
            private set
            {
                _sellOrder = value;
            }
        }
    }
}
