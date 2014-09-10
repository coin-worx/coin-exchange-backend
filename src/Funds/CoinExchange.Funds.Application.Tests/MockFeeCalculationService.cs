using System;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockFeeCalculationService : IFeeCalculationService
    {
        public decimal GetFee(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, decimal volume, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}
