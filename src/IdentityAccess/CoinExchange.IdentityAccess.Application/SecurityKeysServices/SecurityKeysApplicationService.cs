using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

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
        private IPermissionRepository _permissionRepository;
        private int _keyDescriptionCounter = 0;

        /// <summary>
        /// Initializes the service for operating operations for the DigitalSignatures
        /// </summary>
        public SecurityKeysApplicationService(ISecurityKeysGenerationService securityKeysGenerationService,
            IIdentityAccessPersistenceRepository persistenceRepository, ISecurityKeysRepository securityKeysRepository,IPermissionRepository permissionRepository)
        {
            _securityKeysGenerationService = securityKeysGenerationService;
            _persistRepository = persistenceRepository;
            _securityKeysRepository = securityKeysRepository;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Generates a new API key and Secret Key pair
        /// </summary>
        /// <returns></returns>
        public Tuple<ApiKey, SecretKey> CreateSystemGeneratedKey(int userId)
        {
            SecurityKeysPair keysPair = SecurityKeysPairFactory.SystemGeneratedSecurityKeyPair(userId, _securityKeysGenerationService);
            _persistRepository.SaveUpdate(keysPair);
            return new Tuple<ApiKey, SecretKey>(new ApiKey(keysPair.ApiKey),new SecretKey(keysPair.SecretKey) );
        }

        public Tuple<string,string> CreateUserGeneratedKey(CreateUserGeneratedSecurityKeyPair command,string apiKey)
        {
            if (command.Validate())
            {
                //get security key pair for user name
                var getSecurityKeyPair = _securityKeysRepository.GetByApiKey(apiKey);
                if (getSecurityKeyPair == null)
                {
                    throw new ArgumentException("Invalid api key");
                }
                var keys = _securityKeysGenerationService.GenerateNewSecurityKeys();
                List<SecurityKeysPermission> permissions = new List<SecurityKeysPermission>();
                for (int i = 0; i < command.SecurityKeyPermissions.Length; i++)
                {
                    permissions.Add(new SecurityKeysPermission(keys.Item1, command.SecurityKeyPermissions[i].Permission,
                        command.SecurityKeyPermissions[i].Allowed));
                }
                var keysPair = SecurityKeysPairFactory.UserGeneratedSecurityPair(getSecurityKeyPair.UserId,
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
        public bool UpdateSecurityKeyPair(UpdateUserGeneratedSecurityKeyPair updateCommand,string apiKey)
        {
            if (updateCommand.Validate())
            {
                //get security key pair for user name
                var getSecurityKeyPair = _securityKeysRepository.GetByApiKey(apiKey);
                if (getSecurityKeyPair == null)
                {
                    throw new ArgumentException("Invalid api key");
                }
                var keyPair = _securityKeysRepository.GetByApiKey(updateCommand.ApiKey);
                if (keyPair == null)
                {
                    throw new ArgumentException("Invalid Api Key");
                }
                //check if key description already exist
                if (_securityKeysRepository.GetByDescriptionAndApiKey(updateCommand.KeyDescritpion, updateCommand.ApiKey) != null)
                {
                    throw new ArgumentException("The key description already exist");
                }
                //update parameters
                keyPair.UpdateSecuritykeyPair(updateCommand.KeyDescritpion, updateCommand.EnableStartDate,
                    updateCommand.EnableEndDate, updateCommand.EnableExpirationDate, updateCommand.EndDateTime,
                    updateCommand.StartDateTime, updateCommand.ExpirationDateTime);

                //update permissions
                List<SecurityKeysPermission> permissions = new List<SecurityKeysPermission>();
                for (int i = 0; i < updateCommand.SecurityKeyPermissions.Length; i++)
                {
                    permissions.Add(new SecurityKeysPermission(keyPair.ApiKey,
                        updateCommand.SecurityKeyPermissions[i].Permission,
                        updateCommand.SecurityKeyPermissions[i].Allowed));
                }
                keyPair.UpdatePermissions(permissions.ToArray());
                //persist
                _persistRepository.SaveUpdate(keyPair);
                return true;
            }
            throw new InvalidOperationException("Please assign atleast one permission.");
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
        public void DeleteSecurityKeyPair(string keyDescription,string systemGeneratedApiKey)
        {
            //get security key pair for user name
            var getSecurityKeyPair = _securityKeysRepository.GetByApiKey(systemGeneratedApiKey);

            if (getSecurityKeyPair == null)
            {
                throw new ArgumentException("Invalid api key");
            }
            var keyPair = _securityKeysRepository.GetByKeyDescriptionAndUserId(keyDescription, getSecurityKeyPair.UserId);
            if (keyPair == null)
            {
                throw new InvalidOperationException("Could not find the security key pair.");
            }
            _securityKeysRepository.DeleteSecurityKeysPair(keyPair);
        }


        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns></returns>
        public SecurityKeyPermissionsRepresentation[] GetPermissions()
        {
            List<SecurityKeyPermissionsRepresentation> representations=new List<SecurityKeyPermissionsRepresentation>();
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();
            for (int i = 0; i < permissions.Count; i++)
            {
                representations.Add(new SecurityKeyPermissionsRepresentation(false,permissions[i]));
            }
            return representations.ToArray();
        }

        /// <summary>
        /// Get api keys list
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public object GetSecurityKeysPairList(string apiKey)
        {
            //get security key pair for user name
            var getSecurityKeyPair = _securityKeysRepository.GetByApiKey(apiKey);
            return _securityKeysRepository.GetByUserId(getSecurityKeyPair.UserId);
        }

        /// <summary>
        /// get details of specific api key
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public SecurityKeyRepresentation GetKeyDetails(string keyDescription, string apiKey)
        {
            //get security key pair for user name
            var getSecurityKeyPair = _securityKeysRepository.GetByApiKey(apiKey);
            var getApiKey = _securityKeysRepository.GetByKeyDescriptionAndUserId(keyDescription,
                getSecurityKeyPair.UserId);
            List<SecurityKeyPermissionsRepresentation> representations=new List<SecurityKeyPermissionsRepresentation>();
            SecurityKeysPermission[] permissions = getApiKey.GetAllPermissions();
            for (int i = 0; i < permissions.Length; i++)
            {
                representations.Add(new SecurityKeyPermissionsRepresentation(permissions[i].IsAllowed,permissions[i].Permission));
            }
            string expirationDate = getApiKey.EnableExpirationDate ? getApiKey.ExpirationDate.ToString() : "";
            string startDate = getApiKey.EnableStartDate ? getApiKey.StartDate.ToString() : "";
            string endDate = getApiKey.EnableEndDate ? getApiKey.EndDate.ToString() : "";
            return new SecurityKeyRepresentation(getApiKey.KeyDescription,getApiKey.ApiKey,getApiKey.SecretKey, getApiKey.EnableStartDate,
                getApiKey.EnableEndDate, getApiKey.EnableExpirationDate, endDate,
                startDate, expirationDate, representations.ToArray());
        }
    }
}
