using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    /// <summary>
    /// parameters for getting hte withdraw limits for a currency
    /// </summary>
    public class GetWithdrawLimitsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetWithdrawLimitsParams(string currency)
        {
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
