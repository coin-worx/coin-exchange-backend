using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Services;

namespace CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate
{
    public class StubFundsApiKeyAccess : NHibernateSessionFactory, IApiKeyInfoAccess
    {
        public int GetUserIdFromApiKey(string apiKey)
        {
            return 1;
        }
    }
}
