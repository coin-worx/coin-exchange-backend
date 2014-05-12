using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.ReadModel.DTO
{
    /// <summary>
    /// OrderReadModel
    /// </summary>
    public class OrderReadModel
    {
        #region properties

        public string OrderId { get; private set; }
        public string OrderType { get; private set; }
        public string OrderSide { get; private set; }
        public decimal Price { get; private set; }
        public decimal VolumeExecuted { get; private set; }
        public decimal Volume { get; private set; }
        public decimal OpenQuantity { get; private set; }
        public string Status { get; private set; }
        public string TraderId { get; private set; }
        public string CurrencyPair { get; private set; }
        public DateTime OrderDateTime { get; private set; }
        public IList<object> Trades { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderReadModel()
        {
            
        }

        public OrderReadModel(string orderId, string orderType, string orderSide, decimal price, decimal volumeExecuted, string traderId, string status, string currencyPair,DateTime dateTime,decimal volume,decimal openQuantity)
        {
            OrderId = orderId;
            OrderType = orderType;
            OrderSide = orderSide;
            Price = price;
            VolumeExecuted = volumeExecuted;
            TraderId = traderId;
            Status = status;
            CurrencyPair = currencyPair;
            OrderDateTime = dateTime;
            Volume = volume;
            OpenQuantity = openQuantity;
        }
        
    }
}
