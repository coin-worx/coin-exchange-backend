using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Application.Order.Representation
{
    /// <summary>
    /// New order created representation when a new order is placed
    /// </summary>
    public class NewOrderRepresentation
    {
        public NewOrderRepresentation(decimal price, string type, string side, string pair, string txid)
        {
            Price = price;
            Type = type;
            Side = side;
            Pair = pair;
            Txid = txid;
        }

        public string Txid { get; private set; }
        public string Pair { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public decimal Price { get; private set; }

    }
}
