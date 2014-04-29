/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

using System;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Value Object that represents the internal TraderId associated with a trader
    /// </summary>
    [Serializable]
    public class TraderId
    {
        private readonly int _id;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="id"></param>
        public TraderId(int id)
        {
            _id = id;
        }

        /// <summary>
        /// The ID of the Trader
        /// </summary>
        public int Id { get { return _id; } }

        public override bool Equals(object obj)
        {
            if (obj is TraderId)
            {
                return Id == (obj as TraderId).Id;
            }
            return false;
        }
    }
}
