using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command to activate an account
    /// </summary>
    public class ActivationCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCommand"/> class.
        /// </summary>
        public ActivationCommand(string activationKey, string username, string password)
        {
            ActivationKey = activationKey;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Activation Key
        /// </summary>
        public string ActivationKey { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; private set; }
    }
}
