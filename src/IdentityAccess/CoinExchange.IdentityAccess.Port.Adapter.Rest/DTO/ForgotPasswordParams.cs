using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Cotains parameters for Forgot Password Request
    /// </summary>
    public class ForgotPasswordParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordParams"/> class.
        /// </summary>
        public ForgotPasswordParams(string email, string username)
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
