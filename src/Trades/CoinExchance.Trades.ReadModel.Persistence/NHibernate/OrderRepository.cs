using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Criterion;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class OrderRepository : NHibernateSessionFactory,IOrderRepository
    {
        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetOpenOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) && order.Status.Equals("Open"))
                .AsQueryable()
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) && order.Status.Equals("Closed"))
                .AsQueryable()
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetAllOrderOfTrader(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId))
                .AsQueryable()
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public OrderReadModel GetOrderById(string orderId)
        {
            return CurrentSession.Get<OrderReadModel>(orderId);
        }
    }
}
