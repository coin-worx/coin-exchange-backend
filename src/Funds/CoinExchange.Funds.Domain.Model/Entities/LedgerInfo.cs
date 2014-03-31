/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Funds.Domain.Model.Entities
{
    /// <summary>
    /// Contains Ledger Info
    /// </summary>
    public class LedgerInfo
    {
        /// <summary>
        /// Reference ID
        /// </summary>
        public int RefId { get; set; }

        /// <summary>
        /// Timestamp of ledger(Represented in string, can be modified)
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Type of Ledger Entry e.g., Trade
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Asset class, e.g., Currency
        /// </summary>
        public string AssetClass { get; set; }

        /// <summary>
        /// Asset, e.g., XXRP
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Transaction Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Transaction Fee
        /// </summary>
        public double Fee { get; set; }

        /// <summary>
        /// Resulting Balance
        /// </summary>
        public double  Balance { get; set; }
    }
}