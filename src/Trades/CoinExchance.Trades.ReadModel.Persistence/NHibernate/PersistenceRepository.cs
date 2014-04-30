using System;
using CoinExchange.Trades.ReadModel.Repositories;
using Spring.Stereotype;
using Spring.Transaction;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class PersistenceRepository : NHibernateSessionFactory,IPersistanceRepository
    {
        [Transaction(ReadOnly = false)]
        public void SaveOrUpdate(object readModelObject)
        {
            CurrentSession.SaveOrUpdate(readModelObject);
        }
    }
}
