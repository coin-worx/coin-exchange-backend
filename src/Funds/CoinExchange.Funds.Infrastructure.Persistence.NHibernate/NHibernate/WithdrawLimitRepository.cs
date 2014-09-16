using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for saving Withdraw Limits
    /// </summary>
    [Repository]
    public class WithdrawLimitRepository : NHibernateSessionFactory, IWithdrawLimitRepository
    {
        /// <summary>
        /// Gets the Withdraw Limit by Tier Level
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <returns></returns>
        [Transaction]
        public WithdrawLimit GetWithdrawLimitByTierLevel(string tierLevel)
        {
            return CurrentSession.QueryOver<WithdrawLimit>().Where(x => x.TierLevel == tierLevel).SingleOrDefault();
        }

        /// <summary>
        /// Get the Withdrwaa limit by currency type and tier level
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        [Transaction]
        public WithdrawLimit GetLimitByTierLevelAndCurrency(string tierLevel, string currencyType)
        {
            return CurrentSession.QueryOver<WithdrawLimit>().Where(x => x.TierLevel == tierLevel
                && x.LimitsCurrency == currencyType).SingleOrDefault();
        }

        /// <summary>
        /// Gets the withdraw limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transaction]
        public WithdrawLimit GetWithdrawLimitById(int id)
        {
            return CurrentSession.QueryOver<WithdrawLimit>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public IList<WithdrawLimit> GetAllWithdrawLimits()
        {
            return CurrentSession.Query<WithdrawLimit>()
                .AsQueryable()
                .OrderBy(x => x.Id)
                .ToList();
        }
    }
}
