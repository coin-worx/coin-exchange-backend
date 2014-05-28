using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NHibernate.Criterion;
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
            return CurrentSession.QueryOver<User>().Where(x => x.Username == username && x.Deleted == false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByActivationKey(string activationKey)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.ActivationKey == activationKey&&x.Deleted==false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByEmail(string email)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.Email == email&&x.Deleted==false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByEmailAndUserName(string username, string email)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.Username == username && x.Email==email && x.Deleted==false).SingleOrDefault();
        }

        // ToDo For Bilal: Please implement this method and return the user. ALso create the Hibernate mapping for 
        // User.ForgotPasswordCode
        public User GetUserByForgotPasswordCode(string forgotPasswordCode)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = false)]
        public bool DeleteUser(User user)
        {
            user.Deleted = true;
            CurrentSession.SaveOrUpdate(user);
            return true;
        }
    }
}
