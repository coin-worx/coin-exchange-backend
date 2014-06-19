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
        public string Type { get; private set; }
        public string Side { get; private set; }
        public decimal Price { get; private set; }
        public decimal VolumeExecuted { get; private set; }
        public decimal Volume { get; private set; }
        public decimal OpenQuantity { get; private set; }
        public string Status { get; private set; }
        public string TraderId { get; private set; }
        public string CurrencyPair { get; private set; }
        public DateTime DateTime { get; private set; }
        public DateTime? ClosingDateTime { get; private set; }
        public decimal AveragePrice { get;  set; }
        public IList<object> Trades { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderReadModel()
        {
            
        }

        public OrderReadModel(string orderId, string orderType, string orderSide, decimal price, decimal volumeExecuted, string traderId, string status, string currencyPair,DateTime dateTime,decimal volume,decimal openQuantity, DateTime? closingDateTime)
        {
            OrderId = orderId;
            Type = orderType;
            Side = orderSide;
            Price = price;
            VolumeExecuted = volumeExecuted;
            TraderId = traderId;
            Status = status;
            CurrencyPair = currencyPair;
            DateTime = dateTime;
            Volume = volume;
            OpenQuantity = openQuantity;
            ClosingDateTime = closingDateTime;
        }
        
    }
}
