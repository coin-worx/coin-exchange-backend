using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command to Reset Password
    /// </summary>
    public class ResetPasswordCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordCommand"/> class.
        /// </summary>
        public ResetPasswordCommand(string username, string password,string resetPasswordCode)
        {
            Username = username;
            Password = password;
            ResetPasswordCode = resetPasswordCode;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Reset password code.
        /// </summary>
        public string ResetPasswordCode { get; private set; }
    }
}
