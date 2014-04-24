namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    //TODO: Can be removed in the future but for now it is a VO to hold a trade record.
    /// <summary>
    /// VO to hold recent trade records
    /// </summary>
    public class TradeRecord
    {
        public TradeRecord(decimal price, decimal volume, string side, string type, string miscellaneous, string dateTime)
        {
            Price = price;
            this.volume = volume;
            Side = side;
            Type = type;
            Miscellaneous = miscellaneous;
            DateTime = dateTime;
        }

        public decimal Price { get; private set; }
        public decimal volume { get; private set; }
        public string DateTime { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public string Miscellaneous { get; private set; }

    }
}
