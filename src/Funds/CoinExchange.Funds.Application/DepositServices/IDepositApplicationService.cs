using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;

namespace CoinExchange.Funds.Application.DepositServices
{
    /// <summary>
    /// Interface for the Deposit Application service
    /// </summary>
    public interface IDepositApplicationService
    {
        /// <summary>
        /// Gets new address from the BitcoinD Service for making a deposit
        /// </summary>
        /// <returns></returns>
        DepositAddressRepresentation GenarateNewAddress(GenerateNewAddressCommand generateNewAddressCommand);

        /// <summary>
        /// Get the threshold limits of deposit for this user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        DepositLimitRepresentation GetThresholdLimits(int accountId, string currency);

        /// <summary>
        /// Get the list of addresses for the current user
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        IList<DepositAddressRepresentation> GetAddressesForAccount(int accountId);
    }
}
