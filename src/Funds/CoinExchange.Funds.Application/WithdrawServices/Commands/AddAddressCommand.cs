using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Commands
{
    /// <summary>
    /// Addsa new address to the user account
    /// </summary>
    public class AddAddressCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AddAddressCommand(int accountId, string currency, string bitcoinAddress, string description)
        {
            AccountId = accountId;
            Currency = currency;
            BitcoinAddress = bitcoinAddress;
            Description = description;
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
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
