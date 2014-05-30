using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;

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
        public static SecurityKeysPair SystemGeneratedSecurityKeyPair(int userId,ISecurityKeysGenerationService securityKeysGeneration)
        {
            var keys = securityKeysGeneration.GenerateNewSecurityKeys();
            SecurityKeysPair securityKeysPair=new SecurityKeysPair(keys.Item1,keys.Item2,DateTime.Now.ToString(),userId,true);
            return securityKeysPair;
        }

        /// <summary>
        /// Create user generated api key
        /// </summary>
        /// <returns></returns>
        public static SecurityKeysPair UserGeneratedSecurityPair(int userId,string keyDescription,string apiKey,string secretKey,bool enableExpirationDate,string expirationDate,bool enableStartDate,string startDate,bool enableEndDate,string endDate,List<SecurityKeysPermission> keysPermissions ,ISecurityKeysRepository repository)
        {
            //check if key description already exist
            if (repository.GetByKeyDescriptionAndUserId(keyDescription,userId) != null)
            {
                throw new ArgumentException("The key description already exist");
            }
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(apiKey, secretKey, keyDescription, userId, false,keysPermissions);
            if (enableExpirationDate)
            {
                securityKeysPair.ExpirationDate = Convert.ToDateTime(expirationDate);
            }
            if (enableStartDate)
            {
                securityKeysPair.StartDate = Convert.ToDateTime(startDate);
            }
            if (enableEndDate)
            {
                securityKeysPair.EndDate = Convert.ToDateTime(endDate);
            }
            securityKeysPair.EnableStartDate = enableStartDate;
            securityKeysPair.EnableEndDate = enableEndDate;
            securityKeysPair.EnableExpirationDate = enableExpirationDate;
            return securityKeysPair;
        }
    }
}
