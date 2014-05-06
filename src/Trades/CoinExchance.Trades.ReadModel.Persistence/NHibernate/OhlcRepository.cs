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
    }
}
