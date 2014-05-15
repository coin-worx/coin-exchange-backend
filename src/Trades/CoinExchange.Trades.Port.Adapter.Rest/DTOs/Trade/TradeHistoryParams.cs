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

        /// <summary>
        /// Custom to string method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Trade History Params, Start={0}, End={1}", Start, End);
        }
    }
}
