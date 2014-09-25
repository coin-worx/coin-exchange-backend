using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Contains the original Withdrawal limits and the limits that have been used by the user
    /// </summary>
    public class AccountWithdrawLimits
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AccountWithdrawLimits(CurrencyAggregate.Currency currency, AccountId accountId, decimal dailyLimit, decimal dailyLimitUsed, 
            decimal monthlyLimit, decimal monthlyLimitUsed, decimal currentBalance, decimal maximumWithdraw,
            decimal withHeld, decimal fee, decimal minimumAmount)
        {
            Currency = currency;
            AccountId = accountId;
            DailyLimit = dailyLimit;
            DailyLimitUsed = dailyLimitUsed;
            MonthlyLimit = monthlyLimit;
            MonthlyLimitUsed = monthlyLimitUsed;
            CurrentBalance = currentBalance;
            MaximumWithdraw = maximumWithdraw;
            Withheld = withHeld;
            Fee = fee;
            MinimumAmount = minimumAmount;
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
        /// Maximum Withdraw threshold that the user can commit at the moment
        /// </summary>
        public decimal MaximumWithdraw { get; private set; }

        /// <summary>
        /// Maximum Withdraw amount in US Dollars
        /// </summary>
        public decimal MaximumWithdrawInUsd { get; private set; }

        /// <summary>
        /// Withheld Amount
        /// </summary>
        public decimal Withheld { get; private set; }

        /// <summary>
        /// Withheld amount in US Dollars
        /// </summary>
        public decimal WithheldInUsd { get; private set; }

        /// <summary>
        /// Fee for the withdrawal
        /// </summary>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Minimum amount that can be withdrawn
        /// </summary>
        public decimal MinimumAmount { get; private set; }
    }
}
