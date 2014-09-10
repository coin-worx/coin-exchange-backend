using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Validates if the user has the permission to deposit/Withdraw the given type of currency
    /// </summary>
    public class TierValidationService : ITierValidationService
    {
        /// <summary>
        /// Returns true if the user is allowed to perform the transaction on the given currency type
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="isCrypto"> </param>
        /// <returns></returns>
        public bool IsTierVerified(string tierLevel, bool isCrypto)
        {
            switch (tierLevel)
            {
                case TierConstants.TierLevel0:
                    // No transactions of any kind are allowed on Tier Level 0
                    return false;
                case TierConstants.TierLevel1:
                    if(isCrypto)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case TierConstants.TierLevel2:
                    // Both Fiat and Crypto currenceis are allowed on Tier Level 2
                    return true;
                case TierConstants.TierLevel3:
                    // Both Fiat and Crypto currenceis are allowed on Tier Level 2
                    return true;
                default:
                    return false;
            }
        }
    }
}
