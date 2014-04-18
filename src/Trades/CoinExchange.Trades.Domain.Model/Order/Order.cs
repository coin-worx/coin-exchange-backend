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
    public class Order : IComparable<Order>
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
        private OrderState _orderStatus;
        private Volume _filledQuantity;
        private Price _filledCost;
        private Volume _openQty;

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
        /// <param name="pair"></param>
        /// <param name="price"></param>
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
        /// <param name="pair"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(OrderId orderId, string pair, Price price, OrderSide orderSide, OrderType orderType, Volume volume, TraderId traderId)
        {
            OrderId = orderId;
            CurrencyPair = pair;
            Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;
        }

        #region Methods

        /// <summary>
        /// If order gets accepted by the Exchange
        /// </summary>
        public void Accepted()
        {
            if (_orderStatus == OrderState.New)
            {
                _orderStatus = OrderState.Accepted;
            }
        }

        /// <summary>
        /// If order gets cancelled
        /// </summary>
        public void Cancelled()
        {
            if (_orderStatus != OrderState.Complete)
            {
                _orderStatus = OrderState.Cancelled;
            }
        }

        /// <summary>
        /// If order gets Rejected
        /// </summary>
        public void Rejected()
        {
            if (_orderStatus != OrderState.Accepted)
            {
                _orderStatus = OrderState.Rejected;
            }
        }

        /// <summary>
        /// If trader modifies the order
        /// </summary>
        public void UpdateOrder(Price price, Volume volume)
        {
            if (_orderStatus == OrderState.Accepted)
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
            if (_orderStatus == OrderState.Accepted)
            {
                _volume = volume;
            }
        }

        /// <summary>
        /// If trader modifies the Price of the order
        /// </summary>
        public void UpdatePrice(Price price)
        {
            if (_orderStatus == OrderState.Accepted)
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
            // ToDo: Update filled cost here
            _filledCost += filledCost;
            if (_openQty.Value == 0)
            {
                _orderStatus = OrderState.Complete;
            }
            else
            {
                _orderStatus = OrderState.PartiallyFilled;
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
                _volume = value;
            }
        }
        
        /// <summary>
        /// Order Status
        /// </summary>
        public OrderState Status
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

        /// <summary>
        /// The amount filled
        /// </summary>
        public Volume FilledQuantity
        {
            get
            {
                return _filledQuantity;
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
        }

        /// <summary>
        /// The cost of the order after filling
        /// </summary>
        public Volume OpenQuantity
        {
            get
            {
                if (_filledQuantity.Value < _volume.Value)
                {
                    return new Volume(_volume.Value - _filledQuantity.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        #region Implementation of IComparable<in Order>

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
