using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Utility
{
    /// <summary>
    /// Splits a currency pair into the two string that form the pair and returns the currencies as tuple: 
    /// Item 1 = Base Currency
    /// Item 2 = Quote Currency
    /// </summary>
    public static class CurrencySplitterService
    {
        /// <summary>
        /// Splits the currency pair depending upon the specified criteria of the currency pair. If the currency pair has 
        /// '/' between the currencies, it splitds currencies accordingly, otherwise, splits from the third number
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, string> SplitCurrencyPair(string currencyPair)
        {
            if (currencyPair.Contains("/"))
            {
                return new Tuple<string, string>(currencyPair.Split('/')[0], currencyPair.Split('/')[1]);
            }
            return new Tuple<string, string>(currencyPair.Substring(0, 3), currencyPair.Substring(3, 3));
        }
    }
}
