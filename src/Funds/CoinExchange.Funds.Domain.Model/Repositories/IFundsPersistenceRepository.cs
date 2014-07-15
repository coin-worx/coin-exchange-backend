using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Repositories
{
    /// <summary>
    /// Interface for repository responsible for saving and updating items in the Funds BC
    /// </summary>
    public interface IFundsPersistenceRepository
    {
        void SaveOrUpdate(object domainObject);
    }
}
