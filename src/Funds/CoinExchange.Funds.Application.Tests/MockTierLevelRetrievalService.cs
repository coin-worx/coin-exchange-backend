using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockTierLevelRetrievalService : ITierLevelRetrievalService
    {
        public string GetCurrentTierLevel(int userId)
        {
            return "Tier 1";
        }
    }
}
