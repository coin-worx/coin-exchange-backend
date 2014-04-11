namespace CoinExchange.Trades.Application.TradeServices.Representation
{
    /// <summary>
    /// Serves the representation for trading fee.
    /// </summary>
    public class TradeFeeRepresentation
    {
        public TradeFeeRepresentation(decimal tierVolume, decimal nextVoulme, decimal nextFee, decimal maxFee, decimal minFee, decimal fee)
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
