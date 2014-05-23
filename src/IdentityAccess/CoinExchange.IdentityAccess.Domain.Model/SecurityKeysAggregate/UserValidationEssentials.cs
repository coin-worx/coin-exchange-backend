using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// The pair of the API and Secret Key along with the Session logout item mentioned by the user themselves, returned when
    /// the user logs in for a session
    /// </summary>
    public class UserValidationEssentials
    {
        /// <summary>
        /// Parametrized Constructor
        /// </summary>
        /// <param name="digitalSignature"></param>
        /// <param name="sessionLogoutTime"></param>
        public UserValidationEssentials(SecurityKeysPair digitalSignature, TimeSpan sessionLogoutTime)
        {
            SecurityKeys = digitalSignature;
            SessionLogoutTime = sessionLogoutTime;
        }

        /// <summary>
        /// The Pair of API and Secret Key
        /// </summary>
        public SecurityKeysPair SecurityKeys { get; private set; }

        /// <summary>
        /// Logout time mentioned by the user for which these keys are applicable after login
        /// </summary>
        public TimeSpan SessionLogoutTime { get; private set; }
    }
}
