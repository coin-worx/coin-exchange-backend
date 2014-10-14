using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Stub implementation
    /// </summary>
    public class StubDogecoinClientService : ICoinClientService
    {
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

        public string CreateNewAddress()
        {
            return Guid.NewGuid().ToString();
        }

        public void CheckNewTransactions()
        {
            throw new NotImplementedException();
        }

        public void PollConfirmations()
        {
            throw new NotImplementedException();
        }

        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            if (DepositArrived != null)
            {
                DepositArrived(CurrencyConstants.Doge, null);
            }
            Thread.Sleep(200);
            if (DepositConfirmed != null)
            {
                DepositConfirmed("transactionid1", 7);
            }
            return "transactionid1";
        }

        public decimal CheckBalance(string currency)
        {
            return 10;
        }

        public double PollingInterval { get; private set; }
        public string Currency { get; private set; }
        public double NewTransactionsInterval { get; private set; }
    }
}
