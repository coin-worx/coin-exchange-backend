using System;
using System.Linq;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class TickerInfoRepository:NHibernateSessionFactory,ITickerInfoRepository
    {
        [Transaction(ReadOnly = true)]
        public TickerInfoReadModel GetTickerInfoByCurrencyPair(string currencyPair)
        {
            return
                CurrentSession.Query<TickerInfoReadModel>()
                    .Where(x => x.CurrencyPair.Equals(currencyPair))
                    .AsQueryable()
                    .SingleOrDefault();
        }
    }
}
