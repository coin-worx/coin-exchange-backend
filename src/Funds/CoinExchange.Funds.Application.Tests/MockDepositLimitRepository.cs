using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockDepositLimitRepository : IDepositLimitRepository
    {
        IList<DepositLimit> _depositLimits = new List<DepositLimit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MockDepositLimitRepository()
        {
            DepositLimit depositLimit1 = new DepositLimit("Tier 0", 1000, 5000);
            DepositLimit depositLimit2 = new DepositLimit("Tier 1", 1000, 5000);

            _depositLimits.Add(depositLimit1);
            _depositLimits.Add(depositLimit2);
        }

        public DepositLimit GetDepositLimitByTierLevel(string tierLevel)
        {
            foreach (var depositLimit in _depositLimits)
            {
                if (depositLimit.TierLevel == tierLevel)
                {
                    return depositLimit;
                }
            }
            return null;
        }

        public DepositLimit GetLimitByTierLevelAndCurrency(string tierLevel, string currencyType)
        {
            foreach (var depositLimit in _depositLimits)
            {
                if (depositLimit.TierLevel == tierLevel)
                {
                    return depositLimit;
                }
            }
            return null;
        }

        public DepositLimit GetDepositLimitById(int id)
        {
            foreach (var depositLimit in _depositLimits)
            {
                if (depositLimit.Id == id)
                {
                    return depositLimit;
                }
            }
            return null;
        }

        public IList<DepositLimit> GetAllDepositLimits()
        {
            throw new NotImplementedException();
        }
    }
}
