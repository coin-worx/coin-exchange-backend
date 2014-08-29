using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    /// <summary>
    /// Parameters for getting the withdraw addresses for the given account ID and currency
    /// </summary>
    public class CommitWithdrawParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CommitWithdrawParams(string currency, bool isCryptoCurrency, string bitcoinAddress, decimal amount)
        {
            Currency = currency;
            IsCryptoCurrency = isCryptoCurrency;
            BitcoinAddress = bitcoinAddress;
            Amount = amount;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency;

        /// <summary>
        /// Is Crypto Currency
        /// </summary>
        public bool IsCryptoCurrency;

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress;

        /// <summary>
        /// Description
        /// </summary>
        public decimal Amount;
    }
}
