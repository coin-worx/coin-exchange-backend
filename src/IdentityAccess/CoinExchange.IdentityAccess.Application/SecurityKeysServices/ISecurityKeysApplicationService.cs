using System;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices
{
    /// <summary>
    /// Interface for Digital Signature Application Service
    /// </summary>
    public interface ISecurityKeysApplicationService
    {
        /// <summary>
        /// Generates new key
        /// </summary>
        /// <returns></returns>
        Tuple<ApiKey, SecretKey, DateTime> CreateSystemGeneratedKey(int userId);

        /// <summary>
        /// Item 1=Api Key
        /// Item 2=Secret Key
        /// </summary>
        /// <returns></returns>
       SecurityKeyPair CreateUserGeneratedKey(CreateUserGeneratedSecurityKeyPair command,string apiKey);

        /// <summary>
        /// Update Security Key Pair Info
        /// </summary>
        bool UpdateSecurityKeyPair(UpdateUserGeneratedSecurityKeyPair updateCommand, string apiKey);

        /// <summary>
        /// Delete security key pair
        /// </summary>
        void DeleteSecurityKeyPair(string keyDescription, string systemGeneratedApiKey);

        /// <summary>
        /// Is the API key valid
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        bool ApiKeyValidation(string apiKey);

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns></returns>
        SecurityKeyPermissionsRepresentation[] GetPermissions();

        /// <summary>
        /// Get api keys list
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        object GetSecurityKeysPairList(string apiKey);

        /// <summary>
        /// Get details of specific apikey
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        SecurityKeyRepresentation GetKeyDetails(string keyDescription, string apiKey);

    }
}
