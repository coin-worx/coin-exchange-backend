using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Performs operations related to the User and User Account
    /// </summary>
    public class UserApplicationService : IUserApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUserRepository _userRepository = null;
        private ISecurityKeysRepository _securityKeysRepository = null;
        private IPasswordEncryptionService _passwordEncryptionService = null;
        private IIdentityAccessPersistenceRepository _persistenceRepository = null;
        private IEmailService _emailService = null;

        /// <summary>
        /// Initializes with the User Repository
        /// </summary>
        public UserApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository,
            IPasswordEncryptionService passwordEncryptionService, IIdentityAccessPersistenceRepository persistenceRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _persistenceRepository = persistenceRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Request to change Password
        /// </summary>
        /// <param name="userValidationEssentials"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordConfirmation"></param>
        /// <returns></returns>
        public bool ChangePassword(UserValidationEssentials userValidationEssentials, string oldPassword, string newPassword, string newPasswordConfirmation)
        {
            // Get the SecurityKeyspair instance related to this API Key
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey.Value);
            // Get the Userby specifying the Username in the SecurityKeysPair instance
            User user = _userRepository.GetUserByUserName(securityKeysPair.UserName);

            if (_passwordEncryptionService.VerifyPassword(oldPassword, user.Password))
            {
                if (newPassword.Equals(newPasswordConfirmation))
                {
                    string newEncryptedPassword = _passwordEncryptionService.EncryptPassword(newPassword);
                    user.Password = newEncryptedPassword;
                    _persistenceRepository.SaveUpdate(user);
                    return true;
                }
                throw new InvalidCredentialException(string.Format("New Password and confirmation password do not match."));
            }
            throw new InvalidCredentialException(string.Format("Current password incorrect."));
        }

        /// <summary>
        /// Activates Account
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ActivateAccount(string activationKey, string username, string password)
        {
            // Make sure all given credentials contain values
            if (!string.IsNullOrEmpty(activationKey) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByActivationKey(activationKey);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (username == user.Username &&
                        _passwordEncryptionService.VerifyPassword(password, user.Password))
                    {
                        // Mark that this user is now activated
                        user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
                        user.IsUserBlocked = new IsUserBlocked(false);
                        // Update the user instance in repository
                        _persistenceRepository.SaveUpdate(user);
                        return true;
                    }
                }
                else
                {
                    throw new InstanceNotFoundException("No user instance found for the given activation key.");
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Activation Key, Username and/or Password not provided");
            }
            return false;
        }

        /// <summary>
        /// Cancel the account activation for this user
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public bool CancelAccountActivation(string activationKey)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(activationKey))
            {
                // Get the user tied to this Activation Key
                User userByActivationKey = _userRepository.GetUserByActivationKey(activationKey);
                // If activation key is valid, proceed to verify username and password
                if (userByActivationKey != null)
                {
                    _userRepository.DeleteUser(userByActivationKey);
                    return true;
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Activation Key not provided");
            }
            return false;
        }

        /// <summary>
        /// Request for providing the Username by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ForgotUsername(string email)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(email))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    _emailService.SendForgotUsernameEmail(email, user.Username);
                    return true;
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
            return false;
        }

        /// <summary>
        /// Request to reset the password in case it is forgotten by the user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ForgotPassword(string email, string username)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(email) && (!string.IsNullOrEmpty(username)))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (user.Username.Equals(username))
                    {
                        // ToDo: Discuss if the link to the reset password site will be sent by the front end and we will
                        // append the forgotpassword id to it and send in the email to the user who requested it.
                        // If yes, create new service for forgotpassword code generation.
                        //_emailService.SendForgotPasswordEmail(email, )
                        return true;
                    }
                    else
                    {
                        throw new InvalidCredentialException("Wrong username.");
                    }
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
            return false;
        }

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="forgotPasswordActivationCode"></param>
        /// <param name="newPassword"></param>
        /// <param name="confirmNewPassword"></param>
        /// <returns></returns>
        public bool ResetPasswordByEmailLink(string forgotPasswordActivationCode, string newPassword, string confirmNewPassword)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(forgotPasswordActivationCode) && 
                (!string.IsNullOrEmpty(newPassword)) &&
                (!string.IsNullOrEmpty(confirmNewPassword)))
            {
                // Get the user tied to this ForgotPasswordCode
                // ToDo: Ask Bilal to provide a method for getting UserByForgotPasswordCode, Confirm user exists,
                // Check if new andconfirm passwords match, and update the password for the user in the repository
                
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
            return false;
        }
    }
}
