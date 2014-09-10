using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Repositories;
using Spring.Transaction;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Saves Funds BC objects to persistence using NHibernate
    /// </summary>
    public class FundsPersistenceRepository : NHibernateSessionFactory, IFundsPersistenceRepository
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Transaction(TransactionPropagation.Required, ReadOnly = false)]
        public void SaveOrUpdate(object domainObject)
        {
            try
            {
                CurrentSession.SaveOrUpdate(domainObject);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
            }
        }

        [Transaction(TransactionPropagation.Required, ReadOnly = false)]
        public void Delete(object domainObject)
        {
            try
            {
                CurrentSession.Delete(domainObject);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
            }
        }
    }
}
