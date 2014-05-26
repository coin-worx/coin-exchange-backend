using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Persistence repository for storing objects
    /// </summary>
    [Repository]
    public class PersistRepository : NHibernateSessionFactory, IPersistRepository
    {
        [Transaction(ReadOnly = false)]
        public void SaveUpdate(object entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }
    }
}
