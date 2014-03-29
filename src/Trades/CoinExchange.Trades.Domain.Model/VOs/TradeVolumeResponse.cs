using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class TradeVolumeResponse
    {
        public TradeVolumeResponse(Fees fees, decimal volume, string currency)
        {
            this.fees = fees;
            Volume = volume;
            Currency = currency;
        }

        public string Currency { get; private set; }
        public decimal Volume { get; private set; }
        public Fees fees { get; private set; }
    }
}
