using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Commands
{
    /// <summary>
    /// Command to cancel the withdraw with the given withdrawId
    /// </summary>
    public class CancelWithdrawCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CancelWithdrawCommand(string withdrawId)
        {
            WithdrawId = withdrawId;
        }

        /// <summary>
        /// WithdrawId
        /// </summary>
        public string WithdrawId { get; private set; }
    }
}
