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
        #region Properties

        public string TradeId { get; private set; }
        public decimal Price { get; private set; }
        public decimal Volume { get; private set; }
        public DateTime ExecutionDateTime { get; private set; }
        public string CurrencyPair { get; private set; }
        public string BuyOrderId { get; private set; }
        public string SellOrderId { get; private set; }
        public string BuyTraderId { get; private set; }
        public string SellTraderId { get; private set; }

        #endregion

        public TradeReadModel()
        {
            
        }

        public TradeReadModel(string sellTraderId, string buyTraderId, string sellOrderId, string buyOrderId, string currencyPair, DateTime executionDateTime, decimal volume, decimal price, string tradeId)
        {
            SellTraderId = sellTraderId;
            BuyTraderId = buyTraderId;
            SellOrderId = sellOrderId;
            BuyOrderId = buyOrderId;
            CurrencyPair = currencyPair;
            ExecutionDateTime = executionDateTime;
            Volume = volume;
            Price = price;
            TradeId = tradeId;
        }
    }
}
