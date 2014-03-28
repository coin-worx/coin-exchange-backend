/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// Represents the ID for an order. ValueObject
    /// </summary>
    public class OrderId
    {
        private readonly int _id;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="id"></param>
        public OrderId(int id)
        {
            _id = id;
        }

        /// <summary>
        /// The ID of the Order
        /// </summary>
        public int Id { get { return _id; } }
    }
}
