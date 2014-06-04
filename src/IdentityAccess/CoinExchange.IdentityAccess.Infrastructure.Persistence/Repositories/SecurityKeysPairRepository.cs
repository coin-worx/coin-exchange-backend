using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Services;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Projection;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for DigitalSignatureInfo
    /// </summary>
    [Repository]
    public class SecurityKeysPairRepository : NHibernateSessionFactory, ISecurityKeysRepository,IApiKeyInfoAccess
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

        [Transaction(ReadOnly = true)]
        public SecurityKeysPair GetByDescriptionAndApiKey(string description, string apiKey)
        {
            return
                CurrentSession.QueryOver<SecurityKeysPair>()
                    .Where(x => x.KeyDescription == description && x.ApiKey == apiKey && x.Deleted == false)
                    .SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public object GetByUserId(int userId)
        {
            ICriteria crit = CurrentSession.CreateCriteria<SecurityKeysPair>().Add(Restrictions.Eq("UserId", userId)).Add(Restrictions.Eq("SystemGenerated", false)).Add(Restrictions.Eq("Deleted", false))
                        .SetProjection(Projections.ProjectionList()
                            .Add(Projections.Property("KeyDescription"), "KeyDescription")
                            .Add(Projections.Property("ExpirationDate"), "ExpirationDate")
                            .Add(Projections.Property("LastModified"), "LastModified")
                            .Add(Projections.Property("CreationDateTime"), "CreationDateTime")).SetResultTransformer(Transformers.AliasToBean<SecurityKeyPairList>());
            IList<SecurityKeyPairList> results = crit.List<SecurityKeyPairList>();
            return results;
            //return CurrentSession.QueryOver<SecurityKeysPair>().Select(t=>t.KeyDescription,t=>t.ExpirationDate,t=>t.CreationDateTime,t=>t.LastModified).Where(t=>t.UserId==userId && t.Deleted==false && t.SystemGenerated==false)
            //   .List<SecurityKeyPairList>();
        }

        /// <summary>
        /// GetUserId from apikey
        /// </summary>
        /// <param name="apiKey"></param>
        [Transaction(ReadOnly = true)]
        public int GetUserIdFromApiKey(string apiKey)
        {
            return
                CurrentSession.QueryOver<SecurityKeysPair>()
                    .Where(x=>x.ApiKey == apiKey && x.Deleted == false)
                    .SingleOrDefault().UserId;
        }
    }
}
