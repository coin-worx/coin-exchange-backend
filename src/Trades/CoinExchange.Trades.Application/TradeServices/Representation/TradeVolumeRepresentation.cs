namespace CoinExchange.Trades.Application.TradeServices.Representation
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
