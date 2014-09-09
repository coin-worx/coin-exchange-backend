using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for querying Ledger objects
    /// </summary>
    [Repository]
    public class LedgerRepository : NHibernateSessionFactory, ILedgerRepository
    {
        [Transaction]
        public Ledger GetLedgerById(int id)
        {
            return CurrentSession.QueryOver<Ledger>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<Ledger> GetLedgerByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public List<Ledger> GetLedgerByCurrencyName(string currency)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.Currency.Name == currency)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public IList<Ledger> GetLedgerByAccountIdAndCurrency(string currency, AccountId accountId)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.Currency.Name == currency && x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        /// <summary>
        /// Gets the balance for a currency for the specified Account ID
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public decimal GetBalanceForCurrency(string currency, AccountId accountId)
        {
            List<Ledger> ledgers = CurrentSession.Query<Ledger>().
                Where(x => x.Currency.Name == currency && x.AccountId.Value == accountId.Value).
                AsQueryable().
                OrderBy(x => x.DateTime).
                ToList();

            if (ledgers.Any())
            {
                return ledgers.Last().Balance;
            }
            return 0;
        }

        [Transaction]
        public IList<Ledger> GetAllLedgers(int accountId)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.AccountId.Value == accountId)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public Ledger GetLedgerByLedgerId(string ledgerId)
        {
            return CurrentSession.QueryOver<Ledger>().Where(x => x.LedgerId == ledgerId).SingleOrDefault();
        }

        [Transaction]
        public List<Ledger> GetLedgersByTradeId(string tradeId)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.TradeId == tradeId)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public Ledger GetLedgersByDepositId(string depositId)
        {
            return CurrentSession.QueryOver<Ledger>().Where(x => x.DepositId == depositId).SingleOrDefault();
        }

        [Transaction]
        public Ledger GetLedgersByWithdrawId(string withdrawId)
        {
            return CurrentSession.QueryOver<Ledger>().Where(x => x.WithdrawId == withdrawId).SingleOrDefault();
        }

        [Transaction]
        public List<Ledger> GetLedgersByOrderId(string orderId)
        {
            return CurrentSession.Query<Ledger>()
                .Where(x => x.OrderId == orderId)
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction]
        public IList<Ledger> GetAllLedgers()
        {
            return CurrentSession.QueryOver<Ledger>().List<Ledger>();
        }
    }
}
