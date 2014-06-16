using System;
using System.Management.Instrumentation;
using System.Security.Authentication;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices
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
        private ISecurityKeysApplicationService _securityKeysApplicationService;
        private IIdentityAccessPersistenceRepository _persistenceRepository;

        /// <summary>
        /// Initializes with the UserRepository and PasswordEncryption service 
        /// </summary>
        public LoginApplicationService(IUserRepository userRepository, IPasswordEncryptionService passwordEncryptionService,
        ISecurityKeysApplicationService securityKeysApplicationService, IIdentityAccessPersistenceRepository persistenceRepository)
        {
            _userRepository = userRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _securityKeysApplicationService = securityKeysApplicationService;
            _persistenceRepository = persistenceRepository;
        }

        /// <summary>
        /// Login call by the user, logs user in if username and password are correct
        /// </summary>
        /// <returns></returns>
        public UserValidationEssentials Login(LoginCommand loginCommand)
        {
            if (loginCommand != null && 
                !string.IsNullOrEmpty(loginCommand.Username) &&
                !string.IsNullOrEmpty(loginCommand.Password))
            {
                User user = _userRepository.GetUserByUserName(loginCommand.Username);
                if (user != null)
                {
                    if (!user.IsActivationKeyUsed.Value)
                    {
                        throw new InvalidOperationException(
                            "This account is not yet activated. Please activate the account before trying to login.");
                    }
                    if (_passwordEncryptionService.VerifyPassword(loginCommand.Password, user.Password))
                    {
                        Tuple<ApiKey, SecretKey,DateTime> securityKeys =
                            _securityKeysApplicationService.CreateSystemGeneratedKey(user.Id);
                        user.LastLogin = DateTime.Now;
                        _persistenceRepository.SaveUpdate(user);
                        return new UserValidationEssentials(securityKeys, user.AutoLogout);
                    }
                    else
                    {
                        throw new InvalidCredentialException(string.Format("Incorrect password for username: {0}",
                                                                           loginCommand.Username));
                    }
                }
                else
                {
                    throw new InvalidCredentialException(string.Format("Invalid username: {0}", loginCommand.Username));
                }
            }
            else
            {
                throw new InvalidCredentialException("Invalid Username and/or Password.");
            }
        }
    }
}
