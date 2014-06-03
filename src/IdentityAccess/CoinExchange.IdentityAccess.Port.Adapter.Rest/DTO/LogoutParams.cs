using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Contains the parameters for requesting logout
    /// </summary>
    public class LogoutParams
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="apiKey"> </param>
        /// <param name="secretKey"> </param>
        public LogoutParams(string apiKey, string secretKey)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; private set; }
    }
}
