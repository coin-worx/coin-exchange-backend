using System;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Specifies the Tate for a Currency Pair
    /// </summary>
    [Serializable]
    public class Rate
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Rate(string currencyPair, decimal rateValue)
        {
            CurrencyPair = currencyPair;
            RateValue = rateValue;
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair { get; private set; }

        /// <summary>
        /// Rate of the CurrecnyPair
        /// </summary>
        public decimal RateValue { get; private set; }
    }
}
