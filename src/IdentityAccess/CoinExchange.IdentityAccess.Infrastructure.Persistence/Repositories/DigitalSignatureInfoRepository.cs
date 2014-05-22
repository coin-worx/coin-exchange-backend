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
    public class DigitalSignatureInfoRepository:NHibernateSessionFactory,IDigitalSignatureInfoRepository
    {
        [Transaction(ReadOnly = true)]
        public DigitalSignatureInfo GetByKeyDescription(string keyDescription)
        {
            return CurrentSession.Get<DigitalSignatureInfo>(keyDescription);
        }

        [Transaction(ReadOnly = true)]
        public DigitalSignatureInfo GetByApiKey(string apiKey)
        {
            return
                CurrentSession.QueryOver<DigitalSignatureInfo>()
                    .Where(x => x.SecurityKeys.ApiKey == apiKey)
                    .SingleOrDefault();
        }
    }
}
