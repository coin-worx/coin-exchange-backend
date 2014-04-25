using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.Repositories
{
    /// <summary>
    /// Interface for adding or updating readmodels
    /// </summary>
    public interface IPersistanceRepository
    {
        void SaveOrUpdate(object readModelObject);
    }
}
