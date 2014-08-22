
using System;
using System.Timers;

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

        /// <summary>
        /// Receives Withdraw, forwards to Bitcoin Client to proceed. Returns true if withdraw made
        /// </summary>
        /// <returns></returns>
        bool CommitWithdraw(WithdrawAggregate.Withdraw withdraw);

        /// <summary>
        /// Populate the list of tradable currencies
        /// </summary>
        void PopulateCurrencies();

        /// <summary>
        /// Populate all the services that will talk to the crypto currency client daemons
        /// </summary>
        void PopulateServices();

        /// <summary>
        /// Check the balance for the wallet held by the Exchange
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        decimal CheckBalance(string currency);

        /// <summary>
        /// Interval for polling
        /// </summary>
        double PollingInterval { get; }
    }
}
