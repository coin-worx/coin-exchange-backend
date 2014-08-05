using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawLimitEvaluationService : IWithdrawLimitEvaluationService
    {
        public bool EvaluateMaximumWithdraw()
        {
            throw new NotImplementedException();
        }

        public double MaximumWithdraw { get; private set; }
    }
}
