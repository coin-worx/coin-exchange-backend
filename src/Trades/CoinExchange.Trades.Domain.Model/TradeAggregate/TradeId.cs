using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// VO for unique trade id
    /// </summary>
    [Serializable]
    public class TradeId
    {
         private readonly int _id;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="id"></param>
         public TradeId(int id)
        {
            _id = id;
        }

        /// <summary>
        /// The ID of the Trade
        /// </summary>
        public int Id { get { return _id; } }

        public override bool Equals(object obj)
        {
            if (obj is TradeId)
            {
                return Id == (obj as TradeId).Id;
            }
            return false;
        }
    }
}
