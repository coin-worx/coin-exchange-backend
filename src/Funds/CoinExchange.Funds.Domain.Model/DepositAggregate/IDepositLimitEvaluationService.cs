using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for determining the Daily and Monthly limits for Deposit
    /// </summary>
    public interface IDepositLimitEvaluationService
    {
        /// <summary>
        /// Evaluate the limit for deposit and signify if current transaction is within the deposit limits
        /// </summary>
        /// <returns></returns>
        bool EvaluateDepositLimit(decimal amountInUsd, IList<Ledger> depositLedgers, DepositLimit depositLimit,
            decimal bestBidPrice, decimal bestAskPrice);

        /// <summary>
        /// Daily Deposit Limit
        /// </summary>
        decimal DailyLimit { get; }

        /// <summary>
        /// Daily Deposit Limit that has been used in the last 24 hours
        /// </summary>
        decimal DailyLimitUsed { get; }

        /// <summary>
        /// Monthly Deposit Limit
        /// </summary>
        decimal MonthlyLimit { get; }

        /// <summary>
        /// Monthly Deposit Limit that has been used in the last 30 days
        /// </summary>
        decimal MonthlyLimitUsed { get; }

        /// <summary>
        /// Maximum Deposit amount allowed to the user at this moment.. Deposit should be kept 5-10% lower than this limit
        /// </summary>
        decimal MaximumDeposit { get; }
    }
}
