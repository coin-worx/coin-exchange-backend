using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Representation for the Withdraw Tier Level
    /// </summary>
    public class WithdrawTierLimitRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawTierLimitRepresentation(decimal tier0DailyLimit, decimal tier0MonthlyLimit, decimal tier1DailyLimit, decimal tier1MonthlyLimit, decimal tier2DailyLimit, decimal tier2MonthlyLimit, decimal tier3DailyLimit, decimal tier3MonthlyLimit, decimal tier4DailyLimit, decimal tier4MonthlyLimit)
        {
            Tier0DailyLimit = tier0DailyLimit;
            Tier0MonthlyLimit = tier0MonthlyLimit;
            Tier1DailyLimit = tier1DailyLimit;
            Tier1MonthlyLimit = tier1MonthlyLimit;
            Tier2DailyLimit = tier2DailyLimit;
            Tier2MonthlyLimit = tier2MonthlyLimit;
            Tier3DailyLimit = tier3DailyLimit;
            Tier3MonthlyLimit = tier3MonthlyLimit;
            Tier4DailyLimit = tier4DailyLimit;
            Tier4MonthlyLimit = tier4MonthlyLimit;
        }

        /// <summary>
        /// Tier 0 Daily Limit
        /// </summary>
        public decimal Tier0DailyLimit { get; private set; }

        /// <summary>
        /// Tier 0 Monthly Limit
        /// </summary>
        public decimal Tier0MonthlyLimit { get; private set; }
        /// <summary>
        /// Tier 1 Daily Limit
        /// </summary>
        public decimal Tier1DailyLimit { get; private set; }

        /// <summary>
        /// Tier 1 Monthly Limit
        /// </summary>
        public decimal Tier1MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 2 Daily Limit
        /// </summary>
        public decimal Tier2DailyLimit { get; private set; }

        /// <summary>
        /// Tier 2 Monthly Limit
        /// </summary>
        public decimal Tier2MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 3 Daily Limit
        /// </summary>
        public decimal Tier3DailyLimit { get; private set; }

        /// <summary>
        /// Tier 3 Monthly Limit
        /// </summary>
        public decimal Tier3MonthlyLimit { get; private set; }

        /// <summary>
        /// Tier 4 Daily Limit
        /// </summary>
        public decimal Tier4DailyLimit { get; private set; }

        /// <summary>
        /// Tier 4 Monthly Limit
        /// </summary>
        public decimal Tier4MonthlyLimit { get; private set; }
    }
}
