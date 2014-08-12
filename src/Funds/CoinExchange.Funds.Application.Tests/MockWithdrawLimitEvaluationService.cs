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

        public bool EvaluateMaximumWithdrawLimit(decimal withdrawalAmountUsd, IList<Ledger> depositLedgers, WithdrawLimit withdrawLimit, decimal bestBidPrice, decimal bestAskPrice, decimal balance, decimal pendingBalance)
        {
            throw new NotImplementedException();
        }

        public decimal DailyLimit { get; private set; }
        public decimal DailyLimitUsed { get; private set; }
        public decimal MonthlyLimit { get; private set; }
        public decimal MonthlyLimitUsed { get; private set; }
        public decimal WithheldAmount { get; private set; }
        public decimal WithheldConverted { get; private set; }
        public decimal MaximumWithdraw { get; private set; }
        public decimal MaximumWithdrawUsd { get; private set; }
    }
}
