using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Application.Order
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
