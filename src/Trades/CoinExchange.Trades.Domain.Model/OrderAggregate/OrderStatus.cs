/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

using System;
using System.Runtime.Serialization;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Status of the CoinExchangeOrder
    /// </summary>
    [Serializable]
    public enum OrderState
    {
        /// <summary>
        /// Order pending book entry
        /// </summary>
        [DataMember]
        New,
        /// <summary>
        /// Open order
        /// </summary>
        [DataMember]
        Accepted,
        /// <summary>
        /// Closed order
        /// </summary>
        [DataMember]
        Rejected,
        /// <summary>
        /// Order canceled
        /// </summary>
        [DataMember]
        Cancelled,
        /// <summary>
        /// Order expired
        /// </summary>
        [DataMember]
        Expired,
        /// <summary>
        /// order Filled Partially
        /// </summary>
        [DataMember]
        PartiallyFilled,
        /// <summary>
        /// Order Filled Completely
        /// </summary>
        [DataMember]
        FullyFilled,
        /// <summary>
        /// Order Complete
        /// </summary>
        [DataMember]
        Complete
    }
}
