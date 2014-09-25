using System;
using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Service to send events to the specific CoinClientServices to perform withdrawal after specified time interval elapse, to 
    /// create new addresses
    /// </summary>
    public interface IClientInteractionService
    {
        /// <summary>
        /// Invoked when withdrawal gets submitted successfully to the network
        /// </summary>
        event Action<Withdraw> WithdrawExecuted;

        /// <summary>
        /// Invoked when a new deposit is received, Param1 = Currency, 
        /// Param2 = List(Tuple): Item1 = BitcoinAddress, Item2 = TransacitonId, Item3 = Amount, Item4 = Category
        /// </summary>
        event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;

        /// <summary>
        /// Invoked when a Deposit is confirmed. Param1 = TransactionId, Param2 = No. of Confirmations
        /// </summary>
        event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Saves the withdraw in database and memory to be submitted after the specified time interval
        /// </summary>
        /// <param name="withdraw"></param>
        /// <returns></returns>
        bool CommitWithdraw(Withdraw withdraw);

        /// <summary>
        /// Cancels the withdraw with the given Withdraw ID
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        bool CancelWithdraw(string withdrawId);

        /// <summary>
        /// Generate New Bitcoin adddress
        /// </summary>
        /// <returns></returns>
        string GenerateNewAddress(string currency);

        /// <summary>
        /// The interval after which a withdraw is submitted to the network
        /// </summary>
        double WithdrawSubmissionInterval { get; }
    }
}
