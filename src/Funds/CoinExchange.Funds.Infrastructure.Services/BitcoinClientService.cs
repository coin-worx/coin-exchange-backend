using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Service for interacting with the Bitcoin Client
    /// </summary>
    public class BitcoinClientService : IBitcoinClientService
    {
        public string CreateNewAddress()
        {
            throw new NotImplementedException();
        }

        public bool MakeWithdrawal(string address, string currency, decimal amount)
        {
            throw new NotImplementedException();
        }

        public bool DepositMade(string currency, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
