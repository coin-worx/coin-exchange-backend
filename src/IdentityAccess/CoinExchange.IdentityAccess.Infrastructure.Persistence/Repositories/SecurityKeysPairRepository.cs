using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for DigitalSignatureInfo
    /// </summary>
    [Repository]
    public class SecurityKeysPairRepository : NHibernateSessionFactory, ISecurityKeysRepository
    {
        [Transaction(ReadOnly = true)]
        public SecurityKeysPair GetByKeyDescriptionAndUserId(string keyDescription, int userId)
        {
            return
                CurrentSession.QueryOver<SecurityKeysPair>()
                    .Where(x => x.KeyDescription == keyDescription && x.UserId == userId && x.Deleted == false)
                    .SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public SecurityKeysPair GetByApiKey(string apiKey)
        {
            return
                CurrentSession.QueryOver<SecurityKeysPair>()
                    .Where(x => x.ApiKey == apiKey && x.Deleted == false)
                    .SingleOrDefault();
        }
       
        /// <summary>
        /// Soft delete security key pair.
        /// </summary>
        /// <param name="securityKeysPair"></param>
        /// <returns></returns>
        [Transaction(ReadOnly = false)]
        public bool DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair)
        {
            securityKeysPair.Deleted = true;
            CurrentSession.SaveOrUpdate(securityKeysPair);
            return true;
        }

        [Transaction(ReadOnly = false)]
        public SecurityKeysPair GetByDescriptionAndApiKey(string description, string apiKey)
        {
            return
                CurrentSession.QueryOver<SecurityKeysPair>()
                    .Where(x => x.KeyDescription == description && x.ApiKey == apiKey && x.Deleted == false)
                    .SingleOrDefault();
        }
    }
}
