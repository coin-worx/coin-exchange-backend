using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.MarketData
{
    /// <summary>
    /// Contains the parameters for marketData/ohlcinfo Http request action
    /// </summary>
    public class OhlcParams
    {
        private string _currencypair = string.Empty;
        private string _since = string.Empty;
        private int _interval = 0;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="currencypair"> </param>
        /// <param name="interval"> </param>
        /// <param name="since"> </param>
        public OhlcParams(string currencypair,int interval, string since)
        {
            _currencypair = currencypair;
            _since = since;
        }

        /// <summary>
        /// Currency pair(Required)
        /// </summary>
        public string CurrencyPair { get { return _currencypair; } }

        /// <summary>
        /// Interval(optional)
        /// </summary>
        public int interval { get { return _interval; } }

        /// <summary>
        /// Recent trades since (optional)
        /// </summary>
        public string Since { get { return _since; } }
    }
}
