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
        public string OrderId { get; set; }
        public string OrderType { get; set; }
        public string OrderSide { get; set; }
        public decimal Price { get; set; }
        public decimal VolumeExecuted { get; set; }
        public string Status { get; set; }
        public string TraderId { get; set; }
        public string CurrencyPair { get; set; }
        //public List<TradeReadModel> Trades;

        public static OrderReadModel CreateOrderReadModel(Order order)
        {
            OrderReadModel readModel=new OrderReadModel();
            readModel.OrderId = order.OrderId.Id.ToString();
            readModel.OrderSide = order.OrderSide.ToString();
            readModel.OrderType = order.OrderType.ToString();
            readModel.Price = order.Price.Value;
            readModel.Status = order.OrderState.ToString();
            readModel.TraderId = order.TraderId.Id.ToString();
            readModel.VolumeExecuted = order.VolumeExecuted;
            readModel.CurrencyPair = order.CurrencyPair;
            return readModel;
        }
    }
}
