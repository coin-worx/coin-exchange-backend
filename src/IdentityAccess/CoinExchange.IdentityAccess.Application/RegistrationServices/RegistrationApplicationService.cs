using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Authentication;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;

namespace CoinExchange.IdentityAccess.Application.RegistrationServices
{
    /// <summary>
    /// Registration Application Service
    /// </summary>
    public class RegistrationApplicationService : IRegistrationApplicationService
    {
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPasswordEncryptionService _passwordEncryptionService;
        private IActivationKeyGenerationService _activationKeyGenerationService;
        private IEmailService _emailService;
        private ITierRepository _tierRepository;
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public RegistrationApplicationService(IIdentityAccessPersistenceRepository persistenceRepository, 
        IPasswordEncryptionService passwordEncryptionService, IActivationKeyGenerationService activationKeyGenerationService
            , IEmailService emailService,ITierRepository tierRepository)
        {
            _persistenceRepository = persistenceRepository;
            _passwordEncryptionService = passwordEncryptionService;
            _activationKeyGenerationService = activationKeyGenerationService;
            _emailService = emailService;
            _tierRepository = tierRepository;
        }

        /// <summary>
        /// Request from the client to create a new account
        /// </summary>
        /// <param name="signupUserCommand"> </param>
        /// <returns></returns>
        public string CreateAccount(SignupUserCommand signupUserCommand)
        {
            // Check the given credential strings
            if (!string.IsNullOrEmpty(signupUserCommand.Email) &&
                !string.IsNullOrEmpty(signupUserCommand.Username) && 
                !string.IsNullOrEmpty(signupUserCommand.Password))
            {
                // Hash the Password
                string hashedPassword = _passwordEncryptionService.EncryptPassword(signupUserCommand.Password);
                // Generate new activation key
                string activationKey = _activationKeyGenerationService.GenerateNewActivationKey();
                if (!string.IsNullOrEmpty(activationKey))
                {
                    // Create new user
                    User user = new User(signupUserCommand.Email, signupUserCommand.Username, hashedPassword,
                                         signupUserCommand.Country, signupUserCommand.TimeZone,
                                         signupUserCommand.PgpPublicKey, activationKey);
                    IList<Tier> tiers = _tierRepository.GetAllTierLevels();
                    for (int i = 0; i < tiers.Count; i++)
                    {
                        user.AddTierStatus(Status.NonVerified, tiers[i]);
                    }
                    try
                    {
                        // Save to persistence
                        _persistenceRepository.SaveUpdate(user);
                        // return Activation Key
                        return activationKey;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                   throw new DataException("Not able to generate an activation key for New User request");
                }
            }
            else
            {
                throw new InvalidCredentialException("Email, username and/or Password not provided for reister new user request.");
            }
        }
    }
}
