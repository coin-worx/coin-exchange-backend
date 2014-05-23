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
    public class ValidationEssentials
    {
        /// <summary>
        /// Parametrized Constructor
        /// </summary>
        /// <param name="securityKeys"></param>
        /// <param name="sessionLogoutTime"></param>
        public ValidationEssentials(DigitalSignature securityKeys, TimeSpan sessionLogoutTime)
        {
            SecurityKeys = securityKeys;
            SessionLogoutTime = sessionLogoutTime;
        }

        /// <summary>
        /// The Pair of API and Secret Key
        /// </summary>
        public DigitalSignature SecurityKeys { get; private set; }

        /// <summary>
        /// Logout time mentioned by the user for which these keys are applicable after login
        /// </summary>
        public TimeSpan SessionLogoutTime { get; private set; }
    }
}
