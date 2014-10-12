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
        /// <param name="securityKeys"> </param>
        /// <param name="sessionLogoutTime"></param>
        public UserValidationEssentials(Tuple<ApiKey, SecretKey,DateTime> securityKeys, TimeSpan sessionLogoutTime)
        {
            ApiKey = securityKeys.Item1.Value;
            SecretKey = securityKeys.Item2.Value;
            SessionLogoutTime = sessionLogoutTime;
            LastLogin = securityKeys.Item3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserValidationEssentials(bool loginSuccessful, string message, string apiKey, string secretKey, TimeSpan sessionLogoutTime, DateTime lastLogin)
        {
            LoginSuccessful = loginSuccessful;
            Message = message;
            ApiKey = apiKey;
            SecretKey = secretKey;
            SessionLogoutTime = sessionLogoutTime;
            LastLogin = lastLogin;
        }

        /// <summary>
        /// Is the login successful
        /// </summary>
        public bool LoginSuccessful { get; private set; }

        /// <summary>
        /// Response message after login attempt
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; private set; }

        /// <summary>
        /// Logout time mentioned by the user for which these keys are applicable after login
        /// </summary>
        public TimeSpan SessionLogoutTime { get; private set; }

        /// <summary>
        /// The time of last login
        /// </summary>
        public DateTime LastLogin { get; private set; }
    }
}
