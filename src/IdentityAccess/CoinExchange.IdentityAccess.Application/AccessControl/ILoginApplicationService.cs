using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControl
{
    /// <summary>
    /// Interface for operations relted to Login
    /// </summary>
    public interface ILoginApplicationService
    {
        /// <summary>
        /// Requests the login for a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ValidationEssentials Login(string username, string password);
    }
}
