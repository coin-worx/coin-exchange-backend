using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Service to evaluate the Maximum Withdraw 
    /// </summary>
    public interface IWithdrawLimitEvaluationService
    {
        /// <summary>
        /// Evaluates the Maximum Withdraw amount
        /// </summary>
        /// <returns></returns>
        bool EvaluateMaximumWithdrawLimit(decimal withdrawalAmountUsd, IList<Ledger> depositLedgers, WithdrawLimit 
            withdrawLimit, decimal bestBidPrice, decimal bestAskPrice, decimal balance, decimal currentBalance);

        /// <summary>
        /// Daily Limit that the user can use in 24 hours
        /// </summary>
        decimal DailyLimit { get; }

        /// <summary>
        /// Daily limit that has been used by the user in the next 24 hours
        /// </summary>
        decimal DailyLimitUsed { get; }

        /// <summary>
        /// Monthly Limit that the user can use in 30 days
        /// </summary>
        decimal MonthlyLimit { get; }

        /// <summary>
        /// Amount that has been used in the last 30 days
        /// </summary>
        decimal MonthlyLimitUsed { get; }

        /// <summary>
        /// Withheld
        /// </summary>
        decimal WithheldAmount { get; }

        /// <summary>
        /// Withheld amount converted to US Dollars
        /// </summary>
        decimal WithheldConverted { get; }

        /// <summary>
        /// The maximum amount that can be withdrawn
        /// </summary>
        decimal MaximumWithdraw { get; }

        /// <summary>
        /// Represents the maximum amount that can be withdrawn in US Dollars
        /// </summary>
        decimal MaximumWithdrawUsd { get; }
    }
}
