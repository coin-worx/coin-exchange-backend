using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    [Repository]
    public class UserRepository : NHibernateSessionFactory, IUserRepository
    {
        [Transaction(ReadOnly = true)]
        public User GetUserByUserName(string username)
        {
            return CurrentSession.Get<User>(username);
        }
    }
}
