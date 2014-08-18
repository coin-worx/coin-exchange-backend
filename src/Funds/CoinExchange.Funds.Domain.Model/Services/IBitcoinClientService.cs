using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for interacting with the Bitcoin Client for deposits and withdrawals
    /// </summary>
    public interface IBitcoinClientService
    {
        /// <summary>
        /// Creates a new address using the Bitcoin Client
        /// </summary>
        /// <returns></returns>
        string CreateNewAddress();

        // ToDo: Research and figure out how to make the withdrawals and receive deposits using the Bitcoin Client and
        // reflect thos changes here in this service
        /// <summary>
        /// Makes a withdrawal
        /// </summary>
        /// <returns></returns>
        bool MakeWithdrawal(string address, string currency, decimal amount);

        /// <summary>
        /// Handles the deposit
        /// </summary>
        /// <returns></returns>
        bool DepositMade(string currency, decimal amount);
    }
}
