using System.Collections.Generic;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Application.OrderServices
{
    public interface IOrderQueryService
    {
        List<Domain.Model.Order.Order> GetOpenOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "");

        List<Domain.Model.Order.Order> GetClosedOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both");

        List<object> GetOrderBook(string symbol, int count);

    }
}
