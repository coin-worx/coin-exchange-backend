using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Service to keep track of the submitted withdrawals, and wait handler to place withdrawals on the network after the specified
    /// interval elapses
    /// </summary>
    public interface IWithdrawSubmissionService
    {
        event Action<Withdraw> WithdrawExecuted;

        /// <summary>
        /// Saves the withdraw in database and memory to be submitted after the specified time interval
        /// </summary>
        /// <param name="withdraw"></param>
        /// <returns></returns>
        bool CommitWithdraw(string withdraw);

        /// <summary>
        /// Cancels the withdraw with the given Withdraw ID
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        bool CancelWithdraw(string withdrawId);
    }
}
