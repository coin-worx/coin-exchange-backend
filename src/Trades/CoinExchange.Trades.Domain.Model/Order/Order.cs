using System;
using System.Collections.Generic;
/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// CoinExchange Order
    /// </summary>
    public class Order
    {
        private OrderId _orderId = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Order()
        {
            
        }
        /// <summary>
        /// Factory Constructor
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderType"></param>
        /// <param name="volume"></param>
        /// <param name="traderId"></param>
        public Order(string pair, decimal price, OrderSide orderSide, OrderType orderType, decimal volume,string traderId)
        {
            Pair = pair;
            Price = price;
            OrderSide = orderSide;
            OrderType = orderType;
            Volume = volume;
            TraderId = traderId;
        }

        /// <summary>
        /// Comma delimited list of transaction ids for order
        /// </summary>
        public string TxId { get; set; }

        /// <summary>
        /// Immutable ID for this order
        /// </summary>
        public OrderId OrderId { get { return _orderId; } }

        /// <summary>
        /// Expiration time. Optional
        /// </summary>
        public string UserRefId { get; set; }

        /// <summary>
        /// Asset pair
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Type of order (buy or sell)
        /// True = Sell, False = Buy
        /// </summary>
        public bool IsSell { get; set; }

        /// <summary>
        /// Order side, buy or sell
        /// </summary>
        public OrderSide OrderSide { get; set; }

        /// <summary>
        /// Type of Order
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Secondary price. Optional. Dependent upon order type
        /// </summary>
        public decimal? Price2 { get; set; }

        /// <summary>
        /// Order volume in lots
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Amount of leverage required. Optional. default none
        /// </summary>
        public string Leverage { get; set; }

        /// <summary>
        /// Position tx id to close (optional.  used to close positions)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// List of order flags (optional):
        /// </summary>
        public string OFlags { get; set; }

        /// <summary>
        /// Scheduled start time. Optional
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Expiration time. Optional
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// Validate inputs only. do not submit order. Optional
        /// </summary>
        public bool Validate { get; set; }

        /// <summary>
        /// Closing order details
        /// </summary>
        public Dictionary<string, string> Close { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Timestamp of when order was placed
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Timestamp of when order was closed
        /// </summary>
        public string CloseTime { get; set; }

        /// <summary>
        /// Volume executed
        /// </summary>
        public double? VolumeExecuted { get; set; }

        /// <summary>
        /// Total cost
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// Total fee
        /// </summary>
        public decimal? Fee { get; set; }

        /// <summary>
        /// AveragePrice executed
        /// </summary>
        public decimal? AveragePrice { get; set; }

        /// <summary>
        /// Stop price (for trailing stops)
        /// </summary>
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Triggered limit price (when limit based ordertype triggered)
        /// </summary>
        public decimal? LimitPrice { get; set; }

        /// <summary>
        /// Comma delimited list of miscellaneous info
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Comma delimited list of trade ids related to order 
        /// </summary>
        public string Trades { get; set; }

        /// <summary>
        /// In case of Open order, the opening time
        /// </summary>
        public string Opened { get; set; }

        /// <summary>
        /// The time when the order closed
        /// </summary>
        public string Closed { get; set; }

        /// <summary>
        /// The time when order executed
        /// </summary>
        public string Executed { get; set; }

        /// <summary>
        /// Trader id
        /// </summary>
        public string TraderId { get; set; }
    }
}
