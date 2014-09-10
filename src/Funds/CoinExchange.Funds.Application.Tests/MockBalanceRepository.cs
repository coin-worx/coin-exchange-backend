using System;
using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockBalanceRepository : IBalanceRepository
    {
        private List<Balance> _balanceList = new List<Balance>();

        public Balance GetBalanceById(int id)
        {
            throw new NotImplementedException();
        }

        public Balance GetBalanceByCurrencyAndAccountId(Currency currency, AccountId accountId)
        {
            foreach (var balance1 in _balanceList)
            {
                if (balance1.AccountId.Value == accountId.Value && balance1.Currency.Name == currency.Name)
                {
                    return balance1;
                }
            }
            return null;
        }

        public bool AddBalance(Balance balance)
        {
            _balanceList.Add(balance);
            return true;
        }
    }
}
