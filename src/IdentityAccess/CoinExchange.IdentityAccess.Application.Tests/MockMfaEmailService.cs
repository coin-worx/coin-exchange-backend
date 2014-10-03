using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mock MFA Email Service
    /// </summary>
    public class MockMfaEmailService : IMfaCodeSenderService
    {
        public bool SendCode(string address, string code)
        {
            return true;
        }
    }
}
