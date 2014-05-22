using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Registration
{
    /// <summary>
    /// Registration Application Service
    /// </summary>
    public class RegistrationApplicationService : IRegistrationApplicationService
    {
        private IPasswordEncryptionService _passwordEncryptionService;
        private IActivationKeyGenerationService _activationKeyGenerationService;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public RegistrationApplicationService(IPasswordEncryptionService passwordEncryptionService, IActivationKeyGenerationService activationKeyGenerationService)
        {
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
        public bool CreateAccount(string email, string username, string password, string country, TimeZone timeZone, 
            string publicKey)
        {
            User user = new User(email, username, password, country, timeZone, publicKey);
            // ToDo: Send to the repository
            return false;
        }
    }
}
