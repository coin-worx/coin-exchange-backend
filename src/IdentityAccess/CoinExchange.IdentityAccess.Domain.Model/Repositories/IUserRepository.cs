using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Domain.Model.Repositories
{
    /// <summary>
    /// User reading repository
    /// </summary>
    public interface IUserRepository
    {
        User GetUserByUserName(string username);
    }
}
