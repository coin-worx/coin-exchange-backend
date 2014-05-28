using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Generates a unique code for cases of forgotten password
    /// </summary>
    public class PasswordCodeGenerationService : IPasswordCodeGenerationService
    {
        /// <summary>
        /// Generates a unique code for the cases of forgotten password
        /// </summary>
        /// <returns></returns>
        public string CreateNewForgotPasswordCode()
        {
            Guid newGuid = Guid.NewGuid();
            return Convert.ToBase64String(newGuid.ToByteArray());
        }
    }
}
