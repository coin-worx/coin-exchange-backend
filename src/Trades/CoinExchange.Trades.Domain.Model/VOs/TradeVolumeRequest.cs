using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    /// <summary>
    /// VO for trade volume request
    /// </summary>
   public class TradeVolumeRequest
   {
       public string TraderId { get; private set; }
       public string Pair { get; private set; }

        public TradeVolumeRequest(string pair, string traderId)
        {
            Pair = pair;
            TraderId = traderId;
        }
   }
}
