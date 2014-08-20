
using System;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for interacting with the Bitcoin Client for deposits and withdrawals
    /// </summary>
    public interface ICoinClientService
    {
        /// <summary>
        /// Invoked when a new deposit is received
        /// </summary>
        event Action DepositArrived;

        /// <summary>
        /// Creates a new address using the Coin Client,for wither the Deposit or Withdraw
        /// </summary>
        /// <returns></returns>
        string CreateNewAddress(string currency);

        // ToDo: If withdraw can be confirmed as soon as it is accepted by the Client Service, then this method will call
        // the IFundsValidationService to update the balance
        /// <summary>
        /// Receives Withdraw, forwards to Bitcoin Client to proceed. Returns true if withdraw made
        /// </summary>
        /// <returns></returns>
        bool CommitWithdraw(WithdrawAggregate.Withdraw withdraw);

        /// <summary>
        /// Handles the deposit from the Bitcoin Client, creates new Deposit instance if not already present, if already 
        /// present, forwards to the FundsValidationService to verify enough confirmations
        /// </summary>
        /// <returns></returns>
        bool DepositMade(string address, string currency, decimal amount);

        /// <summary>
        /// Check the balance for the wallet held by the Exchange
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        decimal CheckBalance(string currency);
    }
}
