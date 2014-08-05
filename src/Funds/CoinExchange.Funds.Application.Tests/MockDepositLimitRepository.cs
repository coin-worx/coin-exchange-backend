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
        public DepositLimit GetDepositLimitByTierLevel(string tierLevel)
        {
            throw new NotImplementedException();
        }

        public DepositLimit GetDepositLimitById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
