using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.Repositories;

namespace CoinExchange.Funds.Application.Tests
{
    /// <summary>
    /// Mock repository for Fudns persistence
    /// </summary>
    public class MockFundsRepository : IFundsPersistenceRepository
    {
        private List<object> _objectsList = new List<object>();
 
        public void SaveOrUpdate(object domainObject)
        {
            _objectsList.Add(domainObject);
        }

        public void Delete(object domainObject)
        {
            _objectsList.Remove(domainObject);
        }

        public int GetNumberOfObjects()
        {
            return _objectsList.Count;
        }
    }
}
