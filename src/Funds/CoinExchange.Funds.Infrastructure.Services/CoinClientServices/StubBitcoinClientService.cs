using System;
using System.Collections.Generic;
using System.Threading;
using CoinExchange.Common.Domain.Model;
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

        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;

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
            string transactionId = "transactionId1";
            _transactionList = new List<Tuple<string, string, decimal, string>>();
            _transactionList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress,transactionId, amount, 
                BitcoinConstants.ReceiveCategory));
            if (DepositArrived != null)
            {
                DepositArrived(CurrencyConstants.Btc, _transactionList);
            }
            Thread.Sleep(200);
            if (DepositConfirmed != null)
            {
                DepositConfirmed(transactionId, 7);
            }
            return transactionId;
        }

        public void PopulateCurrencies()
        {
            throw new NotImplementedException();
        }

        public void PopulateServices()
        {
            throw new NotImplementedException();
        }

        public decimal CheckBalance(string currency)
        {
            return 0;
        }

        public double PollingInterval { get; private set; }
        public string Currency { get; private set; }
        public double NewTransactionsInterval { get; private set; }
    }
}
