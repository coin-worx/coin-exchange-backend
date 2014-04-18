using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.OrderServices.Representation
{
    /// <summary>
    /// Order representation for clients
    /// </summary>
    public class OrderRepresentation
    {
        private OrderId _orderId = null;
        
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
        /// Order volume in lots
        /// </summary>
        public decimal Volume { get; set; }

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
        /// Order Status
        /// </summary>
        public OrderState Status { get; set; }

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
