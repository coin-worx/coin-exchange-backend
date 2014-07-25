using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockDepositIdGeneratorService : IDepositIdGeneratorService
    {
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
