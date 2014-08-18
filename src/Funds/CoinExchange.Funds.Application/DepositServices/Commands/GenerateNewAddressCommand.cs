using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.DepositServices.Commands
{
    /// <summary>
    /// Command for generatin new address
    /// </summary>
    public class GenerateNewAddressCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GenerateNewAddressCommand(int accountId)
        {
            AccountId = accountId;
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
