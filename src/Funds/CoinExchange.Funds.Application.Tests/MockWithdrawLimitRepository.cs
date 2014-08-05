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
        public WithdrawLimit GetWithdrawLimitByTierLevel(string tierLevel)
        {
            throw new NotImplementedException();
        }

        public WithdrawLimit GetWithdrawLimitById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
