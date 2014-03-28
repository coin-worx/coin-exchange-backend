/*
 * Author: Waqas
 * Comany: Aurora Solutions
*/

namespace CoinExchange.Funds.Domain.Model.Entities
{
    /// <summary>
    /// Represents the Account Balance for a user for a particular currency
    /// </summary>
    public class AccountBalance
    {
        /// <summary>
        /// Name of the asset
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// The balance for the asset that this user owns
        /// </summary>
        public double Balance { get; set; }
    }
}