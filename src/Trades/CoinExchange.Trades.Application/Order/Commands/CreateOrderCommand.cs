using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Application.Order.Commands
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand(decimal price, string type, string side, string pair)
        {
            Price = price;
            Type = type;
            Side = side;
            Pair = pair;
        }

        public string Pair { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public decimal Price { get; private set; }
    }
}
