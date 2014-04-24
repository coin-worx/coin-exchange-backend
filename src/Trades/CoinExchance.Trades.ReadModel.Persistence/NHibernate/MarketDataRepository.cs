using System;
using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class MarketDataRepository:NHibernateSessionFactory,IMarketDataRepository
    {
        [Transaction(ReadOnly = true)]
        public List<object> GetOhlc(string currencyPair, int interval = 1, string since = "")
        {
            //TODO: Need to design views for it
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public TickerInfoReadModel GetTickerInfo(string currencyPair)
        {
            //TODO: Will have to keep it in memory
            throw new NotImplementedException();
        }
    }
}
