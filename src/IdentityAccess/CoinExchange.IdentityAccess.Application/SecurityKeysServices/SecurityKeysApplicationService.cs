using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
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
        private ISecurityKeysRepository _securityKeysRepository;
        private int _keyDescriptionCounter = 0;

        /// <summary>
        /// Initializes the service for operating operations for the DigitalSignatures
        /// </summary>
        public SecurityKeysApplicationService(ISecurityKeysGenerationService securityKeysGenerationService,
            IIdentityAccessPersistenceRepository persistenceRepository, ISecurityKeysRepository securityKeysRepository)
        {
            _securityKeysGenerationService = securityKeysGenerationService;
            _persistRepository = persistenceRepository;
            _securityKeysRepository = securityKeysRepository;
        }

        /// <summary>
        /// Generates a new API key and Secret Key pair
        /// </summary>
        /// <returns></returns>
        public Tuple<ApiKey, SecretKey> CreateSystemGeneratedKey(string username)
        {
            SecurityKeysPair keysPair = SecurityKeysPairFactory.SystemGeneratedSecurityKeyPair(username, _securityKeysGenerationService);
            _persistRepository.SaveUpdate(keysPair);
            return new Tuple<ApiKey, SecretKey>(new ApiKey(keysPair.ApiKey),new SecretKey(keysPair.SecretKey) );
        }

        public Tuple<string,string> CreateUserGeneratedKey(CreateUserGeneratedSecurityKeyPair command)
        {
            if (command.Validate())
            {
                var keys = _securityKeysGenerationService.GenerateNewSecurityKeys();
                List<SecurityKeysPermission> permissions = new List<SecurityKeysPermission>();
                for (int i = 0; i < command.SecurityKeyPermissions.Length; i++)
                {
                    permissions.Add(new SecurityKeysPermission(keys.Item1, command.SecurityKeyPermissions[i].Permission,
                        command.SecurityKeyPermissions[i].Allowed));
                }
                var keysPair = SecurityKeysPairFactory.UserGeneratedSecurityPair(command.UserName,
                    command.KeyDescritpion,
                    keys.Item1, keys.Item2, command.EnableExpirationDate, command.ExpirationDateTime,
                    command.EnableStartDate, command.StartDateTime, command.EnableEndDate, command.EndDateTime,
                    permissions,
                    _securityKeysRepository);
                _persistRepository.SaveUpdate(keysPair);
                return keys;
            }
            throw new ArgumentNullException("Please assign atleast one permission.");
        }

        /// <summary>
        /// Set the permissions
        /// </summary>
        public void UpdateSecurityKeyPair(SecurityKeysPermission[] securityKeysPermissions)
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
        /// <summary>
        /// delete security key pair
        /// </summary>
        /// <param name="keyDescription"></param>
        public void DeleteSecurityKeyPair(string keyDescription)
        {
            throw new NotImplementedException();
        }
    }
}
