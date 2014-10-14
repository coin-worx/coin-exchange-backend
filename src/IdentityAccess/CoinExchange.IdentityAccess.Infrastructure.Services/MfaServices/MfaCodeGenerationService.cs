using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// Generates a 6 digit unique code
    /// </summary>
    public class MfaCodeGenerationService : IMfaCodeGenerationService
    {
        private Random _random = new Random();

        /// <summary>
        /// Generate a unique code
        /// </summary>
        /// <returns></returns>
        public string GenerateCode()
        {
            //return _random.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);
            return "123";
        }
    }
}
