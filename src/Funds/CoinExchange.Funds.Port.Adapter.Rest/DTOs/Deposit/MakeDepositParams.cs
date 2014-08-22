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
        public MakeDepositParams(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }
    }
}
