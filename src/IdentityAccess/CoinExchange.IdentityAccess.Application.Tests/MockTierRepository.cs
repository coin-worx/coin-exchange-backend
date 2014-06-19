using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mock tier repository
    /// </summary>
    public class MockTierRepository:ITierRepository
    {
        public IList<Tier> GetAllTierLevels()
        {
            IList<Tier> tiers=new List<Tier>();
            tiers.Add(new Tier("Tier 0","Tier 0"));
            tiers.Add(new Tier("Tier 1", "Tier 1"));
            return tiers;
        }
    }
}
