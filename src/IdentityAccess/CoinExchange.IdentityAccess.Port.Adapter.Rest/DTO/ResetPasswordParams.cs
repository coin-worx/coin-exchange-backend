using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Contains parameters for resetting password
    /// </summary>
    public class ResetPasswordParams
    {
        public ResetPasswordParams()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordParams"/> class.
        /// </summary>
        public ResetPasswordParams(string username, string password, string resetPasswordCode)
        {
            Username = username;
            Password = password;
            ResetPasswordCode = resetPasswordCode;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Reset code
        /// </summary>
        public string ResetPasswordCode { get; set; }
    }
}
