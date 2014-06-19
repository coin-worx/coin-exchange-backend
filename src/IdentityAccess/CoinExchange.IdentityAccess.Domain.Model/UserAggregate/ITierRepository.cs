using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Tier repository interface
    /// </summary>
    public interface ITierRepository
    {
        /// <summary>
        /// Get list of all tiers
        /// </summary>
        /// <returns></returns>
        IList<Tier> GetAllTierLevels();
    }
}
