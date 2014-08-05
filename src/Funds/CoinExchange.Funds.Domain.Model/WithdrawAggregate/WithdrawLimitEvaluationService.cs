using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Service to determine the maximum Withdrawal amount
    /// </summary>
    public class WithdrawLimitEvaluationService : IWithdrawLimitEvaluationService
    {
        /// <summary>
        /// Evaluates the maximum deposit amount and figures if the current withdrawal should be allowed to commence
        /// </summary>
        /// <returns></returns>
        public bool EvaluateMaximumWithdraw()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maximum withdraw amount
        /// </summary>
        public double MaximumWithdraw { get; private set; }
    }
}
