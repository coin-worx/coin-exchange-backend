using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    /// <summary>
    /// Currency pair repository implementation
    /// </summary>
    [Repository]
    public class CurrencyPairRepository:NHibernateSessionFactory,ICurrencyPairRepository
    {
        [Transaction(ReadOnly = true)]
        public CurrencyPair GetById(string currencyPair)
        {
            return CurrentSession.Get<CurrencyPair>(currencyPair);
        }

        [Transaction(ReadOnly = true)]
        public IList<CurrencyPair> GetTradeableCurrencyPairs()
        {
            return CurrentSession.CreateCriteria<CurrencyPair>().List<CurrencyPair>();
        }
    }
}
