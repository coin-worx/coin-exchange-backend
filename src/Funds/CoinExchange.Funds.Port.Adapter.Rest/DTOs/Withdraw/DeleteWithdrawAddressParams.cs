using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    public class DeleteWithdrawAddressParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DeleteWithdrawAddressParams(string bitcoinAddress)
        {
            BitcoinAddress = bitcoinAddress;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress { get; private set; }
    }
}
