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
/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */
namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// CoinExchange Order
    /// </summary>
    [Serializable]
    public class Order: IComparable<Order>
    {
        #region Private fields

        private OrderId _orderId = null;
        private Volume _volume=null;
        private Price _price=null;
        private decimal _volumeExecuted;
        private TraderId _traderId;
        private string _currencyPair;
        private OrderSide _orderSide;
        private OrderType _orderType;
        private OrderState _orderState;
        private Volume _filledQuantity;
        private Price _filledCost;
        private Volume _openQuantity;
        private DateTime _dateTime;

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Order()
        {
            
        }

        /// <summary>
        /// Factory Constructor for market order
        /// </summary>
        /// <param name="orderId"> </param>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(OrderId orderId, string currencyPair, OrderSide orderSide, OrderType orderType, Volume volume, TraderId traderId)
        {
            OrderId = orderId;
            CurrencyPair = currencyPair;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            Price = new Price(0);
            TraderId = traderId;
            this.DateTime = DateTime.Now;
            FilledQuantity = new Volume(0);
            FilledCost = new Price(0);
            OpenQuantity = Volume;
        }

        /// <summary>
        /// Factory Constructor for limit order
        /// </summary>
        /// <param name="orderId"> </param>
        /// <param name="pair"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(OrderId orderId, string pair, Price price, OrderSide orderSide, OrderType orderType, Volume volume,
            TraderId traderId)
        {
            OrderId = orderId;
            CurrencyPair = pair;
            if (orderType == OrderType.Limit)
            {
                AssertionConcern.AssertGreaterThanZero(price.Value,"Limit order price must be greater than 0");
            }
            Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;
            this.DateTime = DateTime.Now;
            FilledQuantity = new Volume(0);
            _filledCost = new Price(0);
            OpenQuantity = Volume;
        }

        #region Methods

        /// <summary>
        /// If order gets accepted by the Exchange
        /// </summary>
        public void Accepted()
        {
            if (_orderState == OrderState.New)
            {
                _orderState = OrderState.Accepted;
            }
        }

        /// <summary>
        /// If order gets cancelled
        /// </summary>
        public void Cancelled()
        {
            if (_orderState != OrderState.Complete)
            {
                _orderState = OrderState.Cancelled;
            }
        }

        /// <summary>
        /// If order gets Rejected
        /// </summary>
        public void Rejected()
        {
            if (_orderState != OrderState.Accepted)
            {
                _orderState = OrderState.Rejected;
            }
        }

        /// <summary>
        /// If trader modifies the order
        /// </summary>
        public void UpdateOrder(Price price, Volume volume)
        {
            if (_orderState == OrderState.Accepted)
            {
                _price = price;
                _volume = volume;
            }
        }

        /// <summary>
        /// If trader modifies the Volume of the order
        /// </summary>
        /// <param name="volume"></param>
        public void UpdateVolume(Volume volume)
        {
            if (_orderState == OrderState.Accepted)
            {
                _volume = volume;
            }
        }

        /// <summary>
        /// If trader modifies the Price of the order
        /// </summary>
        public void UpdatePrice(Price price)
        {
            if (_orderState == OrderState.Accepted)
            {
                _price = price;
            }
        }
        
        /// <summary>
        /// When an order gets filled
        /// </summary>
        public void Fill(Volume filledQuantity, Price filledCost)
        {
            _filledQuantity += filledQuantity;
            _filledCost += filledCost;
            _openQuantity = _volume - _filledQuantity;
            if (_openQuantity.Value == 0)
            {
                _orderState = OrderState.Complete;
            }
            else
            {
                _orderState = OrderState.PartiallyFilled;
            }
        }

        #endregion Methods

        /// <summary>
        /// Immutable ID for this order
        /// </summary>
        public OrderId OrderId
        {
            get { return _orderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value, "OrderId cannot be null");
                _orderId = value;
            }
        }
        /// <summary>
        /// Order date time
        /// </summary>
        public DateTime DateTime
        {
            get { return _dateTime; }
            private set
            {
                _dateTime = value;
            }
        }
        /// <summary>
        /// Asset currency pair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
            set
            {
                AssertionConcern.AssertEmptyString(value,"Currency pair not specifed");
                _currencyPair = value;
            }
        }
        
        /// <summary>
        /// Order side, buy or sell
        /// </summary>
        public OrderSide OrderSide
        {
            get { return _orderSide; }
            set
            {
                _orderSide = value;
            }
        }

        /// <summary>
        /// Type of Order
        /// </summary>
        public OrderType OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
            }
        }

        /// <summary>
        /// Limit Price
        /// </summary>
        public Price Price
        {
            get { return _price; }
            set
            {
                AssertionConcern.AssertArgumentNotNull(value,"Limit Price not specified");
                _price = value;
            }
        }

        /// <summary>
        /// Order volume in lots
        /// </summary>
        public Volume Volume
        {
            get { return _volume; }
            set
            {
                AssertionConcern.AssertArgumentNotNull(value,"Volume not specified");
                AssertionConcern.AssertGreaterThanZero(value.Value,"Volume must be greater than 0");
                _volume = value;
            }
        }
        
        /// <summary>
        /// Order Status
        /// </summary>
        public OrderState OrderState
        {
            get { return _orderState; }
            set
            {
                _orderState = value;
            }
        }
        
        /// <summary>
        /// Volume executed
        /// </summary>
        public decimal VolumeExecuted
        {
            get { return _volumeExecuted; }
            set
            {
                _volumeExecuted = value;
            }
        }

        /// <summary>
        /// Trader id
        /// </summary>
        public TraderId TraderId
        {
            get { return _traderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value, "TraderId cannot be null");
                _traderId = value;
            }
        }

        /// <summary>
        /// The amount filled
        /// </summary>
        public Volume FilledQuantity
        {
            get
            {
                return _filledQuantity;
            }
            private set
            {
                _filledQuantity = value;
            }
        }

        /// <summary>
        /// The cost of the order after filling
        /// </summary>
        public Price FilledCost
        {
            get
            {
                return _filledCost;
            }
            private  set
            {
                _filledCost = value;
            }
        }

        /// <summary>
        /// The remaining quantity of the order which has not yet been filled
        /// </summary>
        public Volume OpenQuantity
        {
            get
            {
                if (_openQuantity == null)
                {
                    _openQuantity = new Volume(0);
                }

                if (_filledQuantity != null && _volume != null)
                {
                    if (_filledQuantity.Value <= _volume.Value)
                    {
                        _openQuantity = new Volume(_volume.Value - _filledQuantity.Value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Filled quantity exceeding Original quantity. Order: " + this.ToString());
                    }
                }
                return _openQuantity;
            }
            private set
            {
                _openQuantity = value;
            } 
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is Order)
                {
                    return OrderId.Equals((obj as Order).OrderId);
                }
                //if (obj is OrderId)
                //{
                //    return OrderId.Equals(obj);
                //}
            }
            return false;
        }

        /// <summary>
        /// Perform deep copy
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public Order MemberWiseClone(Order order)
        {
            order.OrderId = new OrderId(this.OrderId.Id);
            order.OrderSide = this.OrderSide;
            order.OrderType = this.OrderType;
            order.OrderState = this.OrderState;
            if (OrderType == OrderType.Limit)
            {
                order.Price = new Price(_price.Value);
            }
            else
            {
                order.Price=new Price(0);
            }
            order.TraderId = new TraderId(this.TraderId.Id);
            order.CurrencyPair = this.CurrencyPair;
            order.Volume = new Volume(this.Volume.Value);
            order.VolumeExecuted = this.VolumeExecuted;
            order.OpenQuantity = this.OpenQuantity;
            order.FilledQuantity = this.FilledQuantity;
            order.FilledCost = this.FilledCost;
            order.DateTime = this.DateTime;
            return order;
        }

        #region Implementation of IComparable<in Order>

        /// <summary>
        ///  Comapres the order based on the price
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Order other)
        {
            if (this.Price.IsGreaterThan(other.Price))
            {
                return -1;
            }
            if (this.Price.Equals(other.Price))
            {
                return 0;
            }
            return 1;
        }

        #endregion
    }
}
