using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repositroy for querying DepositLimit objects
    /// </summary>
    public class DepositLimitRepository : NHibernateSessionFactory, IDepositLimitRepository
    {
        /// <summary>
        /// Gets the Deposit Limit by Tier Level
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <returns></returns>
        [Transaction]
        public DepositLimit GetDepositLimitByTierLevel(string tierLevel)
        {
            return CurrentSession.QueryOver<DepositLimit>().Where(x => x.TierLevel == tierLevel).SingleOrDefault();
        }

        /// <summary>
        /// Gets the Deposit limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transaction]
        public DepositLimit GetDepositLimitById(int id)
        {
            return CurrentSession.QueryOver<DepositLimit>().Where(x => x.Id == id).SingleOrDefault();
        }
    }
}
