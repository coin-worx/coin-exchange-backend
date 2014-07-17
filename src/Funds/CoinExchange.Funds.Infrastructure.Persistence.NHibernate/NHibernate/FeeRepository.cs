using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    /// <summary>
    /// Repository for persisteing Fee objects
    /// </summary>
    public class FeeRepository : NHibernateSessionFactory, IFeeRepository
    {
        /// <summary>
        /// Gets the Fee by providing the CurrencyPair name
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        [Transaction]
        public Fee GetFeeByCurrencyPair(string currencyPair)
        {
            return CurrentSession.QueryOver<Fee>().Where(x => x.CurrencyPair == currencyPair).SingleOrDefault();
        }

        /// <summary>
        /// Gets the Fee by specifying the database primary key ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transaction]
        public Fee GetFeeById(int id)
        {
            return CurrentSession.QueryOver<Fee>().Where(x => x.Id == id).SingleOrDefault();
        }
    }
}
