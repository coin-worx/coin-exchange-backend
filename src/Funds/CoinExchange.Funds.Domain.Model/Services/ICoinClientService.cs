
using System;
using System.Collections.Generic;
using System.Timers;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for interacting with the Bitcoin Client for deposits and withdrawals
    /// </summary>
    public interface ICoinClientService
    {
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
        /// Creates a new address using the Coin Client,for wither the Deposit or Withdraw
        /// </summary>
        /// <returns></returns>
        string CreateNewAddress();

        /// <summary>
        /// Receives Withdraw, forwards to Bitcoin Client to proceed. Returns Transaction ID if commit is made
        /// </summary>
        /// <returns></returns>
        string CommitWithdraw(string bitcoinAddress, decimal amount);

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
