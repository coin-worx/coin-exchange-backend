using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// StubMfaCodeGenerationService
    /// </summary>
    public class StubMfaCodeGenerationService : IMfaCodeGenerationService
    {
        public string GenerateCode()
        {
            return "123";
        }
    }
}
