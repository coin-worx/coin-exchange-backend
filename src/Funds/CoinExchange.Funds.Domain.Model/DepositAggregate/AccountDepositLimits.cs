using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Represents the Deposit Threshold limits and the limits used, along with balance information
    /// </summary>
    public class AccountDepositLimits
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AccountDepositLimits(CurrencyAggregate.Currency currency, AccountId accountId, decimal dailyLimit, decimal dailyLimitUsed, 
            decimal monthlyLimit, decimal monthlyLimitUsed, decimal currentBalance, decimal maximumDeposit)
        {
            Currency = currency;
            AccountId = accountId;
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
        public CurrencyAggregate.Currency Currency { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public AccountId AccountId { get; private set; }

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
