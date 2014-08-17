using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Retrieves The Tier Level Service
    /// </summary>
    public class TierLevelRetrievalService : ITierLevelRetrievalService
    {
        private dynamic _userTierLevelApplicationService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="userTierLevelApplicationService"></param>
        public TierLevelRetrievalService(dynamic userTierLevelApplicationService)
        {
            _userTierLevelApplicationService = userTierLevelApplicationService;
        }

        /// <summary>
        /// Gets the Tier Level for the given UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetCurrentTierLevel(int userId)
        {
            dynamic tierLevel = _userTierLevelApplicationService.GetTierLevel(userId);
            if (tierLevel != null)
            {
                return tierLevel.Tier.TierLevel;
            }
            return null;
        }
    }
}
