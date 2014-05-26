using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    public class MockPersistenceRepository : IPersistenceRepository
    {
        private bool _shouldPretendToSave = false;

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
        }
    }
}
