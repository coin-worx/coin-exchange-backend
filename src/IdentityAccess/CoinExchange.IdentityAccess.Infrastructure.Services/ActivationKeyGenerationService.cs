using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Unique Activation Key Generation Service
    /// </summary>
    public class ActivationKeyGenerationService : IActivationKeyGenerationService
    {
        /// <summary>
        /// Generates a new and unique activation key
        /// </summary>
        /// <returns></returns>
        public string GenerateNewActivationKey()
        {
            Guid newGuid = Guid.NewGuid();
            string guidString = Convert.ToBase64String(newGuid.ToByteArray());
            guidString = guidString.Replace("-", "");
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("/", "");
            guidString = guidString.Replace("+", "");
            return guidString;
        }
    }
}
