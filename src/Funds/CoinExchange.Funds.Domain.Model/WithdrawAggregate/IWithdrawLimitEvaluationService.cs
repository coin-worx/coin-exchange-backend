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
        bool EvaluateMaximumWithdrawLimit(double withdrawalAmountUsd, IList<Ledger> depositLedgers, WithdrawLimit 
            withdrawLimit, double bestBidPrice, double bestAskPrice, double balance, double currentBalance);

        /// <summary>
        /// Daily Limit that the user can use in 24 hours
        /// </summary>
        double DailyLimit { get; }

        /// <summary>
        /// Daily limit that has been used by the user in the next 24 hours
        /// </summary>
        double DailyLimitUsed { get; }

        /// <summary>
        /// Monthly Limit that the user can use in 30 days
        /// </summary>
        double MonthlyLimit { get; }

        /// <summary>
        /// Amount that has been used in the last 30 days
        /// </summary>
        double MonthlyLimitUsed { get; }

        /// <summary>
        /// Withheld
        /// </summary>
        double WithheldAmount { get; }

        /// <summary>
        /// Withheld amount converted to US Dollars
        /// </summary>
        double WithheldConverted { get; }

        /// <summary>
        /// The maximum amount that can be withdrawn
        /// </summary>
        double MaximumWithdraw { get; }

        /// <summary>
        /// Represents the maximum amount that can be withdrawn in US Dollars
        /// </summary>
        double MaximumWithdrawUsd { get; }
    }
}
