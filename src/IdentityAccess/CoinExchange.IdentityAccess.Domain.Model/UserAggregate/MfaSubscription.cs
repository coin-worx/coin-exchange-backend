using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Represents one Mfa subscription
    /// </summary>
    public class MfaSubscription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscription(string mfaSubscriptionName)
        {
            MfaSubscriptionName = mfaSubscriptionName;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string MfaSubscriptionName { get; private set; }
    }
}
