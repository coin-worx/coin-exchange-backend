using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.Services
{
    /// <summary>
    /// Generates a unique code to be used for MFA authentication
    /// </summary>
    public interface IMfaCodeGenerationService
    {
        /// <summary>
        /// Generates a new code
        /// </summary>
        /// <returns></returns>
        string GenerateCode();
    }
}
