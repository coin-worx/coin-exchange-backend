using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Service to evaluate the Maximum Deposit
    /// </summary>
    public class DepositLimitEvaluationService : IDepositLimitEvaluationService
    {
        /// <summary>
        /// Evaluates if the current deposit transaction is within the maximum deposit limit and is allowed to proceed
        /// </summary>
        /// <param name="currentDepositAmount"> </param>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBidPrice"></param>
        /// <param name="bestAskPrice"></param>
        /// <returns></returns>
        public bool EvaluateDepositLimit(double currentDepositAmount, IList<Ledger> depositLedgers, DepositLimit depositLimit, double bestBidPrice,
            double bestAskPrice)
        {
            throw new NotImplementedException();
        }

        public double DailyLimit { get; private set; }
        public double MonthlyLimit { get; private set; }
        public double MaximumDeposit { get; private set; }
    }
}
