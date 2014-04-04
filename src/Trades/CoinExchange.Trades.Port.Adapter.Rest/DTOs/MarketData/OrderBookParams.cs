namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.MarketData
{
    /// <summary>
    /// Contains the parameters for orders/orderbook Http request action
    /// </summary>
    public class OrderBookParams
    {
        private string _currencyPair;

        private int _count = 0;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="count"></param>
        public OrderBookParams(string currencyPair, int count)
        {
            _currencyPair = currencyPair;
            _count = count;
        }

        /// <summary>
        /// CurrencyPair(Required)
        /// </summary>
        public string CurrencyPair { get { return _currencyPair; } }

        /// <summary>
        /// Count of the OrderBook(Optional)
        /// </summary>
        public int Count { get { return _count; } }
    }
}
