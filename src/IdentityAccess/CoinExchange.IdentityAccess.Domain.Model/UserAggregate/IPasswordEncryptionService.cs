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

        /// <summary>
        /// Verifies if the password entered by the user will become the same as the hashed password after hashing, returns 
        /// true if yes
        /// </summary>
        /// <param name="enteredPassword"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        bool VerifyPassword(string enteredPassword, string hashedPassword);
    }
}
