using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of permission repository
    /// </summary>
    [Repository]
    public class PermissionRespository:NHibernateSessionFactory,IPermissionRepository
    {
        [Transaction(ReadOnly = true)]
        public IList<Permission> GetAllPermissions()
        {
            return CurrentSession.QueryOver<Permission>().List<Permission>();
        }
    }
}
