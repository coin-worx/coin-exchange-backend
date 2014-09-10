using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repsitory for persisting the the Withdraw Fees
    /// </summary>
    public class WithdrawFeesRepository : NHibernateSessionFactory, IWithdrawFeesRepository
    {
        /// <summary>
        /// Gets the Withdraw Fees by providing the Currency Name
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Transaction]
        public WithdrawFees GetWithdrawFeesByCurrencyName(string currency)
        {
            return CurrentSession.QueryOver<WithdrawFees>().Where(x => x.Currency.Name == currency).SingleOrDefault();
        }

        /// <summary>
        /// Gets the withdraw limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transaction]
        public WithdrawFees GetWithdrawFeesById(int id)
        {
            return CurrentSession.QueryOver<WithdrawFees>().Where(x => x.Id == id).SingleOrDefault();
        }
    }
}
