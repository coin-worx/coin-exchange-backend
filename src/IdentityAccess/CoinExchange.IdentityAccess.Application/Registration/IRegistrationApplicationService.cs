using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.Registration
{
    /// <summary>
    /// IRegistrationApplicationService
    /// </summary>
    public interface IRegistrationApplicationService
    {
        /// <summary>
        /// Request from the client to create a new account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        string CreateAccount(string email, string username, string password, string country, TimeZone timeZone,
            string publicKey);
    }
}
