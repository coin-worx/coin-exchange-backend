using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit
{
    /// <summary>
    /// Parameters for getting deposit addresses
    /// </summary>
    public class GetDepositAddressesParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetDepositAddressesParams(int accountId)
        {
            AccountId = accountId;
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }
    }
}
