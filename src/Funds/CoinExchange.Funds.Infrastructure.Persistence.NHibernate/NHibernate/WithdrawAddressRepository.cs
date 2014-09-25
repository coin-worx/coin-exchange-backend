using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NHibernate.Linq;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for querying Withdraw address objects
    /// </summary>
    public class WithdrawAddressRepository : NHibernateSessionFactory, IWithdrawAddressRepository
    {
        [Transaction]
        public WithdrawAddress GetWithdrawAddressById(int id)
        {
            return CurrentSession.QueryOver<WithdrawAddress>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<WithdrawAddress> GetWithdrawAddressByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<WithdrawAddress>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.CreationDateTime)
                .ToList();
        }

        [Transaction]
        public List<WithdrawAddress> GetWithdrawAddressByAccountIdAndCurrency(AccountId accountId, Currency currency)
        {
            return CurrentSession.Query<WithdrawAddress>()
                .Where(x => x.AccountId.Value == accountId.Value && x.Currency.Name == currency.Name)
                .AsQueryable()
                .OrderByDescending(x => x.CreationDateTime)
                .ToList();
        }
    }
}
