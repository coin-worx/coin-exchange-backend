using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockWithdrawRepository : IWithdrawRepository
    {
        public Withdraw GetWithdrawById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByAccountId(AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByCurrencyName(string currency)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByCurrencyAndAccountId(string currency, AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public Withdraw GetWithdrawByWithdrawId(string withdrawId)
        {
            throw new NotImplementedException();
        }

        public Withdraw GetWithdrawByTransactionId(TransactionId transactionId)
        {
            throw new NotImplementedException();
        }

        public List<Withdraw> GetWithdrawByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }
    }
}
