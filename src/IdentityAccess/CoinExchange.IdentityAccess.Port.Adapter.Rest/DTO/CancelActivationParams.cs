using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters for cancelling an activation
    /// </summary>
    public class CancelActivationParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelActivationParams"/> class.
        /// </summary>
        public CancelActivationParams(string activationKey)
        {
            ActivationKey = activationKey;
        }

        /// <summary>
        /// Activation Key
        /// </summary>
        public string ActivationKey { get; private set; }
    }
}
