using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        bool EvaluateMaximumWithdraw();

        /// <summary>
        /// The maximum amount that can be withdrawn
        /// </summary>
        double MaximumWithdraw { get; }
    }
}
