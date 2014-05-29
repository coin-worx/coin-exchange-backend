using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command to request to remind Username
    /// </summary>
    public class ForgotUsernameCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ForgotUsernameCommand(string username)
        {
            Username = username;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }
    }
}
