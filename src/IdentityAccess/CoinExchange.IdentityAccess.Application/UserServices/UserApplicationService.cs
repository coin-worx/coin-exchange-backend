using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

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

        /// <summary>
        /// Initializes with the User Repository
        /// </summary>
        public UserApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository,
            IPasswordEncryptionService passwordEncryptionService, IIdentityAccessPersistenceRepository persistenceRepository)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _persistenceRepository = persistenceRepository;
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
                Log.Error(string.Format("New Password and confirmation password do not match."));
            }
            Log.Error(string.Format("Current paassword incorrect."));
            return false;
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
            throw new NotImplementedException();
        }

        public bool CancelAccountActivation(string activationKey)
        {
            throw new NotImplementedException();
        }

        public bool ForgotUsername(string email)
        {
            throw new NotImplementedException();
        }

        public bool ForgotPassword(string email, string username)
        {
            throw new NotImplementedException();
        }
    }
}
