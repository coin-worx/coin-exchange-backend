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
        /// Tier Level
        /// </summary>
        public string TierLevel { get; set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public double MonthlyLimit { get; set; }

        /// <summary>
        /// Daily Limit
        /// </summary>
        public double DailyLimit { get; set; }
    }
}
