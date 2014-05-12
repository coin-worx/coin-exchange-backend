using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// serves for the purpose of constants throug out the domain
    /// </summary>
    [Serializable]
    public static class Constants
    {
        private static readonly Dictionary<string, string> _apiKeyToTraderId = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _apiKeyToSecretKey = new Dictionary<string, string>();
 
        // ReSharper disable InconsistentNaming
        public const string ORDER_TYPE_LIMIT = "limit";
        public const string ORDER_TYPE_MARKET = "market";
        public const string ORDER_SIDE_BUY = "buy";
        public const string ORDER_SIDE_SELL = "sell";
        public const int DISRUPTOR_RING_SIZE = 8;//should always be multiple of 2
        public const string RAVEN_DB_DATABASE_NAME = "EventStore";
        public const string RAVEN_DB_CONNECTIONSTRING_NAME = "EventStore";
        public const int OUTPUT_DISRUPTOR_BYTE_ARRAY_SIZE = 200000;
        public const string INPUT_EVENT_STORE = "InputEventStore";
        public const string OUTPUT_EVENT_STORE = "OutputEventStore";

        static Constants()
        {
            _apiKeyToTraderId.Add("123456789", "122334455");
            _apiKeyToTraderId.Add("123456", "887766");
            _apiKeyToSecretKey.Add("123456789", "09887960");
        }

        /// <summary>
        /// Returns the TraderId given the API key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static string GetTraderId(string apiKey)
        {
            string traderid = string.Empty;
            _apiKeyToTraderId.TryGetValue(apiKey, out traderid);

            return traderid;
        }

        /// <summary>
        /// Returns the Secret Key given the API key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static string GetSecretKey(string apiKey)
        {
            string secretKey = string.Empty;
            _apiKeyToSecretKey.TryGetValue(apiKey, out secretKey);

            return secretKey;
        }
    }
}
