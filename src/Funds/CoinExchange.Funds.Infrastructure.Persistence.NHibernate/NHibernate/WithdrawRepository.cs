using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for withdrawing Withdrawal objects
    /// </summary>
    [Repository]
    public class WithdrawRepository : NHibernateSessionFactory, IWithdrawRepository
    {
        [Transaction]
        public Withdraw GetWithdrawById(int id)
        {
            return CurrentSession.QueryOver<Withdraw>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<Withdraw> GetWithdrawByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<Withdraw>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public List<Withdraw> GetWithdrawByCurrencyName(string currency)
        {
            return CurrentSession.Query<Withdraw>()
                .Where(x => x.Currency.Name == currency)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public List<Withdraw> GetWithdrawByCurrencyAndAccountId(string currency, AccountId accountId)
        {
            return CurrentSession.Query<Withdraw>()
                .Where(x => x.Currency.Name == currency && x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public Withdraw GetWithdrawByWithdrawId(string withdrawId)
        {
            return CurrentSession.QueryOver<Withdraw>().Where(x => x.WithdrawId == withdrawId).SingleOrDefault();
        }

        [Transaction]
        public Withdraw GetWithdrawByTransactionId(TransactionId transactionId)
        {
            return CurrentSession.QueryOver<Withdraw>().Where(x => x.TransactionId.Value == transactionId.Value).SingleOrDefault();
        }

        [Transaction]
        public List<Withdraw> GetWithdrawByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            return CurrentSession.Query<Withdraw>()
                .Where(x => x.BitcoinAddress.Value == bitcoinAddress.Value)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }
    }
}
