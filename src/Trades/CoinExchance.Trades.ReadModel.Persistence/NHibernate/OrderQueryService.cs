using System;
using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Services;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class OrderQueryService : NHibernateSessionFactory,IOrderQueryService
    {
        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetOpenOrders(string traderId)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetAllOrderOfTrader(string traderId)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public OrderReadModel GetOrderById(string orderId)
        {
            throw new NotImplementedException();
        }
    }
}
