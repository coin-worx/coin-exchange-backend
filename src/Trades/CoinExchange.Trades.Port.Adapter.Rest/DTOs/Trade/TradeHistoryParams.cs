namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade
{
    /// <summary>
    /// COntians params for the request of TradeHistory action call
    /// </summary>
    public class TradeHistoryParams
    {
        public string Offset { get; set; }
        public string Type { get; set; }
        public bool Trades { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
