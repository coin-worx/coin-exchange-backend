namespace CoinExchange.Funds.Domain.Model.FeeAggregate
{
    /// <summary>
    /// Trading Fee for currency pair
    /// </summary>
    public class Fee
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Fee(string currencyPair, string percentageFee, string amount)
        {
            CurrencyPair = currencyPair;
            PercentageFee = percentageFee;
            Amount = amount;
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair { get; private set; }

        /// <summary>
        /// Percentage Fee
        /// </summary>
        public string PercentageFee { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public string Amount { get; private set; }
    }
}
