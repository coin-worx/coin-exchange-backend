using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mock MFA Code Generation Service
    /// </summary>
    public class MockMfaCodeGenerationService : IMfaCodeGenerationService
    {
        public string GenerateCode()
        {
            return "123";
        }
    }
}
