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
        bool EvaluateDepositLimit(double currentDepositAmount, IList<Ledger> depositLedgers, DepositLimit depositLimit, double bestBidPrice, double
            bestAskPrice);

        /// <summary>
        /// Daily Deposit Limit
        /// </summary>
        double DailyLimit { get; }

        /// <summary>
        /// Monthly Deposit Limit
        /// </summary>
        double MonthlyLimit { get; }

        /// <summary>
        /// Maximum Deposit
        /// </summary>
        double MaximumDeposit { get; }
    }
}
