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

        public SecurityKeysPair GetByKeyDescription(string keyDescription,string userName)
        {
            throw new NotImplementedException();
        }

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

        public void AddSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            _securityKeysPairsList.Add(securityKeysPair);
        }
    }
}
