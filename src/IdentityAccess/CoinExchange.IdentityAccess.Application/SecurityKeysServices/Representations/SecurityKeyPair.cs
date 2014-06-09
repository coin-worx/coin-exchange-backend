using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations
{
    /// <summary>
    /// /VO to represent key pair.
    /// </summary>
    public class SecurityKeyPair
    {
        public string ApiKey { get; private set; }
        public string SecretKey { get; private set; }

        public SecurityKeyPair(string apiKey, string secretKey)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
        }
    }
}
