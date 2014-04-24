using System;
using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Services;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class TradeQueryService : NHibernateSessionFactory,ITradeQueryService
    {
        [Transaction(ReadOnly = true)]
        public List<object> GetRecentTrades(string lastId, string pair)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public List<TradeReadModel> GetTraderTradeHistory(string traderId)
        {
            throw new NotImplementedException();
        }
    }
}
