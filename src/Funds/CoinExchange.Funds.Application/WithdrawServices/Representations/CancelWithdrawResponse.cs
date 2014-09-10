using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Response after CancelWithdraw request
    /// </summary>
    public class CancelWithdrawResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CancelWithdrawResponse(bool commitSuccessful)
        {
            CancelSuccessful = commitSuccessful;
        }

        /// <summary>
        /// Cancel Successful or not
        /// </summary>
        public bool CancelSuccessful { get; private set; }
    }
}
