using System;
using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Services;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class MarketDataQueryService:NHibernateSessionFactory,IMarketDataQueryService
    {
        [Transaction(ReadOnly = true)]
        public List<object> GetOhlc(string currencyPair, int interval = 1, string since = "")
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = true)]
        public TickerInfoReadModel GetTickerInfo(string currencyPair)
        {
            throw new NotImplementedException();
        }
    }
}
