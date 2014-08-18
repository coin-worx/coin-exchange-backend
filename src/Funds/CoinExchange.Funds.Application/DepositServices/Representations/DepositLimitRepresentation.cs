using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.DepositServices.Representations
{
    /// <summary>
    /// Representation for Dpeosit limit thresholds
    /// </summary>
    public class DepositLimitRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositLimitRepresentation(string currency, int accountId, decimal dailyLimit, decimal dailyLimitUsed, decimal monthlyLimit, decimal monthlyLimitUsed, decimal currentBalance, decimal maximumDeposit)
        {
            Currency = currency;
            DailyLimit = dailyLimit;
            DailyLimitUsed = dailyLimitUsed;
            MonthlyLimit = monthlyLimit;
            MonthlyLimitUsed = monthlyLimitUsed;
            CurrentBalance = currentBalance;
            MaximumDeposit = maximumDeposit;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// DailyLimit
        /// </summary>
        public decimal DailyLimit { get; private set; }

        /// <summary>
        /// DailyLimit that has been Used
        /// </summary>
        public decimal DailyLimitUsed { get; private set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public decimal MonthlyLimit { get; private set; }

        /// <summary>
        /// Monthly limit that has already been used
        /// </summary>
        public decimal MonthlyLimitUsed { get; private set; }

        /// <summary>
        /// Current balance for this currency of this user
        /// </summary>
        public decimal CurrentBalance { get; private set; }

        /// <summary>
        /// Maximum Deposit threshl that the user can commit at the moment
        /// </summary>
        public decimal MaximumDeposit { get; private set; }
    }
}
