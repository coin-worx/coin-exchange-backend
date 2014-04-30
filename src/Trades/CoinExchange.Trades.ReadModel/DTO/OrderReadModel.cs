using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class OrderReadModel
    {
        #region properties

        public string OrderId { get; private set; }
        public string OrderType { get; private set; }
        public string OrderSide { get; private set; }
        public decimal Price { get; private set; }
        public decimal VolumeExecuted { get; private set; }
        public string Status { get; private set; }
        public string TraderId { get; private set; }
        public string CurrencyPair { get; private set; }
        public DateTime OrderDateTime { get; private set; }

        #endregion

        public OrderReadModel()
        {
            
        }

        public OrderReadModel(string orderId, string orderType, string orderSide, decimal price, decimal volumeExecuted, string traderId, string status, string currencyPair,DateTime dateTime)
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
        }
        
    }
}
