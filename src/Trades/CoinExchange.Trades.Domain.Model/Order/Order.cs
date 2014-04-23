using System;
using System.Collections.Generic;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.Trades;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// CoinExchange Order
    /// </summary>
    public class Order
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
        private OrderStatus _orderStatus;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Order()
        {
            
        }

        //TODO: waqas bhai please delete this constructor and use another for creating the order
        public Order(string pair, decimal price, OrderSide orderSide, OrderType orderType, decimal volume, TraderId traderId)
        {
            CurrencyPair = pair;
            //Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            //Volume = volume;
            TraderId = traderId;
        }

        /// <summary>
        /// Factory Constructor for market order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pair"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(OrderId orderId,string pair, OrderSide orderSide, OrderType orderType, Volume volume, TraderId traderId)
        {
            OrderId = orderId;
            CurrencyPair = pair;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;
        }

        /// <summary>
        /// Factory Constructor for limit order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pair"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(OrderId orderId, string pair, Price price, OrderSide orderSide, OrderType orderType, Volume volume, TraderId traderId)
        {
            AssertionConcern.AssertArgumentNotNull(price, "Limit Price not specified");
            AssertionConcern.AssertGreaterThanZero(price.Value, "Limit price must be greater than 0");
            OrderId = orderId;
            CurrencyPair = pair;
            Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;
        }

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
                _volume = value;
            }
        }
        
        /// <summary>
        /// Order Status
        /// </summary>
        public OrderStatus Status
        {
            get { return _orderStatus; }
            set
            {
                _orderStatus = value;
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

        public override bool Equals(object obj)
        {
            Order order = obj as Order;
            if (order == null)
                return false;
            return order.OrderId.Id == this.OrderId.Id;
        }

        /// <summary>
        /// Perform deep copy
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public Order MemberWiseClone(Order order)
        {
            order.OrderId=new OrderId(this.OrderId.Id);
            order.OrderSide=this.OrderSide;
            order.OrderType = this.OrderType;
            order.Status = this.Status;
            if (OrderType == OrderType.Limit)
            {
                order.Price = new Price(_price.Value);
            }
            order.TraderId=new TraderId(this.TraderId.Id);
            order.CurrencyPair = this.CurrencyPair;
            order.Volume=new Volume(this.Volume.Value);
            order.VolumeExecuted = this.VolumeExecuted;
            return order;
        }
    }
}
