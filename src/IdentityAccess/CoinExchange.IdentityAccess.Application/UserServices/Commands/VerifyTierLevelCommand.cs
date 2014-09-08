using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command to verify a Tier Level for a User
    /// </summary>
    public class VerifyTierLevelCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public VerifyTierLevelCommand(string apiKey, string tierLevel)
        {
            ApiKey = apiKey;
            TierLevel = tierLevel;
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Tier Level
        /// </summary>
        public string TierLevel { get; private set; }
    }
}
