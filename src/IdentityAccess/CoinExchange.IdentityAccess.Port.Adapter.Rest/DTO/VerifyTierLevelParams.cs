using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters for verifying the Tier level
    /// </summary>
    public class VerifyTierLevelParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public VerifyTierLevelParams(string tierLevel, string apiKey)
        {
            TierLevel = tierLevel;
            ApiKey = apiKey;
        }

        /// <summary>
        /// Tier Level
        /// </summary>
        public string TierLevel { get; private set; }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; private set; }
    }
}
