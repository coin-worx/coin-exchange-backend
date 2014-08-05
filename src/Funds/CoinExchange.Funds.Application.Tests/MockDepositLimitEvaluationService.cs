using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockDepositLimitEvaluationService : IDepositLimitEvaluationService
    {
        public bool EvaluateDepositLimit(double currentDeposit, IList<Ledger> depositLedgers, DepositLimit depositLimit, double bestBidPrice, double bestAskPrice)
        {
            throw new NotImplementedException();
        }

        public double DailyLimit { get; private set; }
        public double MonthlyLimit { get; private set; }
        public double MaximumDeposit { get; private set; }
    }
}
