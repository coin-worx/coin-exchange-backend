using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for querying the Balance for a particular currency and currency
    /// </summary>
    [Repository]
    public class BalanceRepository : NHibernateSessionFactory, IBalanceRepository
    {
        /// <summary>
        /// Gets the balance given the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Balance GetBalanceById(int id)
        {
            return CurrentSession.QueryOver<Balance>().Where(x => x.BalanceId == id).SingleOrDefault();
        }

        /// <summary>
        /// Gets the balance given the name of the currency and the account ID
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [Transaction]
        public Balance GetBalanceByCurrencyAndAccountId(Currency currency, AccountId accountId)
        {
            return CurrentSession
                .QueryOver<Balance>()
                .Where(x => x.Currency.Name == currency.Name && x.AccountId.Value == accountId.Value)
                .SingleOrDefault();
        }
    }
}
