using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Stub Implementation for the Bitcoin Client Service
    /// </summary>
    public class StubBitcoinClientService : IBitcoinClientService
    {
        public string CreateNewAddress()
        {
            return Guid.NewGuid().ToString();
        }

        public bool MakeWithdrawal(string address, string currency, decimal amount)
        {
            return true;
        }

        public bool DepositMade(string currency, decimal amount)
        {
            return true;
        }
    }
}
