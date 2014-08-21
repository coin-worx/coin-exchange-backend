using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for accessing Deposit objects
    /// </summary>
    [Repository]
    public class DepositRepository : NHibernateSessionFactory, IDepositRepository
    {
        [Transaction]
        public Deposit GetDepositById(int id)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<Deposit> GetDepositByCurrencyName(string currency)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.Currency.Name == currency)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public Deposit GetDepositByDepositId(string depositId)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.DepositId == depositId).SingleOrDefault();
        }

        [Transaction]
        public Deposit GetDepositByTransactionId(TransactionId transactionId)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.TransactionId.Value == transactionId.Value).SingleOrDefault();
        }

        [Transaction]
        public List<Deposit> GetDepositsByBitcoinAddress(BitcoinAddress bitcoinAddress)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.BitcoinAddress.Value == bitcoinAddress.Value)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public List<Deposit> GetAllDeposits()
        {
            return CurrentSession.Query<Deposit>()
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        [Transaction]
        public List<Deposit> GetDepositByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<Deposit>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.Date)
                .ToList();
        }
    }
}
