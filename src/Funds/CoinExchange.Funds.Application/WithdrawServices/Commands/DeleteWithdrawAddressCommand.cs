using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Commands
{
    /// <summary>
    /// Command to delete the given withdraw address form the database
    /// </summary>
    public class DeleteWithdrawAddressCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DeleteWithdrawAddressCommand(int accountId, string bitcoinAddress)
        {
            AccountId = accountId;
            BitcoinAddress = bitcoinAddress;
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress { get; private set; }
    }
}
