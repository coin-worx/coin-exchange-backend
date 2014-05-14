namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade
{
    /// <summary>
    /// COntians params for the request of TradeHistory action call
    /// </summary>
    public class TradeHistoryParams
    {
        public TradeHistoryParams(string end, string start)
        {
            End = end;
            Start = start;
        }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
