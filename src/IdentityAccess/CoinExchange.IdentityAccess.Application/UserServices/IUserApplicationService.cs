using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
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
        /// <param name="activationCommand"> </param>
        /// <returns></returns>
        bool ActivateAccount(ActivationCommand activationCommand);

        /// <summary>
        /// Request to Cancel the Account Activation after the user has signed up for an account but has not activated the account
        /// </summary>
        /// <param name="cancelActivationCommand"> </param>
        /// <returns></returns>
        bool CancelAccountActivation(CancelActivationCommand cancelActivationCommand);

        /// <summary>
        /// Request to remind the user of their username. Returns the username to the caller
        /// </summary>
        /// <returns></returns>
        string ForgotUsername(ForgotUsernameCommand forgotUsernameCommand);

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="forgotPasswordCommand"> </param>
        /// <returns></returns>
        string ForgotPassword(ForgotPasswordCommand forgotPasswordCommand);

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="resetPasswordCommand"> </param>
        /// <returns></returns>
        bool ResetPasswordByEmailLink(ResetPasswordCommand resetPasswordCommand);

        /// <summary>
        /// Change Settings for the account
        /// </summary>
        /// <param name="changeSettingsCommand"></param>
        /// <returns></returns>
        bool ChangeSettings(ChangeSettingsCommand changeSettingsCommand);

        /// <summary>
        /// Get settings for an account
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        AccountSettingsRepresentation GetAccountSettings(string apiKey);
    }
}
