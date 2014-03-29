using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class AssetFee
    {
        public decimal Volume { get; private set; }
        public decimal PercentFee { get; private set; }

        public AssetFee(decimal percentFee, decimal volume)
        {
            PercentFee = percentFee;
            Volume = volume;
        }
    }
}
