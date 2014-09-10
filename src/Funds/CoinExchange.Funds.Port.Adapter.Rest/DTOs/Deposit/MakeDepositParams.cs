using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit
{
    /// <summary>
    /// Parameters for making a deposit
    /// </summary>
    public class MakeDepositParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MakeDepositParams(string currency, decimal amount, bool isCryptoCurrency)
        {
            Currency = currency;
            Amount = amount;
            IsCryptoCurrency = isCryptoCurrency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// is the currency crypto currency of fiat
        /// </summary>
        public bool IsCryptoCurrency { get; private set; }
    }
}
