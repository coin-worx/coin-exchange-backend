using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawLimitEvaluationService : IWithdrawLimitEvaluationService
    {
        public bool EvaluateMaximumWithdrawLimit(decimal withdrawalAmount, IList<Withdraw> depositLedgers, WithdrawLimit withdrawLimit, decimal balance, decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0)
        {
            throw new NotImplementedException();
        }

        public bool AssignWithdrawLimits(IList<Withdraw> depositLedgers, WithdrawLimit withdrawLimit, decimal balance, decimal currentBalance, decimal bestBid = 0, decimal bestAsk = 0)
        {
            throw new NotImplementedException();
        }

        public decimal DailyLimit { get; private set; }
        public decimal DailyLimitUsed { get; private set; }
        public decimal MonthlyLimit { get; private set; }
        public decimal MonthlyLimitUsed { get; private set; }
        public decimal WithheldAmount { get; private set; }
        public decimal MaximumWithdraw { get; private set; }
    }
}
