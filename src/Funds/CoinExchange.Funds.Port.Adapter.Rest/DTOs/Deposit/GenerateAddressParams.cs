using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit
{
    /// <summary>
    /// Parameters for generating a new address for deposit
    /// </summary>
    public class GenerateAddressParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GenerateAddressParams(int accountId, string currency)
        {
            AccountId = accountId;
            Currency = currency;
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
