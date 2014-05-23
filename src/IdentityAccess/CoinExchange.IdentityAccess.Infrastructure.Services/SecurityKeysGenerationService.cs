using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Service for generating the API key
    /// </summary>
    public class SecurityKeysGenerationService : ISecurityKeysGenerationService
    {
        public DigitalSignature GenerateNewSecurityKeys()
        {
            Guid apiKeyGuid = Guid.NewGuid();
            string apiKeyGuidString = Convert.ToBase64String(apiKeyGuid.ToByteArray());
            apiKeyGuidString = apiKeyGuidString.Replace("-", "");
            apiKeyGuidString = apiKeyGuidString.Replace("=", "");
            apiKeyGuidString = apiKeyGuidString.Replace("/", "");
            apiKeyGuidString = apiKeyGuidString.Replace("+", "");

            Guid secretKeyGuid = Guid.NewGuid();
            string secretKeyGuidString = Convert.ToBase64String(secretKeyGuid.ToByteArray());
            secretKeyGuidString = secretKeyGuidString.Replace("-", "");
            secretKeyGuidString = secretKeyGuidString.Replace("=", "");
            secretKeyGuidString = secretKeyGuidString.Replace("/", "");
            secretKeyGuidString = secretKeyGuidString.Replace("+", "");
            return new DigitalSignature(apiKeyGuidString, secretKeyGuidString);
        }
    }
}
