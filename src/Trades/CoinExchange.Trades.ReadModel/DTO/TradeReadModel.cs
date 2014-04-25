using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

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

        public static TradeReadModel CreateTradeReadModel(Trade trade)
        {
            TradeReadModel readModel=new TradeReadModel();
            readModel.CurrencyPair = trade.CurrencyPair;
            readModel.ExecutionDateTime = trade.ExecutionTime;
            readModel.OrderId = trade.BuyOrder.OrderId.Id.ToString();
            readModel.Price = trade.ExecutionPrice.Value;
            //Todo: have to put traderid
            return readModel;
        }
    }
}
