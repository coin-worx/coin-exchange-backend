using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.DigitalSignature
{
    /// <summary>
    /// Serves operations related to the Digital Signature(API Secret Key pair)
    /// </summary>
    public class DigitalSignatureApplicationService : IDigitalSignatureApplicationService
    {
        private ISecurityKeysGenerationService _securityKeysGenerationService;

        /// <summary>
        /// Initializes the service for operating operations for the DigitalSignatures
        /// </summary>
        public DigitalSignatureApplicationService(ISecurityKeysGenerationService securityKeysGenerationService)
        {
            _securityKeysGenerationService = securityKeysGenerationService;
        }

        /// <summary>
        /// Generates a new API key and Secret Key pair
        /// </summary>
        /// <returns></returns>
        public SecurityKeys GetNewKey()
        {
            Tuple<string, string> keysTuple = _securityKeysGenerationService.GenerateNewApiKey();
            SecurityKeys securityKeys = new SecurityKeys(keysTuple.Item1, keysTuple.Item2);
            return securityKeys;
        }

        /// <summary>
        /// Set the permissions
        /// </summary>
        public void SetPermission()
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
