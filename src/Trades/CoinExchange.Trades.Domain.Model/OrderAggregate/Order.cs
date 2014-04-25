using System;
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
        private OrderState _orderState;
        private Volume _filledQuantity;
        private Price _filledCost;
        private Volume _openQuantity;

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
            TraderId = traderId;

            _filledQuantity = new Volume(0);
            _filledCost = new Price(0);
            _openQuantity = new Volume(0);
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
            Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;

            _filledQuantity = new Volume(0);
            _filledCost = new Price(0);
            _openQuantity = new Volume(0);
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
            order.TraderId = new TraderId(this.TraderId.Id);
            order.CurrencyPair = this.CurrencyPair;
            order.Volume = new Volume(this.Volume.Value);
            order.VolumeExecuted = this.VolumeExecuted;
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
