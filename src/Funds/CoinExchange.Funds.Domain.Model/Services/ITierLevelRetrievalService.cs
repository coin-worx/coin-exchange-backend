using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Service to retieve the TierLevel(s) for a user from Trades BC
    /// </summary>
    public interface ITierLevelRetrievalService
    {
        /// <summary>
        /// Gets the most highest User Tier Level
        /// </summary>
        /// <returns></returns>
        string GetCurrentTierLevel(int userId);
    }
}
