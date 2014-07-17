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
        public Fee()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Fee(string currencyPair, double percentageFee, double amount)
        {
            CurrencyPair = currencyPair;
            PercentageFee = percentageFee;
            Amount = amount;
        }

        /// <summary>
        /// Primary key for database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair { get; private set; }

        /// <summary>
        /// Percentage Fee
        /// </summary>
        public double PercentageFee { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; private set; }
    }
}
