using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit
{
    /// <summary>
    /// Parameters for getting deposit addresses
    /// </summary>
    public class GetDepositAddressesParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetDepositAddressesParams(string currency)
        {
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
