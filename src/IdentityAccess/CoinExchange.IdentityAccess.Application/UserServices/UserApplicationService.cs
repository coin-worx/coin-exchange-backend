using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
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
        private IPasswordCodeGenerationService _passwordCodeGenerationService = null;

        /// <summary>
        /// Initializes with the User Repository
        /// </summary>
        public UserApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository,
            IPasswordEncryptionService passwordEncryptionService, IIdentityAccessPersistenceRepository persistenceRepository,
            IEmailService emailService, IPasswordCodeGenerationService passwordCodeGenerationService)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _persistenceRepository = persistenceRepository;
            _emailService = emailService;
            _passwordCodeGenerationService = passwordCodeGenerationService;
        }

        /// <summary>
        /// Request to change Password
        /// </summary>
        /// <param name="changePasswordCommand"> </param>
        /// <returns></returns>
        public bool ChangePassword(ChangePasswordCommand changePasswordCommand)
        {
            // Get the SecurityKeyspair instance related to this API Key
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(changePasswordCommand.UserValidationEssentials.ApiKey.Value);

            // See if the SecurityKeysPair instance exists for this API Key
            if (securityKeysPair != null)
            {
                // Get the User by specifying the Username in the SecurityKeysPair instance
                User user = _userRepository.GetUserByUserName(securityKeysPair.UserName);
                if (user != null)
                {
                    // The Session Logout time and the given session logout time must be the same
                    if (user.AutoLogout == changePasswordCommand.UserValidationEssentials.SessionLogoutTime)
                    {
                        // Make sure the session has not expired
                        if (securityKeysPair.CreationDateTime.Add(user.AutoLogout) > DateTime.Now)
                        {
                            // Check if the old password is the same as new one
                            if (_passwordEncryptionService.VerifyPassword(changePasswordCommand.OldPassword,
                                                                          user.Password))
                            {
                                // Create new password and save for the user in the database
                                string newEncryptedPassword =
                                    _passwordEncryptionService.EncryptPassword(changePasswordCommand.NewPassword);
                                user.Password = newEncryptedPassword;
                                _persistenceRepository.SaveUpdate(user);
                                _emailService.SendPasswordChangedEmail(user.Email, user.Username);
                                return true;
                            }
                            else
                            {
                                throw new InvalidCredentialException(string.Format("Current password incorrect."));
                            }
                        }
                        else
                        {
                            _securityKeysRepository.DeleteSecurityKeysPair(securityKeysPair);
                            throw new Exception("Session Timeout expired for this API Key.");
                        }
                    }
                    else
                    {
                        throw new Exception("User's Session Auto Logout and given Session Losout do not match. Operation is aborted");
                    }
                }
                else
                {
                    throw new InstanceNotFoundException("User not found for the given SecurityKeysPair's Username.");
                }
            }
            else
            {
                throw new InstanceNotFoundException("SecurityKeysPair not found for the given API Key");
            }
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
                        // Activate the user only it either the activationKeyUser VO is null, or if the activation key has not been used
                        if (user.IsActivationKeyUsed == null || (user.IsActivationKeyUsed != null && !user.IsActivationKeyUsed.Value))
                        {
                            // Mark that this user is now activated
                            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
                            user.IsUserBlocked = new IsUserBlocked(false);
                            //update user's tier0 status to verified
                            user.UpdateTierStatus(TierLevelConstant.Tier0, Status.Verified);

                            // Update the user instance in repository
                            _persistenceRepository.SaveUpdate(user);
                            _emailService.SendWelcomeEmail(user.Email, user.Username);
                            return true;
                        }
                        else
                        {
                            // ToDo: Send email to user that someone tried to re-activated their account.
                            throw new Exception("The activation key has already been used ");
                        }
                    }
                    else
                    {
                        throw new InvalidCredentialException("Invalid Username or Password.");
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
                User user = _userRepository.GetUserByActivationKey(activationKey);
                // ToDo: Email
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    _userRepository.DeleteUser(user);
                    return true;
                }
                throw new InvalidOperationException(string.Format("{0} {1}", "No user exists against activation key: ", activationKey));
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Activation Key not provided");
            }
        }

        /// <summary>
        /// Request for providing the Username by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string ForgotUsername(string email)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(email))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException("Account is not activated yet. In case you have forgotten your " +
                                                            "username, you can cancel your account activation" +
                                                            " and then sing up for an account again.");
                    }
                    return user.Username;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}",
                                                                      "No user exists against email address: ", email));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
        }

        /// <summary>
        /// Request to reset the password in case it is forgotten by the user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string ForgotPassword(string email, string username)
        {
            // Make sure all given credential contains value
            if (!string.IsNullOrEmpty(email) && (!string.IsNullOrEmpty(username)))
            {
                // Get the user tied to this Activation Key
                User user = _userRepository.GetUserByEmail(email);
                // If activation key is valid, proceed to verify username and password
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException("Account is not activated yet. In case you have forgotten your " +
                                                            "password, you can cancel your account activation" +
                                                            " and then sing up for an account again.");
                    }
                    if (user.Username.Equals(username))
                    {
                        string newForgotPasswordCode = _passwordCodeGenerationService.CreateNewForgotPasswordCode();
                        user.AddForgotPasswordCode(newForgotPasswordCode);
                        _persistenceRepository.SaveUpdate(user);
                        return newForgotPasswordCode;
                    }
                    else
                    {
                        throw new InvalidCredentialException("Wrong username.");
                    }
                }
                throw new InvalidOperationException(string.Format("{0} {1}", "No user could be found for Email: ", email));
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
        }

        /// <summary>
        /// Checks if this is a valid reset link code sent to the user for reseting password and also to verify new 
        /// password matches Confirm Password
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ResetPasswordByEmailLink(string username, string newPassword)
        {
            // Make sure given credential contains value
            if ((!string.IsNullOrEmpty(newPassword)))
            {
                // Get the user tied to this username
                User user = _userRepository.GetUserByUserName(username);

                if (user != null)
                {
                    // Check if the password code is the same as expected and the validity period for this user's last 
                    // forgot password code validation has not expired
                    if (user.IsPasswordCodeValid())
                    {
                        // If code validation has not expired, update the password
                        user.Password = _passwordEncryptionService.EncryptPassword(newPassword);
                        _persistenceRepository.SaveUpdate(user);
                        return true;
                    }
                    else
                    {
                        // else, throw an exception
                        throw new InvalidOperationException(string.Format("{0} {1} {2}",
                                                                          "Validity Period to chnge password for user ",
                                                                          user.Username, " has expired."));
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} {1}",
                                                                      "No user found for the given password code"));
                }
            }
            // If the user did not provide all the credentials, return with failure
            else
            {
                throw new InvalidCredentialException("Email not provided");
            }
        }
    }
}
