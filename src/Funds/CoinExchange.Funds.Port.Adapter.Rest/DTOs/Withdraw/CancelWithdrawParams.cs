using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    /// <summary>
    /// Parameters to cancel withdraw
    /// </summary>
    public class CancelWithdrawParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CancelWithdrawParams(string withdrawId)
        {
            WithdrawId = withdrawId;
        }

        /// <summary>
        /// WithdrawId
        /// </summary>
        public string WithdrawId;
    }
}
