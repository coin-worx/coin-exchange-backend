using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.BalanceService.Representations
{
    /// <summary>
    /// Representation of Balance Details
    /// </summary>
    public class BalanceDetails
    {
        public string Currency { get; private set; }
        public decimal Balance { get; private set; }

        /// <summary>
        /// Argument Constructor
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="balance"></param>
        public BalanceDetails(string currency, decimal balance)
        {
            Currency = currency;
            Balance = balance;
        }
    }
}
