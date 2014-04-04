namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade
{
    /// <summary>
    /// Contians params required for Trades/Querytrades Http request action
    /// </summary>
    public class QueryTradeParams
    {
        private string _txId;

        private bool _includeTrades = false;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="includeTrades"></param>
        public QueryTradeParams(string txId, bool includeTrades)
        {
            _txId = txId;
            _includeTrades = includeTrades;
        }

        /// <summary>
        /// TransactionId (Optional)
        /// </summary>
        public string TxId { get { return _txId; } }

        /// <summary>
        /// Should the reposnce include Trades or not(Optional)
        /// </summary>
        public bool IncludeTrades { get { return _includeTrades; } }
    }
}
