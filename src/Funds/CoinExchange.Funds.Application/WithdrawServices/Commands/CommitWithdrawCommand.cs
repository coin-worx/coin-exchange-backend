using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Commands
{
    /// <summary>
    /// Command to withdraw
    /// </summary>
    public class CommitWithdrawCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CommitWithdrawCommand(int accountId, string currency, bool isCryptoCurrency, string bitcoinAddress, decimal amount)
        {
            AccountId = accountId;
            Currency = currency;
            IsCryptoCurrency = isCryptoCurrency;
            BitcoinAddress = bitcoinAddress;
            Amount = amount;
        }

        /// <summary>
        /// AccountID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// IsCryptoCurrency
        /// </summary>
        public bool IsCryptoCurrency { get; private set; }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress { get; private set; }

        /// <summary>
        /// Amount to be withdrawn
        /// </summary>
        public decimal Amount { get; private set; }
    }
}
