using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Service to Get the Tier Levcel(s) of the user
    /// </summary>
    public class StubTierLevelRetrievalService : ITierLevelRetrievalService
    {
        private string _currentTierLevel = "";

        /// <summary>
        /// Gets the Highest Tier Level for the user
        /// </summary>
        /// <returns></returns>
        public string GetCurrentTierLevel(int userId)
        {
            if (string.IsNullOrEmpty(_currentTierLevel))
            {
                return "Tier 1";
            }
            return _currentTierLevel;
        }

        public void SetTierLevel(string tierLevel)
        {
            _currentTierLevel = tierLevel;
        }
    }
}
