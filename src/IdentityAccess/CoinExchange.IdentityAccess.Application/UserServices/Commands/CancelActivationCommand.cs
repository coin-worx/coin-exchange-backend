using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command to request the cancellation of an account activation
    /// </summary>
    public class CancelActivationCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CancelActivationCommand(string activationKey)
        {
            ActivationKey = activationKey;
        }

        /// <summary>
        /// Activation Key
        /// </summary>
        public string ActivationKey { get; private set; }
    }
}
