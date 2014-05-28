using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mocks User Repository
    /// </summary>
    public class MockUserRepository : IUserRepository
    {
        private List<User> _userList = new List<User>(); 

        /// <summary>
        /// Get the User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByUserName(string username)
        {
            foreach (var user in _userList)
            {
                if (user.Username.Equals(username))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Get user by activation key
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public User GetUserByActivationKey(string activationKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get by email and username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmailAndUserName(string username, string email)
        {
            throw new NotImplementedException();
        }

        public User GetUserByForgotPasswordCode(string forgotPasswordCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the User to the User collection only in this Mock implementation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(User user)
        {
            _userList.Add(user);
            return true;
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
