using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class TradeReadModel
    {
        public string TradeId { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string CurrencyPair { get; set; }
        public string OrderId { get; set; }
        public string TraderId { get; set; }
    }
}
