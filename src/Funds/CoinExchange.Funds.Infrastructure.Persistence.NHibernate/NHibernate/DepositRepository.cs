using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
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
        public Deposit GetDepositById(string depositId)
        {
            return CurrentSession.Get<Deposit>(depositId);
        }

        [Transaction]
        public Deposit GetDepositByCurrencyName(string currency)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.Currency.Name == currency).SingleOrDefault();
        }

        [Transaction]
        public Deposit GetDepositByDepositId(string depositId)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.DepositId == depositId).SingleOrDefault();
        }

        [Transaction]
        public Deposit GetDepositByDate(DateTime dateTime)
        {
            return CurrentSession.QueryOver<Deposit>().Where(x => x.Date == dateTime).SingleOrDefault();
        }
    }
}
