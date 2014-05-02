using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class TradeRepository : NHibernateSessionFactory,ITradeRepository
    {
        [Transaction(ReadOnly = true)]
        public IList<object> GetRecentTrades(string lastId, string pair)
        {
            return
                CurrentSession.QueryOver<TradeReadModel>()
                    .Select(t => t.ExecutionDateTime, t => t.Price, t => t.Volume)
                    .List<object>();

        }

        [Transaction(ReadOnly = true)]
        public IList<TradeReadModel> GetTraderTradeHistory(string traderId)
        {
            return CurrentSession.Query<TradeReadModel>()
                .Where(trade => trade.BuyTraderId.Equals(traderId)||trade.SellTraderId.Equals(traderId))
                .AsQueryable()
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public TradeReadModel GetById(string tradeId)
        {
            return CurrentSession.Get<TradeReadModel>(tradeId);
        }
    }
}
