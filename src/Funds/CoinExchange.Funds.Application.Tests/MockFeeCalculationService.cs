using System;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockFeeCalculationService : IFeeCalculationService
    {
        public double GetFee(Currency baseCurrency, Currency quoteCurrency, double amount)
        {
            throw new NotImplementedException();
        }
    }
}
