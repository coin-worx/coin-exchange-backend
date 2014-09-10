using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Ledgers
{
    /// <summary>
    /// Parameters for getting the ledger for the given currency
    /// </summary>
    public class GetLedgersParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetLedgersParams(string currency)
        {
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }
    }
}
