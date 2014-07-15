namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Represents a single currency - VO
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Currency()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Currency(string name, bool isCryptoCurrency)
        {
            Name = name;
            IsCryptoCurrency = isCryptoCurrency;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the currency a crypto currency(XRP, XBT, LTC etc.) or a fiat currency(USD, EUR etc.)
        /// </summary>
        public bool IsCryptoCurrency { get; set; }
    }
}
