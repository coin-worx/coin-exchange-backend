using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order
{
    public class CreateOrderParam
    {
        public string Pair;
        public string Side;
        public string Type;
        public decimal Price;
        public decimal Price2;
        public decimal Volume;
        public string Leverage="";
        public string Position;
        public string Oflags;
        public string Starttm;
        public string Expiretm;
        public string Userref;
        public bool Validate;

    }
}
