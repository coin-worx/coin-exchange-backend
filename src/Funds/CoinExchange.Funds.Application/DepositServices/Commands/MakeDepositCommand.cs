using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.DepositServices.Commands
{
    /// <summary>
    /// Command for making a deposit
    /// </summary>
    public class MakeDepositCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MakeDepositCommand(int accountId, string currency, decimal amount)
        {
            AccountId = accountId;
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

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
