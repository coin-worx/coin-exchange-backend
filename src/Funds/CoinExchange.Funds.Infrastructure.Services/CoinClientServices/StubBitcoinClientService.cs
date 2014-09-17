using System;
using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Stub Implementation for the Bitcoin Client Service
    /// </summary>
    public class StubBitcoinClientService : ICoinClientService
    {
        private List<Tuple<string, string, decimal, string>> _transactionList = new List<Tuple<string, string, decimal, string>>();
        public event Action<string, int> DepositConfirmed;

        event Action<string, List<Tuple<string, string, decimal, string>>> ICoinClientService.DepositArrived
        {
            add { _transactionList.Add(new Tuple<string, string, decimal, string>("","",0,"")); }
            remove { _transactionList.Remove(new Tuple<string, string, decimal, string>("", "", 0, "")); }
        }

        public string CreateNewAddress()
        {
            return Guid.NewGuid().ToString();
        }

        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            return "123";
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
