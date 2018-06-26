/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
        public const string INPUT_EVENT_STORE = "InputEventStoreDev";
        public const string OUTPUT_EVENT_STORE = "OutputEventStoreDev";
        public const string USER_DOCUMENT_PATH = @"D:\CoinExchangeFiles\";
        public static DateTime LastSnapshotSearch = DateTime.Today.AddDays(-4);
        public const double SnaphsortInterval = 3600000;//hourly
        public static DateTime LoadEventsFrom = DateTime.MinValue;

        static Constants()
        {
            //initiliaze trader ids
            _apiKeyToTraderId.Add("123456789", "1234567");
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
