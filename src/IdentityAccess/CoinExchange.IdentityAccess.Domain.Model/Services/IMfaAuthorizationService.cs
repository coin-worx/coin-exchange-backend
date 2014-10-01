using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Domain.Model.Services
{
    /// <summary>
    /// Validates and authorizes MFA for a user
    /// </summary>
    public interface IMfaAuthorizationService
    {
        bool AuthorizeAccess(string currentAction, string mfaCode);
    }
}
