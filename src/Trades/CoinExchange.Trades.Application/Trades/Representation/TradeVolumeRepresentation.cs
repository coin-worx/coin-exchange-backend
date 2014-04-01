using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Application.Trades.Representation
{
    /// <summary>
    /// serves the representation for trade volume
    /// </summary>
    public class TradeVolumeRepresentation
    {
        public TradeVolumeRepresentation(TradeFeeRepresentation fees, decimal volume, string currency)
        {
            this.Fees = fees;
            Volume = volume;
            Currency = currency;
        }

        public string Currency { get; private set; }
        public decimal Volume { get; private set; }
        public TradeFeeRepresentation Fees { get; private set; }
    }
}
