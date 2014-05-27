using System;
using System.Globalization;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices
{
    /// <summary>
    /// Serves operations related to the Digital Signature(API Secret Key pair)
    /// </summary>
    public class SecurityKeysApplicationService : ISecurityKeysApplicationService
    {
        private ISecurityKeysGenerationService _securityKeysGenerationService;
        private IIdentityAccessPersistenceRepository _persistRepository;
        private int _keyDescriptionCounter = 0;

        /// <summary>
        /// Initializes the service for operating operations for the DigitalSignatures
        /// </summary>
        public SecurityKeysApplicationService(ISecurityKeysGenerationService securityKeysGenerationService, 
            IIdentityAccessPersistenceRepository persistenceRepository)
        {
            _securityKeysGenerationService = securityKeysGenerationService;
            _persistRepository = persistenceRepository;
        }

        /// <summary>
        /// Generates a new API key and Secret Key pair
        /// </summary>
        /// <returns></returns>
        public Tuple<ApiKey, SecretKey> CreateSystemGeneratedKey(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
//Tuple<ApiKey, SecretKey> securityKeys = _securityKeysGenerationService.GenerateNewSecurityKeys();
                //SecurityKeysPair securityKeysPair = new SecurityKeysPair(username, securityKeys.Item1,
                                //securityKeys.Item2, true, (++_keyDescriptionCounter).ToString(CultureInfo.InvariantCulture));
                //_persistRepository.SaveUpdate(securityKeysPair);
                return null;
            }
            return null;
        }

        public bool CreateUserGeneratedKey()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the permissions
        /// </summary>
        public void SetPermissions(SecurityKeysPermission[] securityKeysPermissions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate the given API Key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public bool ApiKeyValidation(string apiKey)
        {
            // ToDo: Get the API Key from the database
            throw new NotImplementedException();
        }
    }
}
