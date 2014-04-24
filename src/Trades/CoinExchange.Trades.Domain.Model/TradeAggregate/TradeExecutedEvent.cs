namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Event resulting after a trade execution
    /// </summary>
    public class TradeExecutedEvent
    {
        private readonly string _aggregateId = string.Empty;
        private readonly Trade _trade = null;
        private readonly string _version = string.Empty;

        public TradeExecutedEvent(string aggregateId, Trade trade)
        {
            _aggregateId = aggregateId;
            _trade = trade;

            // ToDo: Need to implement aa way to create a new Version id for every Event
        }

        /// <summary>
        /// Aggregate Id
        /// </summary>
        public string AggregateId
        {
            get { return _aggregateId; }
        }

        /// <summary>
        /// Trade
        /// </summary>
        public Trade Trade
        {
            get { return _trade; }
        }

        /// <summary>
        /// Version of the TradeExecutedEvent
        /// </summary>
        public string Version
        {
            get { return _version; }
        }
    }
}
