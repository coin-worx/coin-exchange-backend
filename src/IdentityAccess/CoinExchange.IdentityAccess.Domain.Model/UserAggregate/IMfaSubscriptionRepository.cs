using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Mfa Subscription Repository
    /// </summary>
    public interface IMfaSubscriptionRepository
    {
        /// <summary>
        /// Gets a list of all the subscriptions present in the database
        /// </summary>
        /// <returns></returns>
        IList<MfaSubscription> GetAllSubscriptions();
    }
}
