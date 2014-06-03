using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Contains Forgot Username parameters
    /// </summary>
    public class ForgotUsernameParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotUsernameParams"/> class.
        /// </summary>
        public ForgotUsernameParams(string email)
        {
            Email = email;
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }
    }
}
