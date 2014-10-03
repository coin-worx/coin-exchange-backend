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
        /// <summary>
        /// Authenticate the user using TFA if it is subscribed for the given action
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentAction"></param>
        /// <param name="mfaCode"></param>
        /// <returns></returns>
        Tuple<bool,string> AuthorizeAccess(int userId, string currentAction, string mfaCode);
    }
}
