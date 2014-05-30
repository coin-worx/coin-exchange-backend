using System;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
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
        Tuple<ApiKey, SecretKey> CreateSystemGeneratedKey(int userId);

        /// <summary>
        /// Item 1=Api Key
        /// Item 2=Secret Key
        /// </summary>
        /// <returns></returns>
        Tuple<string,string> CreateUserGeneratedKey(CreateUserGeneratedSecurityKeyPair command);

        /// <summary>
        /// Update Security Key Pair Info
        /// </summary>
        bool UpdateSecurityKeyPair(UpdateUserGeneratedSecurityKeyPair updateCommand);

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
    }
}
