using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    public class MockSecurityKeysRepository : ISecurityKeysRepository
    {
        private List<SecurityKeysPair> _securityKeysPairsList = new List<SecurityKeysPair>();

        /// <summary>
        /// Get by Key Desc
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SecurityKeysPair GetByKeyDescription(string keyDescription,string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get by API key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public SecurityKeysPair GetByApiKey(string apiKey)
        {
            foreach (var securityKeysPair in _securityKeysPairsList)
            {
                if (securityKeysPair.ApiKey == apiKey)
                {
                    return securityKeysPair;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete by Security Pair
        /// </summary>
        /// <param name="securityKeysPair"></param>
        /// <returns></returns>
        public bool DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            return true;
        }

        /// <summary>
        /// Add Security KeysPair
        /// </summary>
        /// <param name="securityKeysPair"></param>
        public void AddSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            _securityKeysPairsList.Add(securityKeysPair);
        }

        SecurityKeysPair ISecurityKeysRepository.GetByKeyDescriptionAndUserName(string keyDescription, string userName)
        {
            throw new NotImplementedException();
        }

        SecurityKeysPair ISecurityKeysRepository.GetByApiKey(string apiKey)
        {
            return _securityKeysPairsList.First();
        }

        bool ISecurityKeysRepository.DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            return true;
        }
        
        public SecurityKeysPair GetByDescriptionAndApiKey(string description, string apiKey)
        {
            throw new NotImplementedException();
        }    }
}
