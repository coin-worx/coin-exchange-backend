using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for retreiving the MFA Subscription
    /// </summary>
    [Repository]
    public class MfaSubscriptionRepository : NHibernateSessionFactory, IMfaSubscriptionRepository
    {
        [Transaction(ReadOnly = true)]
        public IList<MfaSubscription> GetAllSubscriptions()
        {
            return CurrentSession.QueryOver<MfaSubscription>().List<MfaSubscription>();
        }
    }
}
