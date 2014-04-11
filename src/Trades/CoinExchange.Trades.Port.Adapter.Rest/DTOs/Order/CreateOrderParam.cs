using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order
{
    /// <summary>
    /// Order parameters to be received from user
    /// </summary>
    public class CreateOrderParam
    {
        public string Pair;
        public string Side;
        public string Type;
        public decimal Price;
        public decimal Volume;
    }
}
