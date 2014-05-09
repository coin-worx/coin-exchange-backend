using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class RecentTradeModel
    {
        public DateTime ExecutionDateTime { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}
