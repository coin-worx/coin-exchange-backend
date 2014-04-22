using System.Collections.Generic;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.OrderServices
{
    public interface IOrderQueryService
    {
        List<OrderRepresentation> GetOpenOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "");

        List<OrderRepresentation> GetClosedOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both");

        List<object> GetOrderBook(string symbol, int count);

    }
}
