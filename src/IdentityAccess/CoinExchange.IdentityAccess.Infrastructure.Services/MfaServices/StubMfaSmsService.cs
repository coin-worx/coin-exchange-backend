using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// Stub Mfa SMS Service
    /// </summary>
    public class StubMfaSmsService : IMfaCodeSenderService
    {
        public bool SendCode(string address, string code)
        {
            return true;
        }
    }
}
