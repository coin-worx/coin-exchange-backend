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
        [Transaction(TransactionPropagation.Required,ReadOnly = false)]
        public void SaveOrUpdate(object readModelObject)
        {
            try
            {
                CurrentSession.SaveOrUpdate(readModelObject);
            }
            catch (Exception exception)
            {

            }
        }
    }
}
