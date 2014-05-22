using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Registration
{
    /// <summary>
    /// Registration Application Service
    /// </summary>
    public class RegistrationApplicationService : IRegistrationApplicationService
    {
        private IPersistenceRepository _persistenceRepository;
        private IPasswordEncryptionService _passwordEncryptionService;
        private IActivationKeyGenerationService _activationKeyGenerationService;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public RegistrationApplicationService(IPersistenceRepository persistenceRepository, IPasswordEncryptionService passwordEncryptionService, IActivationKeyGenerationService activationKeyGenerationService)
        {
            _persistenceRepository = persistenceRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _activationKeyGenerationService = activationKeyGenerationService;
        }

        /// <summary>
        /// Request from the client to create a new account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public string CreateAccount(string email, string username, string password, string country, TimeZone timeZone, 
            string publicKey)
        {
            // Hash the Password
            string hashedPassword = _passwordEncryptionService.EncryptPassword(password);
            // Generate new activation key
            string activationKey = _activationKeyGenerationService.GenerateNewActivationKey();
            // Create new user
            User user = new User(email, username, hashedPassword, country, timeZone, publicKey, activationKey);
            // Save to persistence
            _persistenceRepository.SaveUpdate(user);
            // return Activation Key
            return activationKey;
        }
    }
}
