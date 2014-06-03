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
        public ChangePasswordParams(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

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
