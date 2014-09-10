using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawIdGeneratorService : IWithdrawIdGeneratorService
    {
        public string GenerateNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
