using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    public class MockPersistenceRepository : IIdentityAccessPersistenceRepository
    {
        private bool _shouldPretendToSave = false;
        private List<User> usersList = new List<User>();

        /// <summary>
        /// Accepts parameter to check whether to pretend to save the object and throw an exception, or just do nothing
        /// </summary>
        /// <param name="shouldPretendToSave"></param>
        public MockPersistenceRepository(bool shouldPretendToSave)
        {
            _shouldPretendToSave = shouldPretendToSave;
        }

        /// <summary>
        /// SaveUpdate
        /// </summary>
        /// <param name="entity"></param>
        public void SaveUpdate(object entity)
        {
            if (_shouldPretendToSave)
            {
                throw new DataException("Could not save object in database");
            }
            if (entity is User)
            {
                usersList.Add(entity as User);
            }
        }

        public User GetUser(string username)
        {
            foreach (var user in usersList)
            {
                if (user.Username == username)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Users
        /// </summary>
        public List<User> Users { get; set; }
    }
}
