/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Status of the CoinExchangeOrder
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Order pending book entry
        /// </summary>
        New,
        /// <summary>
        /// Open order
        /// </summary>
        Accepted,
        /// <summary>
        /// Closed order
        /// </summary>
        Rejected,
        /// <summary>
        /// Order canceled
        /// </summary>
        Cancelled,
        /// <summary>
        /// Order expired
        /// </summary>
        Expired,
        /// <summary>
        /// order Filled Partially
        /// </summary>
        PartiallyFilled,
        /// <summary>
        /// Order Filled Completely
        /// </summary>
        FullyFilled,
        /// <summary>
        /// Order Complete
        /// </summary>
        Complete
    }
}
