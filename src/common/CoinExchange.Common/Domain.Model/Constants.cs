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
        public const int DISRUPTOR_RING_SIZE = 16;//should always be multiple of 2
        public const string RAVEN_DB_DATABASE_NAME = "EventStore";
        public const string RAVEN_DB_CONNECTIONSTRING_NAME = "EventStore";
        public const int OUTPUT_DISRUPTOR_BYTE_ARRAY_SIZE = 200000;
        public const string INPUT_EVENT_STORE = "InputEventStore";
        public const string OUTPUT_EVENT_STORE = "OutputEventStore";

        static Constants()
        {
            //initiliaze trader ids
            _apiKeyToTraderId.Add("123456789", "122334455");
            _apiKeyToTraderId.Add("55555", "11111");
            _apiKeyToTraderId.Add("44444", "22222");
            _apiKeyToTraderId.Add("33333", "33333");
            _apiKeyToTraderId.Add("22222", "44444");
            _apiKeyToTraderId.Add("11111", "55555");

            //initialize secret keys
            _apiKeyToSecretKey.Add("55555", "s3cr3t");
            _apiKeyToSecretKey.Add("44444", "s3cr3t");
            _apiKeyToSecretKey.Add("33333", "s3cr3t");
            _apiKeyToSecretKey.Add("22222", "s3cr3t");
            _apiKeyToSecretKey.Add("11111", "s3cr3t");
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
