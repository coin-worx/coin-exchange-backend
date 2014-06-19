using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Commands to request to reset password
    /// </summary>
    public class ForgotPasswordCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordCommand"/> class.
        /// </summary>
        public ForgotPasswordCommand(string email, string username)
        {
            Email = email;
            Username = username;
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }
    }
}
