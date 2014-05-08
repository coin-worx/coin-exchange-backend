using System.Collections.Generic;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.OrderServices
{
    public interface IOrderQueryService
    {
        object GetOpenOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "");

        object GetClosedOrders(TraderId traderId, bool includeTrades = false,
            string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both");
    }
}
