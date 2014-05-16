/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

using System;
using System.Globalization;
using CoinExchange.Common.Domain.Model;
using Newtonsoft.Json;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Represents the ID for an order. ValueObject
    /// </summary>
    [Serializable]
    public class OrderId
    {
        private readonly string _id;

        /// <summary>
        /// Accepts integer ID 
        /// </summary>
        /// <param name="id"></param>
        /*public OrderId(int id)
        {
            _id = id.ToString(CultureInfo.InvariantCulture);
        }*/

        /// <summary>
        /// Accepts string ID
        /// </summary>
        /// <param name="id"></param>
        [JsonConstructor]
        public OrderId(string id)
        {
            AssertionConcern.AssertNullOrEmptyString(id,"OrderId cannot be null");
            _id = id;
        }

        /// <summary>
        /// The ID of the Order
        /// </summary>
        public string Id { get { return _id; } }

        public override bool Equals(object obj)
        {
            if (obj is OrderId)
            {
                return Id == (obj as OrderId).Id;
            }
            return false;
        }
    }
}
