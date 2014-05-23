using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControl
{
    /// <summary>
    /// Serves the login operation(s)
    /// </summary>
    public class LoginApplicationService : ILoginApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUserRepository _userRepository;
        private IPasswordEncryptionService _passwordEncryptionService;
        private ISecurityKeysGenerationService _securityKeysGenerationService;
        /// <summary>
        /// Initializes with the UserRepository and PasswordEncryption service 
        /// </summary>
        public LoginApplicationService(IUserRepository userRepository, IPasswordEncryptionService passwordEncryptionService,
            ISecurityKeysGenerationService securityKeysGenerationService)
        {
            _userRepository = userRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _securityKeysGenerationService = securityKeysGenerationService;
        }

        /// <summary>
        /// Login call by the user, logs user in if username and password are correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ValidationEssentials Login(string username, string password)
        {
            User user = _userRepository.GetUserByUserName(username);
            if (user != null)
            {
                if(_passwordEncryptionService.VerifyPassword(password, user.Password))
                {
                    return new ValidationEssentials(_securityKeysGenerationService.GenerateNewSecurityKeys(),
                        user.AutoLogout);
                }
                else
                {
                    throw new Exception(string.Format("Password incorrect for username: {0}", username));
                }
            }
            else
            {
                throw new InstanceNotFoundException(string.Format("No user could be found for the username: {0}", username));
            }
            return null;
        }
    }
}
