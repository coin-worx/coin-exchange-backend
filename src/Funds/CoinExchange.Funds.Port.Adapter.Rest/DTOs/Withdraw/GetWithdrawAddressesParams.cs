using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    /// <summary>
    /// Parameters for getting the withdraw addresses of the given currency and Account ID
    /// </summary>
    public class GetWithdrawAddressesParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetWithdrawAddressesParams(string currency)
        {
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
