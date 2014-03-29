using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class Fees
    {
        public Fees(decimal tierVolume, decimal nextVoulme, decimal nextFee, decimal maxFee, decimal minFee, decimal fee)
        {
            TierVolume = tierVolume;
            NextVoulme = nextVoulme;
            NextFee = nextFee;
            MaxFee = maxFee;
            MinFee = minFee;
            Fee = fee;
        }

        public decimal Fee { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
        public decimal NextFee { get; set; }
        public decimal NextVoulme { get; set; }
        public decimal TierVolume { get; set; }
        
    }
}
