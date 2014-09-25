using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockBitcoinClientService : ICoinClientService
    {
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;
        public string CreateNewAddress()
        {
            return "newaddress123";
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
            return "withdrawid123";
        }

        public decimal CheckBalance(string currency)
        {
            return 10;
        }

        public void RaiseDepositArrivedEvent()
        {
            if (DepositArrived != null)
            {
                DepositArrived(null, null);
            }
        }

        public void RaiseDepositConfirmedEvent()
        {
            if (DepositConfirmed != null)
            {
                DepositConfirmed(null, 0);
            }
        }

        public double PollingInterval { get; private set; }
        public string Currency { get; private set; }
        public double NewTransactionsInterval { get; private set; }
    }
}
