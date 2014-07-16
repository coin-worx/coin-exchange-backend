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
    /// Repository for querying Deposit Bitcion addresses
    /// </summary>
    [Repository]
    public class DepositAddressRepository : NHibernateSessionFactory, IDepositAddressRepository
    {
        [Transaction]
        public DepositAddress GetDepositAddressById(int id)
        {
            return CurrentSession.QueryOver<DepositAddress>().Where(x => x.Id == id).SingleOrDefault();
        }

        [Transaction]
        public List<DepositAddress> GetDepositAddressByAccountId(AccountId accountId)
        {
            return CurrentSession.Query<DepositAddress>()
                .Where(x => x.AccountId.Value == accountId.Value)
                .AsQueryable()
                .OrderByDescending(x => x.CreationDateTime)
                .ToList();
        }
    }
}
