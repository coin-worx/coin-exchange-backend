using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockDepositRepository : IDepositRepository
    {
        private List<Deposit> _deposits = new List<Deposit>(); 

        public void Save(Deposit deposit)
        {
            _deposits.Add(deposit);
        }

        public Deposit GetDepositById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Deposit> GetDepositByAccountId(AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public List<Deposit> GetDepositByCurrencyName(string currency)
        {
            throw new NotImplementedException();
        }

        public Deposit GetDepositByDepositId(string depositId)
        {
            throw new NotImplementedException();
        }

        public Deposit GetDepositByTransactionId(TransactionId transactionId)
        {
            foreach (var deposit in _deposits)
            {
                if (deposit.TransactionId.Value == transactionId.Value)
                {
                    return deposit;
                }
            }
            return null;
        }

        public List<Deposit> GetDepositsByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        public List<Deposit> GetAllDeposits()
        {
            throw new NotImplementedException();
        }
    }
}
