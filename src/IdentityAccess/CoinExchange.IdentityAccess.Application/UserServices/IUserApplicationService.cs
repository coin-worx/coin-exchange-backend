using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
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
        /// <param name="changePasswordCommand"> </param>
        /// <returns></returns>
        bool ChangePassword(ChangePasswordCommand changePasswordCommand);

        /// <summary>
        /// Request to activate account for which the user has already signed up
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ActivateAccount(string activationKey, string username, string password);

        /// <summary>
        /// Request to Cancel the Account Activation after the user has signed up for an account but has not activated the account
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        bool CancelAccountActivation(string activationKey);

        /// <summary>
        /// Request to remind the user of their username. Returns the username to the caller
        /// </summary>
        /// <returns></returns>
        string ForgotUsername(string email);

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        string ForgotPassword(string email, string username);

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="forgotPasswordActivationCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ResetPasswordByEmailLink(string forgotPasswordActivationCode, string password);
    }
}
