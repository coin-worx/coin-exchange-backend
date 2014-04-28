using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Spring.Context.Support;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Factory for creating trades.
    /// </summary>
    public class TradeFactory
    {
        private static ITradeIdGenerator TradeIdGenerator
        {
            get { return ContextRegistry.GetContext()["TradeIdGenerator"] as ITradeIdGenerator; }
        }

        public static Trade GenerateTrade(string currencyPair, Price executionPrice, Volume executedQuantity,Order matchedOrder, Order inboundOrder)
        {
            Trade trade=new Trade(TradeIdGenerator.GenerateTradeId(),currencyPair, executionPrice, executedQuantity, DateTime.Now,
            matchedOrder, inboundOrder);
            return trade;
        }
    }
}
