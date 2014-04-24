using System;
using CoinExchange.Trades.ReadModel.Services;
using Spring.Stereotype;
using Spring.Transaction;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class Persist : NHibernateSessionFactory,IPersistance
    {
        [Transaction(TransactionPropagation.Required,ReadOnly = false)]
        public void SaveOrUpdate(object readModelObject)
        {
           // CurrentSession.SaveOrUpdate(readModelObject);
            CurrentSession.Save(readModelObject);
            CurrentSession.Flush();
            
        }
    }
}
