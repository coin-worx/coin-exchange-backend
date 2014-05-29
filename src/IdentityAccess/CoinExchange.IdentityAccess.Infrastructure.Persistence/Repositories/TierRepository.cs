using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of Tier repository
    /// </summary>
    [Repository]
    public class TierRepository:NHibernateSessionFactory,ITierRepository
    {
        /// <summary>
        /// Get list of all tiers.
        /// </summary>
        /// <returns></returns>
        [Transaction(ReadOnly = true)]
        public IList<Tier> GetAllTierLevels()
        {
            return CurrentSession.QueryOver<Tier>().List<Tier>();
        }
    }
}
