using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Security key pair factory
    /// </summary>
    public class SecurityKeysPairFactory
    {
        /// <summary>
        /// Create system generated security key pair
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="securityKeysGeneration"></param>
        /// <returns></returns>
        public static SecurityKeysPair SystemGeneratedSecurityKeyPair(string userName,ISecurityKeysGenerationService securityKeysGeneration)
        {
            var keys = securityKeysGeneration.GenerateNewSecurityKeys();
            SecurityKeysPair securityKeysPair=new SecurityKeysPair(keys.Item1,keys.Item2,DateTime.Now.ToString(),userName,true);
            return securityKeysPair;
        }

        /// <summary>
        /// Create user generated api key
        /// </summary>
        /// <returns></returns>
        public static SecurityKeysPair UserGeneratedSecurityPair()
        {
            return null;
        }
    }
}
