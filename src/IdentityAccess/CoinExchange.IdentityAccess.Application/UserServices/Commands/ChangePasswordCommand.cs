using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Change Password Command
    /// </summary>
    public class ChangePasswordCommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey"> </param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public ChangePasswordCommand(string apiKey, string oldPassword, string newPassword)
        {
            AssertionConcern.AssertNullOrEmptyString(newPassword,"New password cannot be empty");
            ApiKey = new ApiKey(apiKey);
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public ApiKey ApiKey { get; private set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public SecretKey SecretKey { get; private set; }

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
