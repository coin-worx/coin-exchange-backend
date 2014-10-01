using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.Services
{
    /// <summary>
    /// Asynchronously sends the MFA code to the given destination
    /// </summary>
    public interface IMfaCodeSenderService
    {
        bool SendCode(string address, string code);
    }
}
