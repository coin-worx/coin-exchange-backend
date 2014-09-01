using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    public class CommitWithdrawResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CommitWithdrawResponse(bool commitSuccessful, string withdrawId, string description)
        {
            WithdrawId = withdrawId;
            CommitSuccessful = commitSuccessful;
            Description = description;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public bool CommitSuccessful { get; private set; }

        /// <summary>
        /// Withdraw ID
        /// </summary>
        public string WithdrawId { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
