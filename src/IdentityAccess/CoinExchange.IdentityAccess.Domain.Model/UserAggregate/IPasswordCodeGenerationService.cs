using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Generates a unique for for cases of forgotten password
    /// </summary>
    public interface IPasswordCodeGenerationService
    {
        string CreateNewForgotPasswordCode();
    }
}
