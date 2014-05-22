using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Password Encryption interface
    /// </summary>
    public interface IPasswordEncryptionService
    {
        /// <summary>
        /// Accepts original string password and returns the encrypted password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string EncryptPassword(string password);
    }
}
