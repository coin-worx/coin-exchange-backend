using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// IEventStore
    /// </summary>
    public interface IEventStore
    {
        bool StoreEvent(object blob);
        object GetEvent(string id);
        List<Order> GetAllOrders();
        void RemoveAllEvents();
        List<Order> GetOrdersByCurrencyPair(string currencyPair);
        IList<Trade> GetTradeEventsFromOrderId(string id);
        IList<object> GetAllEvents();
        void ShutDown();
        void SaveSnapshot(ExchangeEssentialsList exchangeEssentials);
        ExchangeEssentialsList LoadLastSnapshot();
    }
}
