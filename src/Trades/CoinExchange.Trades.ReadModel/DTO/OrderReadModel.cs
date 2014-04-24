using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
