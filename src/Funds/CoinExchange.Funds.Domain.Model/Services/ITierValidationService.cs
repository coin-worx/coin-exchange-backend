using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Validates if the user has the permission to deposit/Withdraw the given type of currency
    /// </summary>
    public interface ITierValidationService
    {
        /// <summary>
        /// Returns true if the user is allowed to perform the transaction on the given currency type
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="isCrpyto"> </param>
        /// <returns></returns>
        bool IsTierVerified(string tierLevel, bool isCrpyto);
    }
}
