/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// Status of the CoinExchangeOrder
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order pending book entry
        /// </summary>
        Pending,
        /// <summary>
        /// Open order
        /// </summary>
        Open,
        /// <summary>
        /// Closed order
        /// </summary>
        Closed,
        /// <summary>
        /// Order canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// Order expired
        /// </summary>
        Expired,
    }
}
