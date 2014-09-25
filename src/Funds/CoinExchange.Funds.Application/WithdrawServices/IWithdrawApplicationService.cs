using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.WithdrawServices.Commands;
using CoinExchange.Funds.Application.WithdrawServices.Representations;

namespace CoinExchange.Funds.Application.WithdrawServices
{
    /// <summary>
    /// Interface for dealing with operations and queries about withdraws
    /// </summary>
    public interface IWithdrawApplicationService
    {
        /// <summary>
        /// Get recent withdrawals for the given currency and account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<WithdrawRepresentation> GetRecentWithdrawals(int accountId); 

        /// <summary>
        /// Get recent withdrawals for the given currency and account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        List<WithdrawRepresentation> GetRecentWithdrawals(int accountId, string currency); 

        /// <summary>
        /// Adds a new Bitcoin address with description
        /// </summary>
        /// <returns></returns>
        WithdrawAddressResponse AddAddress(AddAddressCommand addAddressCommand);

        /// <summary>
        /// Get the list of all the withdrawawl addresses associated with this account for this currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<WithdrawAddressRepresentation> GetWithdrawalAddresses(int accountId, string currency);

        /// <summary>
        /// Commits a withdraw on the user's request
        /// </summary>
        /// <param name="commitWithdrawCommand"></param>
        /// <returns></returns>
        CommitWithdrawResponse CommitWithdrawal(CommitWithdrawCommand commitWithdrawCommand);

        /// <summary>
        /// Retreives the Withdraw Limits for the given account and currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        WithdrawLimitRepresentation GetWithdrawLimitThresholds(int accountId, string currency);

        /// <summary>
        /// Deletes the given bitcoin address from the database
        /// </summary>
        /// <param name="deleteWithdrawAddressCommand"></param>
        /// <returns></returns>
        DeleteWithdrawAddressResponse DeleteAddress(DeleteWithdrawAddressCommand deleteWithdrawAddressCommand);

        /// <summary>
        /// Cancels the withdraw from being sent to the network. Returns the boolean result and the message as a tuple
        /// </summary>
        /// <param name="cancelWithdrawCommand"></param>
        /// <returns></returns>
        CancelWithdrawResponse CancelWithdraw(CancelWithdrawCommand cancelWithdrawCommand);

        /// <summary>
        /// Get the TIer limits for Withdraw
        /// </summary>
        /// <returns></returns>
        WithdrawTierLimitRepresentation GetWithdrawTierLimits();
    }
}
