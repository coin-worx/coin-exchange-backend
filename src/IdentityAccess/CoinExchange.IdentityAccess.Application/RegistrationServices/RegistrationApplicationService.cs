using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.RegistrationServices
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
        /// <param name="signupUserCommand"> </param>
        /// <returns></returns>
        public string CreateAccount(SignupUserCommand signupUserCommand)
        {
            // Hash the Password
            string hashedPassword = _passwordEncryptionService.EncryptPassword(signupUserCommand.Password);
            // Generate new activation key
            string activationKey = _activationKeyGenerationService.GenerateNewActivationKey();
            // Create new user
            User user = new User(signupUserCommand.Email, signupUserCommand.Username, hashedPassword,
                signupUserCommand.Country, signupUserCommand.TimeZone, signupUserCommand.PgpPublicKey, activationKey);
            // Save to persistence
            _persistenceRepository.SaveUpdate(user);
            // return Activation Key
            return activationKey;
        }
    }
}
