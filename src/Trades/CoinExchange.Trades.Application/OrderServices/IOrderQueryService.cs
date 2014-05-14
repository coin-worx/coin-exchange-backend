using System.Collections.Generic;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.OrderServices
{
    public interface IOrderQueryService
    {
        object GetOpenOrders(TraderId traderId, bool includeTrades = false);

        object GetClosedOrders(TraderId traderId, bool includeTrades = false,
            string startTime = "", string endTime = "");

        object GetOrderById(TraderId traderId,OrderId orderId);
    }
}
