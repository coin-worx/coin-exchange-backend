using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.CurrencyPairAggregate
{
    /// <summary>
    /// Currency Pair VO
    /// </summary>
    public class CurrencyPair
    {
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public string CurrencyPairName { get; private set; }

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
        /// Constructor for Nhibernate
        /// </summary>
        public CurrencyPair()
        {
            
        }
    }
}
