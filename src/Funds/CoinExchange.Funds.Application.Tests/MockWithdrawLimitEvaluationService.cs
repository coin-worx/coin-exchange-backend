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
        public bool EvaluateMaximumWithdraw()
        {
            throw new NotImplementedException();
        }

        public bool EvaluateMaximumWithdrawLimit(double withdrawalAmountUsd, IList<Ledger> depositLedgers, WithdrawLimit withdrawLimit, double bestBidPrice, double bestAskPrice, double balance, double pendingBalance)
        {
            throw new NotImplementedException();
        }

        public double DailyLimit { get; private set; }
        public double DailyLimitUsed { get; private set; }
        public double MonthlyLimit { get; private set; }
        public double MonthlyLimitUsed { get; private set; }
        public double WithheldAmount { get; private set; }
        public double WithheldConverted { get; private set; }
        public double MaximumWithdraw { get; private set; }
        public double MaximumWithdrawUsd { get; private set; }
    }
}
