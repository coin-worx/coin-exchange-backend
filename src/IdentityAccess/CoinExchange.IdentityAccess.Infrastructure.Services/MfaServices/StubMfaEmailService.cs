using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// StubMfaEmailService
    /// </summary>
    public class StubMfaEmailService : IMfaCodeSenderService
    {
        public bool SendCode(string address, string code)
        {
            return true;
        }
    }
}
