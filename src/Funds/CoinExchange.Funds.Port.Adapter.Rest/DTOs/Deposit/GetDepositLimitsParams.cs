using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit
{
    /// <summary>
    /// Parameters for getting the limits of deposit for a currency
    /// </summary>
    public class GetDepositLimitsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetDepositLimitsParams(string currency)
        {
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
