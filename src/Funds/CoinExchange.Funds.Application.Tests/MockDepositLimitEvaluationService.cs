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
        public bool EvaluateDepositLimit(decimal currentDeposit, IList<Ledger> depositLedgers, DepositLimit depositLimit, 
            decimal bestBidPrice, decimal bestAskPrice)
        {
            throw new NotImplementedException();
        }

        public bool AssignDepositLimits(IList<Ledger> depositLedgers, DepositLimit depositLimit, decimal bestBidPrice, decimal bestAskPrice)
        {
            throw new NotImplementedException();
        }

        public decimal DailyLimit { get; private set; }
        public decimal DailyLimitUsed { get; private set; }
        public decimal MonthlyLimit { get; private set; }
        public decimal MonthlyLimitUsed { get; private set; }
        public decimal MaximumDeposit { get; private set; }
    }
}
