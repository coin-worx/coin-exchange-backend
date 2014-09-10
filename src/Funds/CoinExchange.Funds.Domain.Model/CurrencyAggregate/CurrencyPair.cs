using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.CurrencyAggregate
{
    /// <summary>
    /// Currency Pair for Funds BC
    /// </summary>
    public class CurrencyPair
    {
        /// <summary>
        /// Constructor for Nhibernate
        /// </summary>
        public CurrencyPair()
        {

        }

        /// <summary>
        /// Default Constructor for currency pair
        /// </summary>
        /// <param name="currencyPairName"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="baseCurrency"></param>
        public CurrencyPair(string currencyPairName, string quoteCurrency, string baseCurrency)
        {
            CurrencyPairName = currencyPairName;
            QuoteCurrency = quoteCurrency;
            BaseCurrency = baseCurrency;
        }

        /// <summary>
        /// Base Currency
        /// </summary>
        public string BaseCurrency { get; private set; }

        /// <summary>
        /// Quote Currency
        /// </summary>
        public string QuoteCurrency { get; private set; }

        /// <summary>
        /// Currency Pair Name
        /// </summary>
        public string CurrencyPairName { get; private set; }
    }
}
