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
        /// Get recent deposits for a given currency and account ID
        /// </summary>
        /// <returns></returns>
        List<DepositRepresentation> GetRecentDeposits(string currency, int accountId);

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
        DepositLimitThresholdsRepresentation GetThresholdLimits(int accountId, string currency);

        /// <summary>
        /// Get the list of addresses for the current user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        IList<DepositAddressRepresentation> GetAddressesForAccount(int accountId, string currency);

        /// <summary>
        /// Make a deposit for a currency. This feature is not present to be called by front end
        /// </summary>
        /// <param name="makeDepositCommand"> </param>
        /// <returns></returns>
        bool MakeDeposit(MakeDepositCommand makeDepositCommand);

        /// <summary>
        /// Get the Monthly and daily Tier Limits for Deposit
        /// </summary>
        /// <returns></returns>
        DepositTierLimitRepresentation GetDepositTiersLimits();
    }
}
