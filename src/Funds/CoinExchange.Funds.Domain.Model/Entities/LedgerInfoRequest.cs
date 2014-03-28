/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Funds.Domain.Model.Entities
{
    /// <summary>
    /// The request object for the LedgerInfo
    /// </summary>
    public class LedgerInfoRequest
    {
        /// <summary>
        /// Asset Class e.g., Currency etc.
        /// </summary>
        public string AssetClass { get; set; }

        /// <summary>
        /// Asset e.g., XBT, USD etc
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Type of Ledger, e.g., Trade etc
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Starting Timestamp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Ending TimeStamp
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Result Offset
        /// </summary>
        public string Offset { get; set; }
    }
}
