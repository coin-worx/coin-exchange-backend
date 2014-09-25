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
        bool EvaluateMaximumWithdrawLimit(decimal withdrawalAmount, IList<Withdraw> depositLedgers, WithdrawLimit 
                                 withdrawLimit, decimal balance, decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0);

        /// <summary>
        /// Assigns the threshold limits without comparing the current withdrawal amount
        /// </summary>
        /// <param name="depositLedgers"></param>
        /// <param name="withdrawLimit"></param>
        /// <param name="balance"></param>
        /// <param name="currentBalance"></param>
        /// <param name="bestBid"> </param>
        /// <param name="bestAsk"> </param>
        /// <returns></returns>
        bool AssignWithdrawLimits(IList<Withdraw> depositLedgers, WithdrawLimit withdrawLimit, decimal balance, 
                                    decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0);

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
        /// The maximum amount that can be withdrawn
        /// </summary>
        decimal MaximumWithdraw { get; }
    }
}
