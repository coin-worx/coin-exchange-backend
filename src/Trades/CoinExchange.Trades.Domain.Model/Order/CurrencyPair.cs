using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// currency pair value object
    /// </summary>
    public class CurrencyPair
    {
        private string _baseCurrency;
        private string _quoteCurrency;

        public string BaseCurrency
        {
            get { return _baseCurrency; }
            private set { _baseCurrency = value; }
        }

        public string QuoteCurrency
        {
            get { return _quoteCurrency; }
            private set { _quoteCurrency = value; }
        }

        public CurrencyPair(string quoteCurrency, string baseCurrency)
        {
            QuoteCurrency = quoteCurrency;
            BaseCurrency = baseCurrency;
        }

        public override string ToString()
        {
            return "Base Currency=" + BaseCurrency + ", QuoteCurrency=" + QuoteCurrency;
        }

        public override bool Equals(object obj)
        {
            CurrencyPair currencyPair = obj as CurrencyPair;
            if (currencyPair == null)
            {
                return false;
            }
            return (_baseCurrency.Equals(currencyPair.BaseCurrency, StringComparison.CurrentCultureIgnoreCase) &&
                    _quoteCurrency.Equals(currencyPair.QuoteCurrency, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
