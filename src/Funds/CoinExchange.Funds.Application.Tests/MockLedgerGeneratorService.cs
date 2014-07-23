using System;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockLedgerGeneratorService : ILedgerIdGeneraterService
    {
        public string GenerateLedgerId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
