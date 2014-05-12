using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class ReadModelAdapter
    {
        public static OrderReadModel GetOrderReadModel(Order order)
        {
            OrderReadModel model = new OrderReadModel(order.OrderId.Id.ToString(), order.OrderType.ToString(),
                order.OrderSide.ToString(), order.Price.Value, order.FilledQuantity.Value, order.TraderId.Id.ToString(),
                order.OrderState.ToString(), order.CurrencyPair,order.DateTime,order.OpenQuantity.Value,order.OpenQuantity.Value);
            return model;
        }

        public static TradeReadModel GetTradeReadModel(Trade trade)
        {
            TradeReadModel model = new TradeReadModel(trade.SellOrder.TraderId.Id.ToString(),
                trade.BuyOrder.TraderId.Id.ToString(), trade.SellOrder.OrderId.Id.ToString(),
                trade.BuyOrder.OrderId.Id.ToString(), trade.CurrencyPair, trade.ExecutionTime,
                trade.ExecutedVolume.Value, trade.ExecutionPrice.Value, trade.TradeId.Id.ToString());
            return model;
        }
    }
}
