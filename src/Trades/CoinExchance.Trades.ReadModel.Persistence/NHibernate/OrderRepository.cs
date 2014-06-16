using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Criterion;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    /// <summary>
    /// Order repository implementation
    /// </summary>
    [Repository]
    public class OrderRepository : NHibernateSessionFactory,IOrderRepository
    {
        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetOpenOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) && 
                (order.Status.Equals("New") || order.Status.Equals("Accepted") || order.Status.Equals("PartiallyFilled")))
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId,DateTime start,DateTime end)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) &&
                      (order.Status.Equals("Cancelled") || order.Status.Equals("Rejected") || order.Status.Equals("Complete"))
                      && order.DateTime >= start && order.DateTime <= end)
                .AsQueryable()
                .OrderBy(x => x.DateTime)
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) &&
                (order.Status.Equals("Cancelled") || order.Status.Equals("Rejected") || order.Status.Equals("Complete")))
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
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

        [Transaction(ReadOnly = true)]
        public OrderReadModel GetOrderById(TraderId traderId,OrderId orderId)
        {
            OrderReadModel model=CurrentSession.Get<OrderReadModel>(orderId.Id);
            if (model.TraderId.Equals(traderId.Id, StringComparison.InvariantCultureIgnoreCase))
                return model;
            return null;
        }

        [Transaction(ReadOnly = false)]
        public void RollBack()
        {
            string sqlQuery = string.Format("DELETE FROM COINEXCHANGE.ORDER");

            CurrentSession.CreateSQLQuery(sqlQuery).ExecuteUpdate();
        }
    }
}
