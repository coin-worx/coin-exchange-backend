/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// Value Object that represents the internal TraderId associated with a trader
    /// </summary>
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
    }
}
