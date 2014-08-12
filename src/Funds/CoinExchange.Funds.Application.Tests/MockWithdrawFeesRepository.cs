using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawFeesRepository : IWithdrawFeesRepository
    {
        public WithdrawFees GetWithdrawFeesById(int id)
        {
            throw new NotImplementedException();
        }

        public WithdrawFees GetWithdrawFeesByCurrencyName(string currency)
        {
            return new WithdrawFees(new Currency(currency), 0.1m, 0.001m);
        }
    }
}
