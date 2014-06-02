using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters for changing password
    /// </summary>
    public class ChangePasswordParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordParams"/> class.
        /// </summary>
        public ChangePasswordParams(string apiKey, string secretKey, string oldPassword, string newPassword)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Old Password
        /// </summary>
        public string OldPassword { get; private set; }

        /// <summary>
        /// New Password
        /// </summary>
        public string NewPassword { get; private set; }
    }
}
