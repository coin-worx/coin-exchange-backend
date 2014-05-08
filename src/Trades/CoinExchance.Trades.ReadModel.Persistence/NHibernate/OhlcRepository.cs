using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class OhlcRepository:NHibernateSessionFactory,IOhlcRepository
    {
        [Transaction]
        public OhlcReadModel GetOhlcByDateTime(DateTime dateTime)
        {
            return CurrentSession.QueryOver<OhlcReadModel>().Where(x => x.DateTime == dateTime).SingleOrDefault();
        }

        public IList<object> GetOhlcByCurrencyPair(string currencyPair)
        {
            return
                CurrentSession.QueryOver<OhlcReadModel>()
                    .Select(x => x.Open, x => x.High, x => x.Low, x => x.Close, x => x.DateTime, x => x.Volume,
                        x => x.DateTime).Where(x=>x.CurrencyPair==currencyPair)
                    .List<object>();
        }
    }
}
