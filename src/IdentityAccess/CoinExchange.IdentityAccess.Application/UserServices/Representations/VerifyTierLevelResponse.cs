using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Response to verifying the Tier level
    /// </summary>
    public class VerifyTierLevelResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public VerifyTierLevelResponse(bool verificationSuccessful, string description)
        {
            VerificationSuccessful = verificationSuccessful;
            Description = description;
        }

        /// <summary>
        /// Verfiication successful
        /// </summary>
        public bool VerificationSuccessful { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
