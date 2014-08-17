using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawLimitRepository : IWithdrawLimitRepository
    {
        IList<WithdrawLimit> _withdrawLimits = new List<WithdrawLimit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MockWithdrawLimitRepository()
        {
            WithdrawLimit withdrawLimit1 = new WithdrawLimit("Tier 0", 1000, 5000);
            WithdrawLimit withdrawLimit2 = new WithdrawLimit("Tier 1", 1000, 5000);

            _withdrawLimits.Add(withdrawLimit1);
            _withdrawLimits.Add(withdrawLimit2);
        }

        public WithdrawLimit GetWithdrawLimitByTierLevel(string tierLevel)
        {
            foreach (var withdrawLimit in _withdrawLimits)
            {
                if (withdrawLimit.TierLevel == tierLevel)
                {
                    return withdrawLimit;
                }
            }
            return null;
        }

        public WithdrawLimit GetWithdrawLimitById(int id)
        {
            foreach (var withdrawLimit in _withdrawLimits)
            {
                if (withdrawLimit.Id == id)
                {
                    return withdrawLimit;
                }
            }
            return null;
        }
    }
}
