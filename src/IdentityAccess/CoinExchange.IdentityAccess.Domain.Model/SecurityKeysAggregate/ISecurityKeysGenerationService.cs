using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Interface for API Key Generation
    /// </summary>
    public interface ISecurityKeysGenerationService
    {
        /// <summary>
        /// Generates a new API Key and Secret key. Returns:
        /// Item1 = API Key
        /// Item2 = Secret Key
        /// </summary>
        /// <returns></returns>
        Tuple<ApiKey, SecretKey> GenerateNewSecurityKeys();
    }
}
