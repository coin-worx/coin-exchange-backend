using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="userValidationEssentials"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="confirmNewPassword"></param>
        public ChangePasswordCommand(UserValidationEssentials userValidationEssentials, string oldPassword, string newPassword, string confirmNewPassword)
        {
            UserValidationEssentials = userValidationEssentials;
            OldPassword = oldPassword;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }

        /// <summary>
        /// User Validation Credentials
        /// </summary>
        public UserValidationEssentials UserValidationEssentials { get; private set; }

        /// <summary>
        /// Old Password
        /// </summary>
        public string OldPassword { get; private set; }

        /// <summary>
        /// New Password
        /// </summary>
        public string NewPassword { get; private set; }

        /// <summary>
        /// Confirmation of new password
        /// </summary>
        public string ConfirmNewPassword { get; private set; }
    }
}
