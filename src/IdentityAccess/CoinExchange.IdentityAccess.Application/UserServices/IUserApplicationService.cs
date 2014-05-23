using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Interface for operations related to user and modifications in the user account
    /// </summary>
    public interface IUserApplicationService
    {
        /// <summary>
        /// Requests the change of password
        /// </summary>
        /// <param name="userValidationEssentials"> </param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordConfirmation"></param>
        /// <returns></returns>
        bool ChangePassword(UserValidationEssentials userValidationEssentials, string oldPassword, string newPassword, string newPasswordConfirmation);

        /// <summary>
        /// Request to activate account for which the user has already signed up
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ActivateAccount(string activationKey, string username, string password);

        bool CancelAccountActivation(string activationKey);
    }
}
