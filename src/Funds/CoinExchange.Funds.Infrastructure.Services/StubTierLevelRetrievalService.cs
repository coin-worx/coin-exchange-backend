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
        /// <summary>
        /// Gets the Highest Tier Level for the user
        /// </summary>
        /// <returns></returns>
        public string GetCurrentTierLevel(string userId)
        {
            return "Tier 1";
        }
    }
}
