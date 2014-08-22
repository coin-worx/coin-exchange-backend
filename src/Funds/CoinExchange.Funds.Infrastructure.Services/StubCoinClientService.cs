using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Stub Implementation for the Bitcoin Client Service
    /// </summary>
    public class StubCoinClientService : ICoinClientService
    {
        public event Action DepositArrived;

        public string CreateNewAddress(string currency)
        {
            return Guid.NewGuid().ToString();
        }

        public bool CommitWithdraw(Withdraw withdraw)
        {
            return true;
        }

        public void PopulateCurrencies()
        {
            throw new NotImplementedException();
        }

        public void PopulateServices()
        {
            throw new NotImplementedException();
        }

        public bool DepositMade(string address, string currency, decimal amount)
        {
            return true;
        }

        public decimal CheckBalance(string currency)
        {
            return 0;
        }

        public double PollingInterval { get; private set; }
    }
}
