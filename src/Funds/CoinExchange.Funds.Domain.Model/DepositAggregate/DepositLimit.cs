using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Specifies the monthly and daily limits for withdrawals
    /// </summary>
    public class DepositLimit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositLimit()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositLimit(string tierLevel, decimal dailyLimit, decimal monthlyLimit)
        {
            TierLevel = tierLevel;
            DailyLimit = dailyLimit;
            MonthlyLimit = monthlyLimit;            
        }

        /// <summary>
        /// Primary key for Database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Tier Level
        /// </summary>
        public string TierLevel { get; private set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public decimal MonthlyLimit { get; private set; }

        /// <summary>
        /// Daily Limit
        /// </summary>
        public decimal DailyLimit { get; private set; }
    }
}
